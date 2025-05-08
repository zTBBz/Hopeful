namespace Hopeful.Injection.Resolvers;

public interface IInjectionResolver
{
    void Resolve(InjectionInfo info, object client, object? injection);
}
