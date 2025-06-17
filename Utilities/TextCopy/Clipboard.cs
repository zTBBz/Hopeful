using System;
using static Hopeful.Utilities.TextCopy.Platform;

namespace Hopeful.Utilities.TextCopy;

public static class Clipboard
{
    public static bool UseLocalClipboard = false;
    private static string _localTextClipboard = string.Empty;
    private static readonly Action<string> _setAction = CreateSet();
    private static readonly Func<string> _getFunc = CreateGet();

    public static void SetText(string text)
    {
        ArgumentNullException.ThrowIfNull(text, nameof(text));

        if (UseLocalClipboard)
        {
            _localTextClipboard = text;
            return;
        }

        _setAction(text);
    }

    public static string GetText()
        => UseLocalClipboard ? _localTextClipboard : _getFunc();

    static Action<string> CreateSet()
        => OS switch
        {
            OSType.Windows => WindowsClipboard.SetText,
            OSType.Linux => LinuxClipboard.SetText,
            OSType.MacOSX => OsxClipboard.SetText,
            OSType.Unknown => throw new NotSupportedException(),
            _ => throw new NotSupportedException()
        };

    static Func<string> CreateGet()
        => OS switch
        {
            OSType.Windows => WindowsClipboard.GetText,
            OSType.Linux => LinuxClipboard.GetText,
            OSType.MacOSX => OsxClipboard.GetText,
            OSType.Unknown => throw new NotSupportedException(),
            _ => throw new NotSupportedException()
        };
}
