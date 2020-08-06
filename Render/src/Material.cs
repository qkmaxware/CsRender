using System;
using System.Drawing;
using System.Linq;
using Qkmaxware.Geometry;

namespace Qkmaxware.Rendering {

/// <summary>
/// Base class for all material rendering
/// </summary>
public class Material {
    /// <summary>
    /// Two sided materials will not be subject to occlusion culling
    /// </summary>
    public bool TwoSided = false;
    /// <summary>
    /// Shader to apply to each vertex
    /// </summary>
    /// <param name="variables">shading variables</param>
    /// <returns>vertex colour</returns>
    public virtual Color Vert(ShaderVariables variables) { return Color.Transparent; }
    /// <summary>
    /// Shader to apply to each face's edges
    /// </summary>
    /// <param name="variables">shading variables</param>
    /// <returns>edge colour</returns>
    public virtual Color Edge(ShaderVariables variables) { return Fragment(variables); }
    /// <summary>
    /// Shader to apply to each face's surface
    /// </summary>
    /// <param name="variables">shading variables</param>
    /// <returns>color of point on face</returns>
    public virtual Color Fragment(ShaderVariables variables) { return Color.Transparent; }
}

/// <summary>
/// Material for wireframe objects
/// </summary>
public class Wireframe : Material {
    public Color Colour = Color.White;

    /// <summary>
    /// New wireframe material with colour
    /// </summary>
    /// <param name="colour">material colour</param>
    public Wireframe(Color colour) {
        this.Colour = colour;
        this.TwoSided = true;
    }
    /// <summary>
    /// Shader to apply to each face's edges
    /// </summary>
    /// <param name="variables">shading variables</param>
    /// <returns>edge colour</returns>
    public override Color Edge(ShaderVariables variables) {
        return this.Colour;
    }
}

/// <summary>
/// Solid colour material with no shading
/// </summary>
public class UnlitColour : Material {
    public Color Colour = Color.White;
    /// <summary>
    /// New unlit colour material
    /// </summary>
    /// <param name="colour">material colour</param>
    public UnlitColour(Color colour) {
        this.Colour = colour;
    }
    /// <summary>
    /// Shader to apply to each face's edges
    /// </summary>
    /// <param name="variables">shading variables</param>
    /// <returns>edge colour</returns>
    public override Color Edge(ShaderVariables variables) {
        return Colour;
    }
    /// <summary>
    /// Shader to apply to each face's surface
    /// </summary>
    /// <param name="variables">shading variables</param>
    /// <returns>color of point on face</returns>
    public override Color Fragment(ShaderVariables variables) { 
        return Colour;
    }
}

/// <summary>
/// Texture wrapping mode
/// </summary>
public enum TextureWrapMode {
    Clamp,
    Repeat,
}

/// <summary>
/// Base class for a material with a single texture
/// </summary>
public class TexturedMaterial : Material {
    public Color[,] Texture;
    public TextureWrapMode WrapMode = TextureWrapMode.Clamp;

    protected int TextureHeight => Texture.GetLength(0);
    protected int TextureWidth => Texture.GetLength(1);

    /// <summary>
    /// Create a textured material with the given texture and sampling mode
    /// </summary>
    /// <param name="texture">texture</param>
    /// <param name="wrap">texturing sampling mode</param>
    public TexturedMaterial(Color[,] texture, TextureWrapMode wrap = TextureWrapMode.Clamp) {
        this.Texture = texture;
        this.WrapMode = wrap;
    }

    private int Wrap(int x, int x_min, int x_max) {
        return (((x - x_min) % (x_max - x_min)) + (x_max - x_min)) % (x_max - x_min) + x_min;
    }
    private int Clamp(int x, int x_min, int x_max) {
        return x < x_min ? x_min : (x > x_max ? x_max : x);
    }

    /// <summary>
    /// Sample a colour on the given texture at the provided UV coordinates
    /// </summary>
    /// <param name="uv">UV coordinates between 0 and 1</param>
    /// <returns>texture colour</returns>
    protected Color ColourSample(Vec2 uv) {
        var uvx = (int)(uv.X * TextureWidth);
        var uvy = (int)(uv.Y * TextureHeight);

        switch (WrapMode) {
            case TextureWrapMode.Repeat:
                uvx = Wrap(uvx, 0, TextureWidth - 1);
                uvy = Wrap(uvy, 0, TextureHeight - 1);
                break;
            case TextureWrapMode.Clamp:
            default:
                uvx = Clamp(uvx, 0, TextureWidth - 1);
                uvy = Clamp(uvy, 0, TextureHeight - 1);
                break;
        }

        return Texture[uvy, uvx];
    }
}

/// <summary>
/// Unlit textured material
/// </summary>
public class UnlitTexture : TexturedMaterial {
    public UnlitTexture(Color[,] texture, TextureWrapMode wrap = TextureWrapMode.Clamp) : base(texture, wrap) {}
    
