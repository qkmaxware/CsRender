using System.Drawing;
using Qkmaxware.Geometry;

namespace Qkmaxware.Rendering {

/// <summary>
/// Base class for rendering a skybox 
/// </summary>
public class Skybox {
    /// <summary>
    /// Get the skybox colour for the given camera 
    /// </summary>
    /// <param name="camera">camera</param>
    /// <param name="x">pixel x</param>
    /// <param name="y">pixel y</param>
    /// <returns>skybox color for camera pixel</returns>
    public virtual Color GetPixel(BaseCamera camera, int x, int y) {
        return Color.Black;
    }
}

/// <summary>
/// Skybox with a vertical gradient
/// </summary>
public class GradientSkybox : Skybox {

    private Color bottom;
    private Color top;

    /// <summary>
    /// Create a gradient skybox with two colours
    /// </summary>
    /// <param name="first">colour at the bottom</param>
    /// <param name="second">colour at the top</param>
    public GradientSkybox(Color first, Color second) {
        this.bottom = first;
        this.top = second;
    }

    private static Color Blend(Color backColor, Color color, double amount) {
        byte r = (byte) ((color.R * amount) + backColor.R * (1 - amount));
        byte g = (byte) ((color.G * amount) + backColor.G * (1 - amount));
        byte b = (byte) ((color.B * amount) + backColor.B * (1 - amount));
        return Color.FromArgb(r, g, b);
    }

    public override Color GetPixel(BaseCamera camera, int x, int y) {
        var interpolation_factor = camera.ScreenToWorldPoint(new Vec2(x,y)).Normalized.Z;
        return Blend(bottom, top, interpolation_factor);
    }
}

}