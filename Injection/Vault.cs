using Hopeful.Injection.Resolvers;
using Hopeful.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hopeful.Injection;

public sealed class Vault : IDisposable
{
    public Vault? Parent => _parent;
    private readonly Vault? _parent;

    public Vault() { }
    public Vault(Vault parent)
    {
        _parent = parent;
        _services = Services.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    private readonly Dictionary<Type, InjectionInfo[]> _injections = []; // <ClientType, ClientTypeInjections[]>
    private readonly Dictionary<(object? serviceKey, Type serviceType), object> _services = []; // <(ServiceKey, ServiceType (can be interface)), ServiceInstance>
    private readonly Dictionary<(object? serviceKey, Type serviceType), List<Func<object, object>>> _decorators = []; // Func<OriginalService, DecoratorService>

    public T CreateWithInjection<T>() where T : new()
    {
        var instance = new T();
        Inject(instance);
        return instance;
    }

    public object Inject(object clientInstance)
    {
        if (_injections.TryGetValue(clientInstance.GetType(), out var injections))
            foreach (var injection in injections)
                injection.Resolver.Resolve(injection, clientInstance, Resolve(injection));
        return clientInstance;
    }

    private object? Resolve(InjectionInfo info)
    {
        object? injection = null;
        var type = info.TypeToken;

        if (type.TryGetCustomAttribute<ServiceAttribute>(out var service))
            return info.DecoratorTargetType != null ? InjectService(type, info.DecoratorTargetType, service.Key) : InjectService(type, service.Key);

        // If the injection is required, attempt to create it
        if (!info.IsOptional)
        {
            if (type.IsInterface || type.IsAbstract) throw new InvalidOperationException();
            injection = Activator.CreateInstance(type);
        }

        return injection;
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
        var graph = new Graph<Type>();
        Type? fist = null;

        foreach (var service in services)
        {
            var type = service.ServiceType ?? service.Type;
            fist ??= type;

            graph.AddVertex(type);

            if (!_injections.TryGetValue(type, out var injections)) continue;
            var innerServices = injections.Where(i => i.TypeToken.TryGetCustomAttribute<ServiceAttribute>(out _));
            foreach (var inner in innerServices)
            {
                graph.AddVertex(inner.TypeToken);
                graph.AddEdge(type, inner.TypeToken);
            }
        }

        var sortedServices = graph.DfsSort(fist!);
        const BindingFlags flag = BindingFlags.Public | BindingFlags.Instance;
        foreach (var serviceType in sortedServices)
        {
            var serviceInfo = services.First(s => s.Type == serviceType);

            var methodInfo = typeof(Vault).GetMethod(nameof(ExtractService), flag, null, [typeof(object), typeof(bool)], null)!.MakeGenericMethod(serviceInfo.Type, serviceInfo.ServiceType ?? serviceInfo.Type);
            methodInfo.Invoke(this, [serviceInfo.Key, true]);
            //ExtractService(serviceInfo.Type, serviceInfo.ServiceType ?? serviceInfo.Type, serviceInfo.Key, true);
        }

        // Inject
        foreach (var serviceType in sortedServices)
        {
            var serviceInfo = services.First(s => s.Type == serviceType);
            var instance = _services[(serviceInfo.Key, serviceInfo.ServiceType ?? serviceInfo.Type)];
            Inject(instance);
        }
    }

    public void LoadDecorators(Assembly assembly)
    {
        var decorators = FindDecorators(assembly);
        const BindingFlags flag = BindingFlags.Public | BindingFlags.Instance;
        foreach (var (Key, DecoratorType, ServiceType) in decorators)
        {
            var methodInfo = typeof(Vault).GetMethod(nameof(ExtractDecorator), flag, null, [typeof(Func<object, object>), typeof(object)], null)!.MakeGenericMethod(ServiceType);
            Func<object, object> factory = inner => Activator.CreateInstance(DecoratorType, [inner])!;
            methodInfo.Invoke(this, [factory, Key]);
        }
    }

    private static List<(Type Type, InjectionInfo[] Injections)> FindInjections(Assembly assembly)
    {
        var types = assembly.GetTypes();
        const BindingFlags anyFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        List<(Type Type, InjectionInfo[] Injections)> list = [];
        List<InjectionInfo> typeList = [];

        for (int i = 0; i < types.Length; i++)
        {
            foreach (FieldInfo field in types[i].GetFields(anyFlags))
                if (field.TryGetCustomAttribute(out InjectAttribute? attr))
                {
                    IInjectionResolver resolver = new FieldInjectionResolver(field);
                    bool isOptional = field.GetNullability() == Nullability.Nullable;
                    typeList.Add(new(field.FieldType, resolver, isOptional, attr.DecoratorTargetType));
                }

            foreach (PropertyInfo property in types[i].GetProperties(anyFlags))
                if (property.TryGetCustomAttribute(out InjectAttribute? attr))
                {
                    IInjectionResolver resolver = new PropertyInjectionResolver(property);
                    bool isOptional = property.GetNullability() == Nullability.Nullable;
                    typeList.Add(new(property.PropertyType, resolver, isOptional, attr.DecoratorTargetType));
                }

            if (typeList.IsNotEmpty())
            {
                list.Add((types[i], typeList.ToArray()));
                typeList.Clear();
            }
        }
        return list;
    }

    private static IEnumerable<(object? Key, Type Type, Type? ServiceType)> FindServices(Assembly assembly)
        => assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Select(t => (Type: t, Attr: t.GetCustomAttribute<ServiceAttribute>()))
            .Where(x => x.Attr != null)
            .Select(x => (x.Attr!.Key, x.Type, x.Attr.ServiceType));

    private static List<(object? Key, Type DecoratorType, Type ServiceType)> FindDecorators(Assembly assembly)
    {
        var types = assembly.GetTypes();
        List<(object? Key, Type ServiceType, Type DecoratorType)> list = [];

        foreach (var type in types)
        {
            var baseType = type.BaseType;
            if (baseType?.IsGenericType == true && baseType.GetGenericTypeDefinition() == typeof(Decorator<>))
            {
                list.Add((null, baseType.GetGenericArguments()[0], type));
                continue;
            }

            if (type.TryGetCustomAttribute<DecoratorAttribute>(out var attr))
                list.Add((attr.Key, attr.Type, type));
        }
        return list;
    }

    public TService InjectService<TService>(object? key = null)
        => (TService)InjectService(typeof(TService), key);
    
    public object InjectService(Type serviceType, object? key = null)
        => _services.TryGetValue((key, serviceType), out var service) ? service : throw new InvalidOperationException($"Service Type {serviceType} does not exist.");

    public TService InjectService<TService, TDecorator>(object? key = null)
        => (TService)InjectService(typeof(TService), typeof(TDecorator), key);
    
    public object InjectService(Type serviceType, Type decoratorType, object? key = null)
    {
        var service = InjectService(serviceType, key);

        if (_decorators.TryGetValue((key, serviceType), out var decorator))
            foreach (var decorate in decorator)
            {
                service = decorate(service);
                if (decoratorType.IsAssignableFrom(service.GetType())) break;
            }
        return service;
    }

    public void ExtractService<TService, TServiceInstance>(object? key = null, bool skipInjection = false)
        => ExtractService<TService>(skipInjection ? Activator.CreateInstance<TServiceInstance>()! : Inject(Activator.CreateInstance<TServiceInstance>()!), key);

    public void ExtractService<TService>(object serviceInstance, object? key = null) // maybe add check for serviceInstance nullabulity
    {
        var serviceType = typeof(TService);
        var instanceType = serviceInstance.GetType();
        if (!serviceType.IsAssignableFrom(instanceType)) throw new InvalidOperationException($"Instance Type {instanceType} not assigned from Service Type {serviceType}.");
        if (!_services.TryAdd((key, serviceType), serviceInstance)) throw new InvalidOperationException($"Service Type {serviceType} with Key {key} already exist.");
    }

    public void ExtractDecorator<TService>(Func<object, object> decoratorFactory, object? serviceKey = null)
    {
        ArgumentNullException.ThrowIfNull(decoratorFactory);
        var key = (serviceKey, typeof(TService));
        _decorators.TryGetValue(key, out var list);
        _decorators[key] = list ??= [];
        list.Add(decoratorFactory);
    }

    public void ExtractDecorator<TService, TDecorator>(object? serviceKey = null)
    {
        var decorator = typeof(TDecorator);
        var service = typeof(TService);
        var ctor = decorator.GetConstructor([service]) ?? throw new InvalidOperationException($"Decorator Type {decorator} must have constructor accepting {service}");
        ExtractDecorator<TService>(s => (TService)ctor.Invoke([s]), serviceKey);
    }

    public IEnumerable<KeyValuePair<Type, InjectionInfo[]>> Injections => _injections.AsEnumerable();
    public IEnumerable<KeyValuePair<(object?, Type), object>> Services => _services.AsEnumerable();
    public IEnumerable<KeyValuePair<(object?, Type), List<Func<object, object>>>> Decorators => _decorators.AsEnumerable();

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
