using System.Runtime.InteropServices;
using System;

namespace Hopeful.Utilities.TextCopy;

internal static class Platform
{
    private static bool initialized = false;

    public static OSType OS
    {
        get
        {
            if (!initialized)
            {
                field = Initialize();
                initialized = true;
            }
            return field;
        }
    }


    [DllImport("libc")]
    static extern int uname(nint buf);

    private static OSType Initialize()
    {
        PlatformID pid = Environment.OSVersion.Platform;
        switch (pid)
        {
            case PlatformID.Win32NT or PlatformID.Win32S or PlatformID.Win32Windows or PlatformID.WinCE:
                return OSType.Windows;
            case PlatformID.MacOSX:
                return OSType.MacOSX;
            case PlatformID.Unix:
                // Mac can return a value of Unix sometimes, We need to double check it.
                nint buf = nint.Zero;
                try
                {
                    buf = Marshal.AllocHGlobal(8192);
                    if (uname(buf) == 0)
                        if (Marshal.PtrToStringAnsi(buf) == "Darwin")
                            return OSType.MacOSX;
                }
                catch
                {
                }
                finally
                {
                    if (buf != nint.Zero)
                        Marshal.FreeHGlobal(buf);
                }

                return OSType.Linux;
            default:
                return OSType.Unknown;
        }
    }

    internal enum OSType
    {
        Windows,
        Linux,
        MacOSX,
        Unknown
    }
}
