using System;

namespace Hopeful.Injection;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class DecoratorAttribute : Attribute
{
    public object? Key { get; }
    public Type Type { get; }

    public DecoratorAttribute(Type type) => Type = type;
    public DecoratorAttribute(object key, Type type)
    {
        Key = key;
        Type = type;
    }
}
