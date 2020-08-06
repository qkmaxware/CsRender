using System;
using System.Linq;
using System.Drawing;
using Qkmaxware.Geometry;
using System.Collections.Generic;

namespace Qkmaxware.Rendering {

/// <summary>
/// Base class for a camera that can render a scene
/// </summary>
public abstract class BaseCamera : SceneNode {

    /// <summary>
    /// Frame size
    /// </summary>
    /// <value></value>
    public Size Size {get; private set;}
    /// <summary>
    /// Rendered pixels
    /// </summary>
    public Color[,] PixelBuffer {get; private set;}
    /// <summary>
    /// Pixel depth values
    /// </summary>
    public double[,] DepthBuffer {get; private set;}
    /// <summary>
    /// Background skybox
    /// </summary>  
    public Skybox Skybox {get; private set;} = new Skybox();

    protected readonly double focallength = 1;
    protected double nearClipDistance = 0.1;
    protected double farClipDistance = 1000;

    /// <summary>
    /// Create a new camera with the given image size
    /// </summary>
    /// <param name="size">rendered image size</param>
    public BaseCamera(Size size) {
        this.Size = size;
        this.PixelBuffer = new Color[size.Height,size.Width];
        this.DepthBuffer = new double[size.Height,size.Width];
        Dirty();
    }

    /// <summary>
    /// Set the near and far clipping planes
    /// </summary>
    /// <param name="near">near clip plane distance</param>
    /// <param name="far">far clip plane distance</param>
    public void SetClippingDistance(double near, double far) {
        this.nearClipDistance = Math.Min(near, far);
        this.farClipDistance = Math.Max(near, far);
        Dirty();
    }

    /// <summary>
    /// Event when camera properties change
    /// </summary>
    protected abstract void Dirty();
    /// <summary>
    /// Convert from a world position to a screen position
    /// </summary>
    /// <param name="world">world position</param>
    /// <returns>screen position</returns>
    public abstract Vec3 WorldToScreenPoint(Vec3 world);
    /// <summary>
    /// Convert from a screen position to a world position
    /// </summary>
    /// <param name="screen">screen position</param>
    /// <returns>world position</returns>
    public abstract Vec3 ScreenToWorldPoint(Vec2 screen);

    private void ClearPixels() {
        for (int i = 0; i < PixelBuffer.GetLength(0); i++)
            for (int j = 0; j < PixelBuffer.GetLength(1); j++)
                PixelBuffer[i, j] = Skybox.GetPixel(this, j, i);
    }

    private void ClearDepth() {
        for (int i = 0; i < DepthBuffer.GetLength(0); i++)
            for (int j = 0; j < DepthBuffer.GetLength(1); j++)
                DepthBuffer[i, j] = farClipDistance;
    }

    /// <summary>
    /// Render the given scene to the pixel buffer
    /// </summary>
    /// <param name="scene">scene to render</param>
    public void Render(Scene scene) {
        // Clean data
        ClearPixels();
        ClearDepth();

        // Set constant shader properties
        var vars = new ShaderVariables();
        vars.WorldCameraPosition = this.Position;
        vars.LightSources = scene.OfType<LightSource>().ToList().AsReadOnly();

        // Loop over all models
        foreach (var renderable in scene.OfType<Renderable>()) {
            vars.ModelToWorld = renderable.LocalToWorldMatrix;
            if (renderable.Mesh != null && renderable.Material != null) {
                Render(ref vars, renderable.Mesh, renderable.UVs, renderable.Material);
            }
        }
    }

    private void Render(ref ShaderVariables shader, IEnumerable<Triangle> tris, IUvMap? uvs, Material material) {
        var backwards = this.Backward;
        foreach (var tri in tris) {
            var worldTri = tri.Transform(shader.ModelToWorld); // Convert to world space
            shader.WorldNormal = worldTri.Normal;

            // Backface culling
            if (!material.TwoSided) {
                var normal = worldTri.Normal; 
                if (Vec3.Dot(backwards, normal) < -0.1) {
                    continue;
                }
            }

            // Get triangles
            var v1 = WorldToScreenPoint(worldTri.Item1);
            var v2 = WorldToScreenPoint(worldTri.Item2);
            var v3 = WorldToScreenPoint(worldTri.Item3);

            // Get the UV coordinates
            Vec2 uv1 = (uvs != null) ? uvs[tri.Item1] : Vec2.Zero;
            Vec2 uv2 = (uvs != null) ? uvs[tri.Item2] : Vec2.Zero;
            Vec2 uv3 = (uvs != null) ? uvs[tri.Item3] : Vec2.Zero;

            Rasterize(ref shader, worldTri, v1, v2, v3, uv1, uv2, uv3, material);
        }
    }

