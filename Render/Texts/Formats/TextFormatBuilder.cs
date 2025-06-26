using System;

namespace Hopeful.Render.Texts.Formats;

public sealed class TextFormatBuilder(TextFormat format)
{
    public static TextFormatBuilder Setup()
        => new(new());

    public TextFormatBuilder WithSetup(Action<TextFormat, TextSegment> action)
    {
        format.SetupAction = action;
        return this;
    }

    public TextFormatBuilder WithUpdate(Action<TextFormat, TextSegment, float> action)
    {
        format.UpdateAction = action;
        return this;
    }

    public TextFormatBuilder WithParameter(string name, in IComparable defaultValue)
    {
        format.Parameters ??= [];
        format.Parameters[name] = new TextFormatParameter(defaultValue);
        return this;
    }

    public TextFormat Save() => format;
}