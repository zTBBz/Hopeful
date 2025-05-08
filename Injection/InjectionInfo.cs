using Hopeful.Injection.Resolvers;
using System;

namespace Hopeful.Injection;

public readonly struct InjectionInfo(Type typeToken, IInjectionResolver resolver, bool isOptional)
{
    public Type TypeToken { get; } = typeToken;
    public IInjectionResolver Resolver { get; } = resolver;
    public bool IsOptional { get; } = isOptional;
}
