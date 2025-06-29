using Hopeful.Injection.Resolvers;
using System;

namespace Hopeful.Injection;

public readonly struct InjectionInfo(Type typeToken, IInjectionResolver resolver, bool isOptional, object? key = null, Type? decoratorTargetType = null)
{
    public readonly object? Key = key;
    public Type TypeToken { get; } = typeToken;
    public Type? DecoratorTargetType { get; } = decoratorTargetType;
    public IInjectionResolver Resolver { get; } = resolver;
    public bool IsOptional { get; } = isOptional;
}
