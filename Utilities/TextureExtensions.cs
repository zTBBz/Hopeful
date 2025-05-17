using DirectXTexNet;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.InteropServices;

namespace Hopeful.Utilities;

public static class TextureExtensions
{
    public static Texture2D WithData<T>(this Texture2D texture, T[] data) where T : struct
    {
        texture.SetData(data);
        return texture;
    }

    public static void SaveToDDS(this Texture2D texture, string outputPath, DXGI_FORMAT format)
    {
        ArgumentNullException.ThrowIfNull(texture, nameof(texture));
        ArgumentNullException.ThrowIfNullOrEmpty(outputPath, nameof(outputPath));

        // Copy data from Texture2D
        int width = texture.Width;
        int height = texture.Height;
        byte[] pixelData = new byte[width * height * 4]; // RGBA
        texture.GetData(pixelData); // Waiting for allow use non-main thead for GetData(). Or create own MonoGame fork.
        // https://github.com/labnation/MonoGame/blob/d270be3e800a3955886e817cdd06133743a7e043/MonoGame.Framework/Graphics/Texture2D.OpenGL.cs#L213

        // Create blank RGBA and copy Texture2D data to him.
        using var image = TexHelper.Instance.Initialize2D(DXGI_FORMAT.R8G8B8A8_UNORM, width, height, 1, 0, CP_FLAGS.NONE);
        IntPtr imageData = image.GetImage(0).Pixels;
        Marshal.Copy(pixelData, 0, imageData, pixelData.Length);

        // Compress to choiced format
        TEX_COMPRESS_FLAGS compress = TEX_COMPRESS_FLAGS.DEFAULT;
        image.Compress(1, format, compress, 0.5f);

        // Save as DDS
        image.SaveToDDSFile(1, DDS_FLAGS.NONE, outputPath);
    }
}
