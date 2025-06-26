using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Hopeful.Render.Texts;

public sealed class TextFormat
{
    public Action<TextFormat, TextSegment>? SetupAction = null;
    public Action<TextFormat, TextSegment, float>? UpdateAction = null;
    public Dictionary<string, TextFormatParameter>? Parameters = null;
    public bool IsSetuped = false;
    // TODO: MAYBE add support for values (use-case is TypingFormat, where need save Progress)

    public bool TryGetParameter(string name, [NotNullWhen(true)] out TextFormatParameter? parameter)
    {
        parameter = null;
        Parameters?.TryGetValue(name, out parameter);
        return parameter != null;
    }

    // TODO: Look after TEST for removing TextSegment segment param. If segment not changes, just cache TextSegment in TextFormat
    public TValue GetParameterValue<TValue>(string name, TextSegment segment) where TValue : IComparable
    {
        ArgumentNullException.ThrowIfNullOrEmpty(name);
        if (!TryGetParameter(name, out var parameter)) throw new KeyNotFoundException(name);
        parameter.Value = parameter.Parser.Invoke(segment.Text); // Parse to get actual value
        return parameter.GetValue<TValue>();
    }
}
