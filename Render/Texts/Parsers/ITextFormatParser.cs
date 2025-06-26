using SixLabors.ImageSharp;

namespace Hopeful.Render.Texts;

public interface ITextFormatParser
{
    public Color ParseColor(string text);
    public TextSegment[] ParseText(string text);
}