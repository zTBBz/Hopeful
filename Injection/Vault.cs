using Hopeful.Injection.Resolvers;
using Hopeful.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hopeful.Injection;

public sealed class Vault : IDisposable
{
    public readonly Vault? Parent;

    public Vault() { }
    public Vault(Vault parent, VaultExportSettings settings = VaultExportSettings.Services)
    {
        Parent = parent;
        if (settings.HasFlag(VaultExportSettings.Services))
            _services = Services.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        if (settings.HasFlag(VaultExportSettings.Decorators))
            _decorators = Decorators.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    private readonly Dictionary<Type, InjectionInfo[]> _injections = []; // <ClientType, ClientTypeInjections[]>
    private readonly Dictionary<(object? serviceKey, Type serviceType), object> _services = []; // <(ServiceKey, ServiceType (can was interface)), ServiceInstance>
    private readonly Dictionary<(object? serviceKey, Type serviceType), List<Func<object, object>>> _decorators = []; // Func<OriginalService, DecoratorService>

    public T CreateWithInjection<T>() where T : new()
    {
        var instance = new T();
        Inject(instance);
        return instance;
    }

    public object Inject(in object clientInstance)
    {
        if (!_injections.TryGetValue(clientInstance.GetType(), out var injections)) return clientInstance;

        foreach (var injection in injections)
            injection.Resolver.Resolve(injection, clientInstance, Resolve(injection));

        return clientInstance;
    }

    private object? Resolve(InjectionInfo info)
    {
        object? injection = null;
        var type = info.TypeToken;
        var key = info.Key;
        
        if (!info.IsOptional)
        {
            if (type.IsAbstract && !type.IsInterface) throw new InvalidOperationException();

            injection = info.DecoratorTargetType != null ? InjectService(type, info.DecoratorTargetType, key) : InjectService(type, key);
        }

        return injection;
    }

    public void LoadInjections(Assembly assembly)
    {
        var entry = FindInjections(assembly);
        foreach (var (type, injections) in entry)
            _injections.Add(type, injections);
    }

    private Graph<Type> graph;

    public void LoadServices(Assembly assembly)
    {
        var services = FindServices(assembly);
        graph = new Graph<Type>();

        // Prepare graph
        foreach (var service in services)
            graph.AddVertex(service.Type); // graph.AddVertex(service.ServiceType ?? service.Type); remove Interfaces from graph

        // Add dependencies to graph
        foreach (var (clientType, injections) in _injections)
        {
            foreach (var injection in injections)
            {
                if (graph.ContainsVertex(injection.TypeToken) || _services.TryGetValue((injection.Key, injection.TypeToken), out _))
                    graph.AddEdge(injection.TypeToken, clientType);
            }
        }

        var sortedServices = graph.DfsSort();

        // Extract and Inject inner services
        foreach (var serviceType in sortedServices)
        {
            var serviceInfos = services.Where(s => s.Type == serviceType || serviceType.IsAssignableFrom(s.Type));
            //var serviceInfos = services.Where(s => (s.ServiceType ?? s.Type) == serviceType || serviceType.IsAssignableFrom(s.ServiceType ?? s.Type));

            foreach (var serviceInfo in serviceInfos)
            {
                var instance = Activator.CreateInstance(serviceInfo.Type)!;

                ExtractService(serviceInfo.ServiceType ?? serviceInfo.Type, instance, serviceInfo.Key);

                Inject(instance);
            }
        }
    }

    public void LoadDecorators(Assembly assembly)
    {
        var decorators = FindDecorators(assembly);
        foreach (var (key, decoratorType, serviceType) in decorators)
            ExtractDecorator(serviceType, inner => Activator.CreateInstance(decoratorType, [inner])!, key);
    }

    private static List<(Type Type, InjectionInfo[] Injections)> FindInjections(Assembly assembly)
    {
        var types = assembly.GetTypes();
        const BindingFlags anyFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        List<(Type Type, InjectionInfo[] Injections)> list = [];
        List<InjectionInfo> typeList = [];

        for (var i = 0; i < types.Length; i++)
        {
            foreach (var field in types[i].GetFields(anyFlags))
                if (field.TryGetCustomAttribute(out InjectAttribute? attr))
                {
                    IInjectionResolver resolver = new FieldInjectionResolver(field);
                    var isOptional = field.GetNullability() == Nullability.Nullable;
                    typeList.Add(new(field.FieldType, resolver, isOptional, attr.Key, attr.DecoratorTargetType));
                }

            foreach (var property in types[i].GetProperties(anyFlags))
                if (property.TryGetCustomAttribute(out InjectAttribute? attr))
                {
                    IInjectionResolver resolver = new PropertyInjectionResolver(property);
                    var isOptional = property.GetNullability() == Nullability.Nullable;
                    typeList.Add(new(property.PropertyType, resolver, isOptional, attr.Key, attr.DecoratorTargetType));
                }

            if (typeList.IsNotEmpty())
            {
                list.Add((types[i], typeList.ToArray()));
                typeList.Clear();
            }
        }
        return list;
    }

    private static IEnumerable<(object? Key, Type Type, Type? ServiceType)> FindServices(Assembly assembly) // ServiceType is interface
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

    public void ExtractService<TService>(object serviceInstance, object? key = null)
        => ExtractService(serviceType: typeof(TService), serviceInstance: serviceInstance, key);

    public void ExtractService(Type serviceType, object serviceInstance, object? key = null)
    {
        var instanceType = serviceInstance.GetType();
        if (!serviceType.IsAssignableFrom(instanceType)) throw new InvalidOperationException($"Instance Type {instanceType} not assigned from Service Type {serviceType}.");
        if (!_services.TryAdd((key, serviceType), serviceInstance)) throw new InvalidOperationException($"Service Type {serviceType} with Key {key} already exist.");
    }

    public void ExtractDecorator<TService, TDecorator>(object? serviceKey = null)
    {
        var decorator = typeof(TDecorator);
        var service = typeof(TService);
        var ctor = decorator.GetConstructor([service]) ?? throw new InvalidOperationException($"Decorator Type {decorator} must have constructor accepting {service}");
        ExtractDecorator<TService>(s => (TService)ctor.Invoke([s])!, serviceKey);
    }

    public void ExtractDecorator<TService>(Func<object, object> decoratorFactory, object? serviceKey = null)
    => ExtractDecorator(typeof(TService), decoratorFactory, serviceKey);

    public void ExtractDecorator(Type serviceType, Func<object, object> decoratorFactory, object? serviceKey = null)
    {
        ArgumentNullException.ThrowIfNull(decoratorFactory);
        var key = (serviceKey, serviceType);
        _decorators.TryGetValue(key, out var list);
        _decorators[key] = list ??= [];
        list.Add(decoratorFactory);
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
