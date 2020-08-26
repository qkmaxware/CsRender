using System;
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
/// Skybox with a solid user defined colour
/// </summary>
public class SolidSkybox : Skybox {
    public Color Colour {get; private set;}
    public SolidSkybox() {
        this.Colour = Color.Black;
    }
    public SolidSkybox(Color colour) {
        this.Colour = colour;
    }
    public override Color GetPixel(BaseCamera camera, int x, int y) {
        return this.Colour;
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
        // Use angles from polar coorinates
        var point = camera.ScreenToWorldPoint(new Vec2(x,y));
        var z = point.Z;
        var length = point.Length;
        if (length == 0) {
            return top;
        } else {
            var vangle = Math.Acos(z / length);
            var interpolation_factor = vangle / Math.PI; // angles are 0deg to 180deg (0 to Pi radians)
            return Blend(top, bottom, interpolation_factor);
        }
    }
}

/// <summary>
/// Skybox with a wrapped 3d texture
/// </summary>
public class TexturedSkybox : Skybox {
    public Texture2D Texture {get; private set;}

    public TexturedSkybox(Texture2D texture) {
        this.Texture = texture;
    }
    
    public override Color GetPixel(BaseCamera camera, int x, int y) {
        var point = camera.ScreenToWorldPoint(new Vec2(x,y));
        var z = point.Z;
        var length = point.Length;
        if (length == 0)
            return Color.White;
        
        // Vertical angle (0 - 180deg)
        var vangle = Math.Acos(z / length);
        var v_interpolation_factor = vangle / Math.PI;
        var uvy = (int)(Texture.Height * v_interpolation_factor);

        // Horizontal angle (0 - 360deg)
        var hangle = Math.Asin( point.Y / (length * Math.Sin(vangle)) ) + (Math.PI * 0.5);
        var h_interpolation_factor = hangle / (2 * Math.PI);
        var uvx = (int)(Texture.Width * h_interpolation_factor);

        return this.Texture[uvy, uvx];
    }
}

}