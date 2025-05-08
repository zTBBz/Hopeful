using System;

namespace Hopeful.Injection;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ServiceAttribute : Attribute
{
    public object? Key { get; }

    public ServiceAttribute() { }
    public ServiceAttribute(object key) => Key = key;
}
