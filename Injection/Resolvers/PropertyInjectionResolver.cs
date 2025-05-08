using System;
using System.Reflection;

namespace Hopeful.Injection.Resolvers;

public sealed class PropertyInjectionResolver : IInjectionResolver
{
    public PropertyInfo Property { get; }
    public PropertyInjectionResolver(PropertyInfo property)
    {
        if (!property.CanWrite) throw new ArgumentException("The specified property is set-only!");
        Property = property;
    }

    public void Resolve(InjectionInfo info, object client, object? injection)
        => Property.SetValue(client, injection);
}
