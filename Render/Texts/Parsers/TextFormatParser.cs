using System.Collections.Generic;
using System.Text.RegularExpressions;
using Color = SixLabors.ImageSharp.Color;

namespace Hopeful.Render.Texts;

[Service(typeof(ITextFormatParser))]
public sealed class TextFormatParser : ITextFormatParser
{
    private readonly Regex _tagRegex = new(@"\{(?:/)?([^:}]+)(?::([^}]+))?\}", RegexOptions.Compiled);
    private readonly Regex _rgbColorTagRegex = new(@"(\d+),(\d+),(\d+)", RegexOptions.Compiled);

    private readonly Dictionary<string, Color> _predefinedColors = new()
    {
        { "white", Color.White },
        { "black", Color.Black },
        { "red", Color.Red },
        { "green", Color.Green },
        { "blue", Color.Blue },
        { "yellow", Color.Yellow },
        { "purple", Color.Purple },
        { "cyan", Color.Cyan },
        { "gray", Color.Gray },
        { "orange", Color.Orange }
    };

    [Inject]
    private readonly TextFormatStore _formats = null!;

    public Color ParseColor(string text)
    {
        if (_predefinedColors.TryGetValue(text.ToLower(), out var color)) return color;

        var rgbMatch = _rgbColorTagRegex.Match(text);

        if (!rgbMatch.Success) return Color.White;

        var r = byte.Parse(rgbMatch.Groups[1].Value);
        var g = byte.Parse(rgbMatch.Groups[2].Value);
        var b = byte.Parse(rgbMatch.Groups[3].Value);
        return Color.FromRgb(r, g, b);
    }

    public TextSegment[] ParseText(string text)
    {
        var readySegments = new List<TextSegment>();
        var stack = new Stack<FormatState>();
        var currentSegment = new TextSegment();
        var currentFormats = new List<TextFormat>();
        var currentPosition = 0;

        foreach (Match match in _tagRegex.Matches(text))
        {
            // Add text before tag
            if (match.Index > currentPosition)
            {
                var plainText = text[currentPosition..match.Index];
                readySegments.Add(new()
                {
                    Text = plainText,
                    FontName = currentSegment.FontName,
                    Color = currentSegment.Color,
                    Position = currentSegment.Position,
                    Formats = currentFormats

                });
            }

            var tag = match.Groups[1].Value;
            var param = match.Groups[2].Value;
            var tagLower = tag.ToLower();
            var isClosing = match.Value.StartsWith("{/");

            if (!isClosing)
            {
                // Save current state before modification
                stack.Push(new((TextSegment)currentSegment.Clone(), currentFormats));

                // Color parsing
                currentSegment.Color = ParseColor(tagLower);

                // Effect parsing
                if (_formats.TryGetTextFormat(tagLower, out var format))
                {
                    if (format.Parameters != null && !string.IsNullOrEmpty(param))
                    {
                        var parameters = param.Split(',');
                        for (var i = 0; i < parameters.Length && i < format.Parameters.Count; i++)
                        {
                            Debug.WriteLine($"Format {tagLower} with param number {i} is {parameters[i]}");
                            var parameter = format.Parameters[parameters[i]]; // Incorrect getting parameter instance
                            parameter.Value = parameter.Parser.Invoke(parameters[i]);
                        }
                        currentFormats.Add(format); // TODO: FIX THIS - formats without parameters cannot be added
                    }
                }
            }
            else if (stack.Count > 0)
            {
                // Restore previous state
                var previousState = stack.Pop();
                currentSegment = previousState.Segment;
                currentFormats = previousState.Formats;
            }

            currentPosition = match.Index + match.Length;
        }

        // Add remaining text
        if (currentPosition < text.Length)
        {
            var remainingText = text[currentPosition..];
            readySegments.Add(new()
            {
                Text = remainingText,
                FontName = currentSegment.FontName!,
                Color = currentSegment.Color,
                Position = currentSegment.Position,
                Formats = currentFormats
            });
        }

        return readySegments.ToArray();
    }

    private record FormatState(TextSegment Segment, List<TextFormat> Formats);
}
