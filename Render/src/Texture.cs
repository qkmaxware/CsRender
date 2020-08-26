using System.Drawing;

namespace Qkmaxware.Rendering {

/// <summary>
/// Texture wrapping mode
/// </summary>
public enum TextureWrapMode {
    Clamp,
    Repeat,
}

/// <summary>
/// Base class for 2D textures
/// </summary>
public abstract class Texture2D {
    /// <summary>
    /// wrap mode for pixel sampling
    /// </summary>
    public TextureWrapMode WrapMode = TextureWrapMode.Clamp;

    /// <summary>
    /// Texture height
    /// </summary>
    public abstract int Height {get;}
    /// <summary>
    /// Texture width
    /// </summary>
    public abstract int Width {get;}

    /// <summary>
    /// Sample the given texture pixel
    /// </summary>
    public abstract Color this[int x, int y] {get;}
}

/// <summary>
/// 2D texture defined by pixel colours
/// </summary>
public class PixelTexture : Texture2D {
    private Color[,] pixels;

    public override int Height => pixels.GetLength(0);
    public override int Width => pixels.GetLength(1);

    public PixelTexture (Color[,] pixels) {
        this.pixels = pixels;
    }

    private int Wrap(int x, int x_min, int x_max) {
        return (((x - x_min) % (x_max - x_min)) + (x_max - x_min)) % (x_max - x_min) + x_min;
    }
    private int Clamp(int x, int x_min, int x_max) {
        return x < x_min ? x_min : (x > x_max ? x_max : x);
    }

    public override Color this [int x, int y] {
        get {
            switch (WrapMode) {
                case TextureWrapMode.Repeat:
                    x = Wrap(x, 0, this.Width - 1);
                    y = Wrap(y, 0, this.Height - 1);
                    break;
                case TextureWrapMode.Clamp:
                default:
                    x = Clamp(x, 0, this.Width - 1);
                    y = Clamp(y, 0, this.Height - 1);
                    break;
            }

            return pixels[y, x];
        }
    }
}

}