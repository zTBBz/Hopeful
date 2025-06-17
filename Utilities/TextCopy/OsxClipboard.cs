using System.Runtime.InteropServices;

namespace Hopeful.Utilities.TextCopy;

internal static class OsxClipboard
{
    private static readonly nint nsString = objc_getClass("NSString");
    private static readonly nint nsPasteboard = objc_getClass("NSPasteboard");
    private static readonly nint nsStringPboardType;
    private static readonly nint utfTextType;
    private static readonly nint generalPasteboard;

    static OsxClipboard()
    {
        utfTextType = objc_msgSend(objc_msgSend(nsString, sel_registerName("alloc")), sel_registerName("initWithUTF8String:"), "public.utf8-plain-text");
        nsStringPboardType = objc_msgSend(objc_msgSend(nsString, sel_registerName("alloc")), sel_registerName("initWithUTF8String:"), "NSStringPboardType");
        generalPasteboard = objc_msgSend(nsPasteboard, sel_registerName("generalPasteboard"));
    }

    public static string GetText()
    {
        var ptr = objc_msgSend(generalPasteboard, sel_registerName("stringForType:"), nsStringPboardType);
        var charArray = objc_msgSend(ptr, sel_registerName("UTF8String"));
        return Marshal.PtrToStringAnsi(charArray) ?? string.Empty;
    }

    public static void SetText(string text)
    {
        nint str = nint.Zero;
        try
        {
            str = objc_msgSend(objc_msgSend(nsString, sel_registerName("alloc")), sel_registerName("initWithUTF8String:"), text);
            objc_msgSend(generalPasteboard, sel_registerName("clearContents"));
            objc_msgSend(generalPasteboard, sel_registerName("setString:forType:"), str, utfTextType);
        }
        finally
        {
            if (str != nint.Zero)
                objc_msgSend(str, sel_registerName("release"));
        }
    }

    [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
    static extern nint objc_getClass(string className);

    [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
    static extern nint objc_msgSend(nint receiver, nint selector);

    [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
    static extern nint objc_msgSend(nint receiver, nint selector, string arg1);

    [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
    static extern nint objc_msgSend(nint receiver, nint selector, nint arg1);

    [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
    static extern nint objc_msgSend(nint receiver, nint selector, nint arg1, nint arg2);

    [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
    static extern nint sel_registerName(string selectorName);
}