    /// <summary>
    /// Shader to apply to each face's edges
    /// </summary>
    /// <param name="variables">shading variables</param>
    /// <returns>edge colour</returns>
    public override Color Edge(ShaderVariables variables) {
        return Fragment(variables);
    }
    /// <summary>
    /// Shader to apply to each face's surface
    /// </summary>
    /// <param name="variables">shading variables</param>
    /// <returns>color of point on face</returns>
    public override Color Fragment(ShaderVariables variables) {
        return ColourSample(variables.UVCoordinates);
    }
}

/// <summary>
/// Diffuse lit solid colour material
/// </summary>
public class DiffuseColour : Material {
    public Color Colour = Color.White;
    public double Albedo = 1;

    /// <summary>
    /// Create a new diffuse lit solid colour material
    /// </summary>
    /// <param name="colour">material colour</param>
    public DiffuseColour(Color colour) {
        this.Colour = colour;
    }

    private Color Darken(Color colour, double shade) {
        double r = shade * colour.R;
        double g = shade * colour.G;
        double b = shade * colour.B;

        return Color.FromArgb(colour.A, (int)r % 255, (int)g % 255, (int)b % 255);
    }

    private Color Mix(Color a, Color b) {
        return Color.FromArgb(
            a.A,
            (a.R * b.R)/255,
            (a.G * b.G)/255,
            (a.B * b.B)/255
        );
    }
    /// <summary>
    /// Shader to apply to each face's surface
    /// </summary>
    /// <param name="variables">shading variables</param>
    /// <returns>color of point on face</returns>
    public override Color Fragment(ShaderVariables variables) {
        var surfColour = Colour;
        Vec3 N = variables.WorldNormal;
        var tint = variables.LightSources.Select((light) => {
            var lightColour = light.Tint;
            Vec3 L = light.Direction(variables.WorldPosition, variables.WorldNormal);
            double Lintensity = light.Intensity(variables.WorldPosition, variables.WorldNormal);
            double diffuse_surface_shade = (Albedo / Math.PI) * Lintensity * Math.Max(Vec3.Dot(L, N), 0);

            return Darken(lightColour, diffuse_surface_shade);
        }).Aggregate((a,b) => Mix(a, b));
        return Mix(surfColour, tint);
    }
}

/// <summary>
/// Diffuse lit textured material
/// </summary>
public class DiffuseTexture : TexturedMaterial {
    public Color Colour = Color.White;
    public double Albedo = 1;

    /// <summary>
    /// Create a diffuse shaded material with the given texture
    /// </summary>
    /// <param name="texture">texture</param>
    /// <param name="wrap">texture sampling mode</param>
    public DiffuseTexture(Color[,] texture, TextureWrapMode wrap = TextureWrapMode.Clamp) : base(texture, wrap) {}

    private Color Darken(Color colour, double shade) {
        double r = shade * colour.R;
        double g = shade * colour.G;
        double b = shade * colour.B;

        return Color.FromArgb(colour.A, (int)r % 255, (int)g % 255, (int)b % 255);
    }

    private Color Mix(Color a, Color b) {
        return Color.FromArgb(
            a.A,
            (a.R * b.R)/255,
            (a.G * b.G)/255,
            (a.B * b.B)/255
        );
    }
    /// <summary>
    /// Shader to apply to each face's surface
    /// </summary>
    /// <param name="variables">shading variables</param>
    /// <returns>color of point on face</returns>
    public override Color Fragment(ShaderVariables variables) {
        var surfColour = ColourSample(variables.UVCoordinates);
        Vec3 N = variables.WorldNormal;
        var tint = variables.LightSources.Select((light) => {
            var lightColour = light.Tint;
            Vec3 L = light.Direction(variables.WorldPosition, variables.WorldNormal);
            double Lintensity = light.Intensity(variables.WorldPosition, variables.WorldNormal);
            double diffuse_surface_shade = (Albedo / Math.PI) * Lintensity * Math.Max(Vec3.Dot(L, N), 0);

            return Darken(lightColour, diffuse_surface_shade);
        }).Aggregate((a,b) => Mix(a, b));
        return Mix(surfColour, tint);
    }
}

}