using System;

namespace Hopeful.Render.Texts;

public sealed class TextFormatParameter(IComparable defaultValue, Func<string, IComparable> parser)
{
    public IComparable? Value;
    public readonly IComparable DefaultValue = defaultValue;

    public readonly Func<string, IComparable> Parser = parser;

    public TValue GetValue<TValue>() where TValue : IComparable
    {
        if (Value == null) return (TValue)DefaultValue;
        return (TValue)Value;
    }
}
