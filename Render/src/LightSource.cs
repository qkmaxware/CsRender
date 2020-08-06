using System;
using System.Drawing;
using Qkmaxware.Geometry;

namespace Qkmaxware.Rendering {

/// <summary>
/// Base class for light sources
/// </summary>
public abstract class LightSource : SceneNode {

    /// <summary>
    /// Light colour
    /// </summary>
    public Color Tint {get; set;} = Color.White;

    /// <summary>
    /// Intensity of the light at the given position
    /// </summary>
    /// <param name="position">world position</param>
    /// <param name="normal">world normal</param>
    /// <returns>intensity of the light between 0 and 1</returns>
    public abstract double Intensity(Vec3 position, Vec3 normal);

    /// <summary>
    /// Direction of the light incident to the given position
    /// </summary>
    /// <param name="position">world position</param>
    /// <param name="normal">world normal</param>
    /// <returns>light direction</returns>
    public abstract Vec3 Direction (Vec3 position, Vec3 normal);

}

/// <summary>
/// Soft light applied from all angles 
/// </summary>
public class AmbientLight : LightSource {
    public double SourceIntensity {get; set;} = 0.1;
    public override double Intensity(Vec3 position, Vec3 normal) {
       return SourceIntensity; 
    }
    public override Vec3 Direction (Vec3 position, Vec3 normal) {
        return -normal;
    }
}

/// <summary>
/// Light applied from a point in space
/// </summary>
public class PointLight : LightSource {
    public double SourceIntensity {get; set;} = 1;
    public override double Intensity(Vec3 position, Vec3 normal) {
        var distance = Vec3.Distance(this.Position, position);
        var intensity = (1 / (distance * distance));
        return SourceIntensity * Math.Max(intensity, 0); 
    }

    public override Vec3 Direction (Vec3 position, Vec3 normal) {
        return (position - this.Position).Normalized;
    }
}

}