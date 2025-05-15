using System;
using JetBrains.Annotations;

namespace Hopeful.Injection;

[MeansImplicitUse, AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class InjectAttribute : Attribute
{
    public object? Key { get; }
    public bool? Optional { get; set; }

    public InjectAttribute() { }
    public InjectAttribute(object key) => Key = key;
}
