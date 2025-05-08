using System.Reflection;

namespace Hopeful.Injection.Resolvers;

public sealed class FieldInjectionResolver(FieldInfo field) : IInjectionResolver
{
    public FieldInfo Field { get; } = field;
    public void Resolve(InjectionInfo info, object client, object? injection)
        => Field.SetValue(client, injection);
}
