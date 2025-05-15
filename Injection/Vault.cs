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
    private readonly Dictionary<(object?, Type), object> _services = []; // <(ServiceKey, ServiceType), ServiceInstance>

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
                object? injectInstance = Resolve(injection.TypeToken, injection.IsOptional);
                injection.Resolver.Resolve(injection, client, injectInstance);
            }
        }
    }

    private object? Resolve(Type type, bool isOptional)
    {
        object? injection = null;

        if (type.TryGetCustomAttribute<ServiceAttribute>(out var service)) // services supports interfaces. Need example go to AssetManager line 81.
            return InjectService(type, service.Key);

        // If the injection is required, attempt to create it
        if (!isOptional)
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

    [Pure]
    private static List<(Type type, InjectionInfo[] injections)> FindInjections(Assembly assembly)
    {
        var types = assembly.GetTypes();
        const BindingFlags anyFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        List<(Type, InjectionInfo[])> list = [];
        List<InjectionInfo> typeList = [];

        for (int i = 0; i < types.Length; i++)
        {
            foreach (FieldInfo field in types[i].GetFields(anyFlags))
                if (field.TryGetCustomAttribute(out InjectAttribute? attr))
                {
                    IInjectionResolver resolver = new FieldInjectionResolver(field);
                    bool isOptional = attr.Optional ?? field.GetNullability() == Nullability.Nullable;
                    typeList.Add(new InjectionInfo(field.FieldType, resolver, isOptional));
                }

            foreach (PropertyInfo property in types[i].GetProperties(anyFlags))
                if (property.TryGetCustomAttribute(out InjectAttribute? attr))
                {
                    IInjectionResolver resolver = new PropertyInjectionResolver(property);
                    bool isOptional = attr.Optional ?? property.GetNullability() == Nullability.Nullable;
                    typeList.Add(new InjectionInfo(property.PropertyType, resolver, isOptional));
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
    public object InjectService(Type type, object? key = null)
    {
        if (_services.TryGetValue((key, type), out var service))
            return service;
        throw new InvalidOperationException($"Service Type {type} is not exist.");
    }

    [Pure]
    public void ExtractService(Type type, Type instanceType, object? key = null)
        => _services.TryAdd((key, type), Activator.CreateInstance(instanceType)!);

    [Pure]
    public void ExtractService(Type type, object instance, object? key = null)
    {
        var instanceType = instance.GetType();
        if (instanceType != type) throw new InvalidOperationException($"Instance Type {instanceType} not equal Service Type {type}.");
        _services.TryAdd((key, type), instance);
    }

    [Pure]
    public IEnumerable<KeyValuePair<Type, InjectionInfo[]>> EnumerateInjections()
    {
        var current = this;
        var seen = new HashSet<Type>();

        while (current != null)
        {
            foreach (var kvp in current._injections)
            {
                if (seen.Add(kvp.Key)) // избегаем перекрытий
                    yield return kvp;
            }

            current = current._parent;
        }
    }

    [Pure]
    public IEnumerable<KeyValuePair<(object?, Type), object>> EnumerateServices()
    {
        var current = this;
        var seen = new HashSet<(object?, Type)>();

        while (current != null)
        {
            foreach (var kvp in current._services)
            {
                if (seen.Add(kvp.Key))
                    yield return kvp;
            }

            current = current._parent;
        }
    }

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