    private void Rasterize(ref ShaderVariables shader, Triangle worldTri, Vec3 v1, Vec3 v2, Vec3 v3, Vec2 uv1, Vec2 uv2, Vec2 uv3, Material material) {
        // Sort vertices into ascending order of y
        var worldV1 = worldTri.Item1;
        var worldV2 = worldTri.Item2;
        var worldV3 = worldTri.Item3;

        if (v1.Y > v2.Y) {
            (v1, v2) = (v2, v1);
            (uv1, uv2) = (uv2, uv1);
            (worldV1, worldV2) = (worldV2, worldV1);
        } 
        if (v2.Y > v3.Y) {
            (v2, v3) = (v3, v2);
            (uv2, uv3) = (uv3, uv2);
            (worldV2, worldV3) = (worldV3, worldV2);
        }
        if (v1.Y > v2.Y) {
            (v1, v2) = (v2, v1);
            (uv1, uv2) = (uv2, uv1);
            (worldV1, worldV2) = (worldV2, worldV1);
        } 

        // Rasterize
        if (v2.Y == v3.Y) {
            // Flat Bottom Triangle
            DrawFlatBottomTriangle (worldV1, worldV2, worldV3, v1, v2, v3, uv1, uv2, uv3, material, ref shader);

            DrawLine(worldV1, worldV2, v1, v2, uv1, uv2, material, ref shader, surface: false);
            DrawLine(worldV2, worldV3, v2, v3, uv2, uv3, material, ref shader, surface: false);
            DrawLine(worldV1, worldV3, v1, v3, uv1, uv3, material, ref shader, surface: false);

            DrawVertex(worldV1, v1, uv1, material, ref shader);
            DrawVertex(worldV2, v2, uv2, material, ref shader);
            DrawVertex(worldV3, v3, uv3, material, ref shader);
        } else if (v1.Y == v2.Y) {
            // Flat Top Triangle
            DrawFlatTopTriangle (worldV1, worldV2, worldV3, v1, v2, v3, uv1, uv2, uv3, material, ref shader);

            DrawLine(worldV1, worldV2, v1, v2, uv1, uv2, material, ref shader, surface: false);
            DrawLine(worldV2, worldV3, v2, v3, uv2, uv3, material, ref shader, surface: false);
            DrawLine(worldV1, worldV3, v1, v3, uv1, uv3, material, ref shader, surface: false);

            DrawVertex(worldV1, v1, uv1, material, ref shader);
            DrawVertex(worldV2, v2, uv2, material, ref shader);
            DrawVertex(worldV3, v3, uv3, material, ref shader);
        } else {
            // General Triangle
            double t = (v2.Y - v3.Y)/(v1.Y - v3.Y);
            Vec3 d = new Vec3(
                (v1.X) + ((v2.Y - v1.Y) / (v3.Y - v1.Y)) * (v3.X - v1.X),
                v2.Y,
                Lerp(v3.Z, v1.Z, t) 
            );
            Vec3 worldD = Vec3.Lerp(worldV3, worldV1, t);
            Vec2 uvd = new Vec2(
                Lerp(uv3.X, uv1.X, t),
                Lerp(uv3.Y, uv1.Y, t)
            );

            DrawFlatBottomTriangle (worldV1, worldV2, worldD, v1, v2, d, uv1, uv2, uvd, material, ref shader);
            DrawFlatTopTriangle (worldV2, worldD, worldV3, v2, d, v3, uv2, uvd, uv3, material, ref shader);

            DrawLine(worldV1, worldV2, v1, v2, uv1, uv2, material, ref shader, surface: false);
            DrawLine(worldV2, worldV3, v2, v3, uv2, uv3, material, ref shader, surface: false);
            DrawLine(worldV1, worldV3, v1, v3, uv1, uv3, material, ref shader, surface: false);

            DrawVertex(worldV1, v1, uv1, material, ref shader);
            DrawVertex(worldV2, v2, uv2, material, ref shader);
            DrawVertex(worldV3, v3, uv3, material, ref shader);
        }
    }

    private static double Lerp(double a, double b, double t) {
        return (1 - t) * a + t * b;
    }

    private static Color Blend(Color backColor, Color color, double amount) {
        byte r = (byte) ((color.R * amount) + backColor.R * (1 - amount));
        byte g = (byte) ((color.G * amount) + backColor.G * (1 - amount));
        byte b = (byte) ((color.B * amount) + backColor.B * (1 - amount));
        return Color.FromArgb(r, g, b);
    }

