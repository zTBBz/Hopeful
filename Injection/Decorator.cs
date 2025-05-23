namespace Hopeful.Injection;

public abstract class Decorator<T>(T inner)
{
    protected readonly T _inner = inner;
}
