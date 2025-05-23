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
    private readonly Vault? _parent;
    public Vault? Parent => _parent;

    public Vault() { }
    
    public Vault(Vault parent)
    {
        _parent = parent;
        _services = EnumerateServices().ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    private readonly Dictionary<Type, InjectionInfo[]> _injections = []; // <ClientType, ClientTypeInjections[]>
    private readonly Dictionary<(object? serviceKey, Type serviceType), object> _services = []; // <(ServiceKey, ServiceType (can be interface)), ServiceInstance>
    private readonly Dictionary<(object? serviceKey, Type serviceType), List<Func<object, object>>> _decorators = []; // Func<OriginalService, DecoratorService>

    public T CreateWithInjection<T>() where T : class, new()
    {
        var instance = new T();
        Inject(instance);
        return instance;
    }

    public void Inject(object client)
    {
        if (_injections.TryGetValue(client.GetType(), out var injections))
        {
            foreach (var injection in injections)
            {
                object? injectInstance = Resolve(injection);
                injection.Resolver.Resolve(injection, client, injectInstance);
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

        throw new NotSupportedException($"Type {type} cannot be injected as a dependency.");
    }

    public void LoadInjections(Assembly assembly)
    {
        var entry = FindInjections(assembly);
        foreach (var (type, injections) in entry)
            _injections.Add(type, injections);
    }

    public void LoadServices(Assembly assembly)
    {
        var services = FindServices(assembly);
        foreach (var service in services)
            ExtractService(service.Type, service.Type, service.Key);
    }

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
                    bool isOptional = attr.Optional ?? field.GetNullability() == Nullability.Nullable;
                    typeList.Add(new InjectionInfo(field.FieldType, resolver, isOptional, attr.DecoratorTargetType));
                }

            foreach (PropertyInfo property in types[i].GetProperties(anyFlags))
                if (property.TryGetCustomAttribute(out InjectAttribute? attr))
                {
                    IInjectionResolver resolver = new PropertyInjectionResolver(property);
                    bool isOptional = attr.Optional ?? property.GetNullability() == Nullability.Nullable;
                    typeList.Add(new InjectionInfo(property.PropertyType, resolver, isOptional, attr.DecoratorTargetType));
                }

            list.Add((types[i], typeList.ToArray()));
            typeList.Clear();
        }
        return list;
    }

    [Pure]
    private static IEnumerable<(object? Key, Type Type)> FindServices(Assembly assembly)
    {
        return assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Select(t => (Type: t, Attr: t.GetCustomAttribute<ServiceAttribute>()))
            .Where(x => x.Attr != null)
            .Select(x => (x.Attr!.Key, x.Type));
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

    [Pure]
    public object InjectService(Type type, object? key = null, Type? decoratorTargetType = null) // targetType only for decorators chain, support base class and interfaces
    {
        if (_services.TryGetValue((key, type), out var service))
        {
            if (_decorators.TryGetValue((key, type), out var decorator))
                foreach (var decorate in decorator)
                {
                    service = decorate(service);
                    if (decoratorTargetType != null && decoratorTargetType.IsAssignableFrom(service.GetType())) break;
                }

            return service;
        }

        throw new InvalidOperationException($"Service Type {type} is not exist.");
    }

    [Pure]
    public void ExtractService(Type type, Type instanceType, object? key = null)
        => _services.TryAdd((key, type), Activator.CreateInstance(instanceType)!);

    [Pure]
    public void ExtractService(Type type, object instance, object? key = null)
    {
        var instanceType = instance.GetType();
        if (!type.IsAssignableFrom(instanceType)) throw new InvalidOperationException($"Instance Type {instanceType} not equal Service Type {type}.");
        _services.TryAdd((key, type), instance);
    }

    [Pure]
    public void ExtractDecorator(Type serviceType, Func<object, object> decoratorFabric, object? serviceKey = null)
    {
        var k = (serviceKey, serviceType);
        if (!_decorators.TryGetValue(k, out var list))
            _decorators[k] = list = [];
        list.Add(decoratorFabric);
    }

    [Pure]
    public IEnumerable<KeyValuePair<Type, InjectionInfo[]>> EnumerateInjections()
        => _injections.AsEnumerable();

    [Pure]
    public IEnumerable<KeyValuePair<(object?, Type), object>> EnumerateServices()
        => _services.AsEnumerable();

    [Pure]
    public IEnumerable<KeyValuePair<(object?, Type), List<Func<object, object>>>> EnumerateDecorators()
        => _decorators.AsEnumerable();

    public void Dispose()
    {
        foreach (var pair in _services)
            if (pair.Value is IDisposable disposable)
            {
                disposable.Dispose();
                _services.Remove(pair.Key);
            }

        _injections.Clear();
    }
}
