using Hopeful.Injection.Resolvers;
using Hopeful.Utilities;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hopeful.Injection;

public sealed class Vault : IDisposable
{
    /// <summary>
    /// Gets the parent vault, if any.
    /// </summary>
    public Vault? Parent => _parent;
    private readonly Vault? _parent;

    public Vault() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Vault"/> class with a parent vault.
    /// </summary>
    /// <param name="parent">The parent vault to inherit services from.</param>
    public Vault(Vault parent)
    {
        _parent = parent;
        _services = EnumerateServices().ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    private readonly Dictionary<Type, InjectionInfo[]> _injections = []; // <ClientType, ClientTypeInjections[]>
    private readonly Dictionary<(object? serviceKey, Type serviceType), object> _services = []; // <(ServiceKey, ServiceType (can be interface)), ServiceInstance>
    private readonly Dictionary<(object? serviceKey, Type serviceType), List<Func<object, object>>> _decorators = []; // Func<OriginalService, DecoratorService>

    /// <summary>
    /// Creates a new instance of <typeparamref name="T"/> and injects dependencies into it.
    /// </summary>
    /// <typeparam name="T">The type to instantiate and inject.</typeparam>
    /// <returns>The created and injected instance.</returns>
    public T CreateWithInjection<T>() where T : class, new()
    {
        var instance = new T();
        Inject(instance);
        return instance;
    }

    /// <summary>
    /// Injects dependencies into the specified client instance.
    /// </summary>
    /// <param name="clientInstance">The object to inject dependencies into.</param>
    public void Inject(object clientInstance)
    {
        if (_injections.TryGetValue(clientInstance.GetType(), out var injections))
        {
            foreach (var injection in injections)
            {
                object? injectInstance = Resolve(injection);
                injection.Resolver.Resolve(injection, clientInstance, injectInstance);
            }
        }
    }

    private object? Resolve(InjectionInfo info)
    {
        object? injection = null;
        var type = info.TypeToken;

        if (type.TryGetCustomAttribute<ServiceAttribute>(out var service))
            return InjectService(type, service.Key, info.DecoratorTargetType);

        // If the injection is required, attempt to create it
        if (!info.IsOptional)
        {
            if (type.IsInterface || type.IsAbstract) throw new InvalidOperationException();
            injection = Activator.CreateInstance(type);
        }

        return injection;
    }

    /// <summary>
    /// Loads all injectable fields and properties from the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly to scan for injectable members.</param>
    public void LoadInjections(Assembly assembly)
    {
        var entry = FindInjections(assembly);
        foreach (var (type, injections) in entry)
            _injections.Add(type, injections);
    }

    /// <summary>
    /// Loads all services marked with <see cref="ServiceAttribute"/> from the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly to scan for services.</param>
    public void LoadServices(Assembly assembly)
    {
        var services = FindServices(assembly);
        foreach (var service in services)
            ExtractService(service.Type, service.ServiceType ?? service.Type, service.Key);
    }

    /// <summary>
    /// Loads all decorators from the specified assembly.
    /// </summary>
    /// <remarks>
    /// This method registers as decorators both:
    /// <list type="bullet">
    ///   <item>
    ///     <description>Classes marked with <see cref="DecoratorAttribute"/>.</description>
    ///   </item>
    ///   <item>
    ///     <description>Classes that inherit from <c>Decorator&lt;T&gt;</c> (generic base class).</description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <param name="assembly">The assembly to scan for decorators.</param>
    public void LoadDecorators(Assembly assembly)
    {
        var decorators = FindDecorators(assembly);
        foreach (var (ServiceType, DecoratorType, Key) in decorators)
            ExtractDecorator(ServiceType, inner => Activator.CreateInstance(DecoratorType, [inner])!, Key);
    }

    [Pure]
    private static List<(Type type, InjectionInfo[] injections)> FindInjections(Assembly assembly)
    {
        var types = assembly.GetTypes();
        const BindingFlags anyFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        List<(Type type, InjectionInfo[] injections)> list = [];
        List<InjectionInfo> typeList = [];

        for (int i = 0; i < types.Length; i++)
        {
            foreach (FieldInfo field in types[i].GetFields(anyFlags))
                if (field.TryGetCustomAttribute(out InjectAttribute? attr))
                {
                    IInjectionResolver resolver = new FieldInjectionResolver(field);
                    bool isOptional = field.GetNullability() == Nullability.Nullable;
                    typeList.Add(new InjectionInfo(field.FieldType, resolver, isOptional, attr.DecoratorTargetType));
                }

            foreach (PropertyInfo property in types[i].GetProperties(anyFlags))
                if (property.TryGetCustomAttribute(out InjectAttribute? attr))
                {
                    IInjectionResolver resolver = new PropertyInjectionResolver(property);
                    bool isOptional = property.GetNullability() == Nullability.Nullable;
                    typeList.Add(new InjectionInfo(property.PropertyType, resolver, isOptional, attr.DecoratorTargetType));
                }

            if (typeList.IsNotEmpty())
            {
                list.Add((types[i], typeList.ToArray()));
                typeList.Clear();
            }
        }
        return list;
    }

    [Pure]
    private static IEnumerable<(object? Key, Type Type, Type? ServiceType)> FindServices(Assembly assembly)
    {
        return assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Select(t => (Type: t, Attr: t.GetCustomAttribute<ServiceAttribute>()))
            .Where(x => x.Attr != null)
            .Select(x => (x.Attr!.Key, x.Type, x.Attr.ServiceType));
    }

    [Pure]
    private static List<(Type ServiceType, Type DecoratorType, object? Key)> FindDecorators(Assembly assembly)
    {
        var types = assembly.GetTypes();
        List<(Type ServiceType, Type DecoratorType, object? Key)> list = [];

        foreach (var type in types)
        {
            var baseType = type.BaseType;
            if (baseType?.IsGenericType == true && baseType.GetGenericTypeDefinition() == typeof(Decorator<>))
            {
                var serviceType = baseType.GetGenericArguments()[0];
                list.Add((serviceType, type, null));
                continue;
            }

            var attr = type.GetCustomAttribute<DecoratorAttribute>();
            if (attr != null)
                list.Add((attr.Type, type, attr.Key));
        }
        return list;
    }

    /// <summary>
    /// Resolves and returns a service instance by type and optional key, applying decorators if present.
    /// </summary>
    /// <param name="serviceType">The type of the service to resolve.</param>
    /// <param name="key">The optional key for the service.</param>
    /// <param name="decoratorTargetType">The type at which to stop applying decorators (optional).</param>
    /// <returns>The resolved service instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the service is not registered.</exception>
    [Pure]
    public object InjectService(Type serviceType, object? key = null, Type? decoratorTargetType = null)
    {
        if (_services.TryGetValue((key, serviceType), out var service))
        {
            if (_decorators.TryGetValue((key, serviceType), out var decorator))
                foreach (var decorate in decorator)
                {
                    service = decorate(service);
                    if (decoratorTargetType != null && decoratorTargetType.IsAssignableFrom(service.GetType())) break;
                }

            return service;
        }

        throw new InvalidOperationException($"Service Type {serviceType} does not exist.");
    }

    /// <summary>
    /// Registers a service type and its implementation type in the vault.
    /// </summary>
    /// <param name="serviceType">The service interface or base type.</param>
    /// <param name="serviceInstanceType">The concrete implementation type.</param>
    /// <param name="key">The optional key for the service.</param>
    public void ExtractService(Type serviceType, Type serviceInstanceType, object? key = null)
        => _services.TryAdd((key, serviceType), Activator.CreateInstance(serviceInstanceType)!);

    /// <summary>
    /// Registers a service instance in the vault.
    /// </summary>
    /// <param name="serviceType">The service interface or base type.</param>
    /// <param name="serviceInstance">The service instance.</param>
    /// <param name="key">The optional key for the service.</param>
    /// <exception cref="InvalidOperationException">Thrown if the instance type is not assignable to the service type, or if the service is already registered.</exception>
    public void ExtractService(Type serviceType, object serviceInstance, object? key = null)
    {
        var instanceType = serviceInstance.GetType();
        if (!serviceType.IsAssignableFrom(instanceType)) throw new InvalidOperationException($"Instance Type {instanceType} not assigned from Service Type {serviceType}.");
        if (!_services.TryAdd((key, serviceType), serviceInstance)) throw new InvalidOperationException($"Service Type {serviceType} with Key {key} already exist.");
    }

    /// <summary>
    /// Registers a decorator for a service type and optional key.
    /// </summary>
    /// <param name="serviceType">The service type to decorate.</param>
    /// <param name="decoratorFactory">The factory function that creates the decorator.</param>
    /// <param name="serviceKey">The optional key for the service.</param>
    public void ExtractDecorator(Type serviceType, Func<object, object> decoratorFactory, object? serviceKey = null)
    {
        ArgumentNullException.ThrowIfNull(decoratorFactory);
        var k = (serviceKey, serviceType);
        if (!_decorators.TryGetValue(k, out var list))
            _decorators[k] = list = [];
        list.Add(decoratorFactory);
    }

    /// <summary>
    /// Enumerates all registered injections.
    /// </summary>
    /// <returns>An enumerable of client types and their injection info arrays.</returns>
    [Pure]
    public IEnumerable<KeyValuePair<Type, InjectionInfo[]>> EnumerateInjections()
        => _injections.AsEnumerable();

    /// <summary>
    /// Enumerates all registered services.
    /// </summary>
    /// <returns>An enumerable of service keys/types and their instances.</returns>
    [Pure]
    public IEnumerable<KeyValuePair<(object?, Type), object>> EnumerateServices()
        => _services.AsEnumerable();

    /// <summary>
    /// Enumerates all registered decorators.
    /// </summary>
    /// <returns>An enumerable of service keys/types and their decorator factory lists.</returns>
    [Pure]
    public IEnumerable<KeyValuePair<(object?, Type), List<Func<object, object>>>> EnumerateDecorators()
        => _decorators.AsEnumerable();

    /// <summary>
    /// Disposes all registered services that implement <see cref="IDisposable"/> and clears all injections.
    /// </summary>
    public void Dispose()
    {
        var toRemove = _services.Where(pair => pair.Value is IDisposable).ToArray();
        foreach (var pair in toRemove)
        {
            ((IDisposable)pair.Value).Dispose();
            _services.Remove(pair.Key);
        }
        _services.Clear();
        _decorators.Clear();
        _injections.Clear();
    }
}