    private void SetPixel(Vec3 pixel, Color c) {
        var x = (int)pixel.X;
        var y = (int)pixel.Y;
        var depth = pixel.Z;

        if (x >= 0 && y >= 0 && x < Size.Width && y < Size.Height) {
            if (depth >= nearClipDistance && depth < DepthBuffer[y,x]) {
                var opacity = c.A / 255.0;
                // If fully opaque, set the depth buffer
                DepthBuffer[y,x] = Lerp(DepthBuffer[y,x], depth, opacity);
                PixelBuffer[y,x] = Blend(PixelBuffer[y,x], c, opacity);
            }
        }
    }

    private void DrawVertex(Vec3 world, Vec3 screen, Vec2 uv, Material material, ref ShaderVariables vars) {
        vars.WorldPosition = world;
        vars.ScreenPixel = screen;
        vars.UVCoordinates = uv;

        var colour = material.Vert(vars);
        SetPixel(screen, colour);
    }

    private void DrawLine(Vec3 worldV1, Vec3 worldV2, Vec3 v1, Vec3 v2, Vec2 uv1, Vec2 uv2, Material material, ref ShaderVariables shader, bool surface) {
        double dist2d = Math.Sqrt((v2.X - v1.X)*(v2.X - v1.X) + (v2.Y - v1.Y)*(v2.Y - v1.Y));
        if (dist2d != 0) {
            double invdist = 1/ dist2d;
            for (double i = 0; i < 1; i += invdist) {
                Vec3 world = Vec3.Lerp(worldV1, worldV2, i);
                Vec3 pixel = Vec3.Lerp(v1, v2, i);
                Vec2 uv = Vec2.Lerp(uv1, uv2, i);

                shader.WorldPosition = world;
                shader.ScreenPixel = pixel;
                shader.UVCoordinates = uv;

                var colour = surface ? material.Fragment(shader) : material.Edge(shader);
                SetPixel(pixel, colour);
            }
        }
    }

    private void DrawFlatBottomTriangle(Vec3 worldV1, Vec3 worldV2, Vec3 worldV3, Vec3 a, Vec3 b, Vec3 c, Vec2 uva, Vec2 uvb, Vec2 uvc, Material img, ref ShaderVariables shader){
        double invslope1 = (b.X - a.X)/(b.Y - a.Y);
        double invslope2 = (c.X - a.X)/(c.Y - a.Y);
        
        double leftX = Math.Floor(a.X);
        double rightX = Math.Floor(a.X);
        
        int startH = (int)Math.Floor(a.Y);
        int endH = (int)Math.Floor(b.Y);
        
        for(int scan = startH; scan < endH; scan++){
            
            //Texturing only
            double t = (scan - startH)/(endH - startH);

            Vec2 left = Vec2.Lerp(uva, uvb, t);
            Vec2 right = Vec2.Lerp(uva, uvc, t);
            Vec3 worldLeft = Vec3.Lerp(worldV1, worldV2, t);
            Vec3 worldRight = Vec3.Lerp(worldV1, worldV3, t);
            
            double zL = Lerp(a.Z, b.Z, t);
            double zR = Lerp(a.Z, c.Z, t);
            DrawLine(worldLeft, worldRight, new Vec3(leftX,scan,zL),new Vec3(rightX,scan,zR),left,right,img, ref shader, surface: true);
            
            leftX += invslope1;
            rightX += invslope2;
        }
    }
    private void DrawFlatTopTriangle (Vec3 worldV1, Vec3 worldV2, Vec3 worldV3, Vec3 a, Vec3 b, Vec3 c, Vec2 uva, Vec2 uvb, Vec2 uvc, Material img, ref ShaderVariables shader){
        double invslope1 = (c.X - a.X) / (c.Y - a.Y);
        double invslope2 = (c.X - b.X) / (c.Y - b.Y);
        
        double leftX = Math.Floor(c.X);
        double rightX = Math.Floor(c.X);

        int startH = (int)Math.Floor(c.Y);
        int endH = (int)Math.Floor(a.Y);
        
        for(int scan = startH; scan > endH; scan--){
            double t = (scan - startH)/(endH - startH);

            Vec2 left = Vec2.Lerp(uvc, uva, t);
            Vec2 right = Vec2.Lerp(uvc, uvb, t);
            Vec3 worldLeft = Vec3.Lerp(worldV3, worldV1, t);
            Vec3 worldRight = Vec3.Lerp(worldV3, worldV2, t);

            double zL = Lerp(c.Z, a.Z, t);
            double zR = Lerp(c.Z, b.Z, t);
            DrawLine(worldLeft, worldRight, new Vec3(leftX,scan, zL),new Vec3(rightX,scan, zR),left,right,img, ref shader, surface: true);
            
            leftX -= invslope1;
            rightX -= invslope2;
        }
    }
}

}