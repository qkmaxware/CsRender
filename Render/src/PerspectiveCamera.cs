using System;
using System.Drawing;
using Qkmaxware.Geometry;

namespace Qkmaxware.Rendering {

/// <summary>
/// Camera for perspective rendering
/// </summary>
public class PerspectiveCamera : BaseCamera {

    public PerspectiveCamera(Size size) : base(size) {}

    protected override void Dirty() {}

    /// <summary>
    /// Convert a pixel on the screen to a point in the world
    /// </summary>
    /// <param name="screen">screen pixel</param>
    /// <returns>world point at the view plane</returns>
    public override Vec3 ScreenToWorldPoint(Vec2 screen) {
        Vec3 local = new Vec3(
            screen.X - this.Size.Width/2.0,
            focallength,
            screen.Y - this.Size.Height/2.0
        );

        Vec3 world = this.LocalToWorldMatrix * local;

        return world;
    }

    /// <summary>
    /// Convert a point in the world to a pixel on the screen
    /// </summary>
    /// <param name="world">world point</param>
    /// <returns>screen pixel</returns>
    public override Vec3 WorldToScreenPoint(Vec3 world) {
        // Step 1, convert world space vector to local camera space vector
        var local = this.WorldToLocalMatrix * world;

        // Step 2, convert to screen space vector
        var screen = ToScreenSpace(local);

        var pixel = new Vec3(
            (screen.X * this.Size.Width + this.Size.Width/2.0) ,
            (screen.Y * this.Size.Height + this.Size.Height/2.0) ,
            screen.Z 
        );

        //Console.WriteLine($"{local} -> {pixel}");
        return pixel;
    }

    private Vec3 ToScreenSpace(Vec3 local) {
        var distance = local.Length;

        var zAngle = Math.Atan2(local.X, local.Y);
        var xAngle = Math.Atan2(local.Z, local.Y);

        var screenX = Math.Tan(zAngle) * focallength;
        var screenY = Math.Tan(xAngle) * focallength;

        return new Vec3(
            screenX,
            screenY,
            Math.Sign(local.Y) * distance // in front of or behind 
        );
    }

}

}