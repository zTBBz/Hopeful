using System;
using JetBrains.Annotations;

namespace Hopeful.Injection;

[MeansImplicitUse, AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class InjectAttribute : Attribute
{
    public object? Key { get; }
    public bool? Optional { get; set; }
    public Type? DecoratorTargetType { get; }

    public InjectAttribute() { }
    public InjectAttribute(object key) => Key = key;
    public InjectAttribute(Type decoratorTargetType) => DecoratorTargetType = decoratorTargetType;

    public InjectAttribute(object key, Type decoratorTargetType)
    {
        Key = key;
        DecoratorTargetType = decoratorTargetType;
    }
}
