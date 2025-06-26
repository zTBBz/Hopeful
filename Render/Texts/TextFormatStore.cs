using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Hopeful.Render.Texts;

[Service]
public sealed class TextFormatStore
{
    private readonly Dictionary<string, TextFormat> _formats = [];

    public void AddTextFormat(string name, in TextFormat format)
        => _formats[name] = format;

    public bool TryGetTextFormat(string name, [NotNullWhen(true)] out TextFormat? format)
        => _formats.TryGetValue(name, out format);
}
