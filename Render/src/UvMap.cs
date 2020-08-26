using System;
using System.Collections;
using System.Collections.Generic;
using Qkmaxware.Geometry;

namespace Qkmaxware.Rendering {

/// <summary>
/// Mapping of vertices to 2D UV coordinates
/// </summary>
public interface IUvMap {
    /// <summary>
    /// Get the UV coordinates for a particular model vertex
    /// </summary>
    /// <value>UV coordinate or zero if it does not exist</value>
    Vec2 this[Vec3 vec] {get;}
}

/// <summary>
/// Mapping of vertices to 2D UV coordinates
/// </summary>
public class UV : IUvMap, IEnumerable<Vec2> {

    private Dictionary<Vec3, Vec2> uvs = new Dictionary<Vec3, Vec2>();

    /// <summary>
    /// Empty UV map
    /// </summary>
    public UV() {}
    /// <summary>
    /// UV map with existing mappings
    /// </summary>
    /// <param name="keys">mapping</param>
    public UV(IEnumerable<KeyValuePair<Vec3, Vec2>> keys) {
        foreach (var pair in keys) {
            AddMapping(pair.Key, pair.Value);
        }
    }

    private static Vec2 ToSpherical(Vec3 v) {
        // All values between -π ≤ θ ≤ π
        var phi = Math.Atan2(v.Y, v.X);                                 // rotation around Z
        var theta = Math.Atan2(Math.Sqrt(v.X * v.X + v.Y * v.Y), v.Z);  // rotation around X

        // All values between 0 and 1
        return new Vec2(
            ((phi / (Math.PI)) + 1)/2.0,    // Range (-π ≤ θ ≤ π) -> (-1 ≤ θ ≤ 1) -> (0 ≤ θ ≤ 2) -> (0 ≤ θ ≤ 1)
            (theta / (Math.PI))             // Range (0  ≤ θ ≤ π) -> (0  ≤ θ ≤ 1)
        );
    }

    /// <summary>
    /// Create a UV map from a spherical projection
    /// </summary>
    /// <param name="triangles">geometry to map</param>
    /// <returns>UV map</returns>
    public static UV Spherical(IEnumerable<Triangle> triangles) {
        UV map = new UV();
        foreach (var tri in triangles) {
            map.AddMapping(tri.Item1, ToSpherical(tri.Item1));
            map.AddMapping(tri.Item2, ToSpherical(tri.Item2));
            map.AddMapping(tri.Item3, ToSpherical(tri.Item3));
        }
        return map;
    }

    /// <summary>
    /// Add a mapping between vertex and UV coordinate
    /// </summary>
    /// <param name="key">vertex</param>
    /// <param name="value">UV coordinates</param>
    public void AddMapping(Vec3 key, Vec2 value) {
        this.uvs[key] = value;
    }

    /// <summary>
    /// Remove a mapping
    /// </summary>
    /// <param name="key">vertex</param>
    public void RemoveMapping(Vec3 key) {
        this.uvs.Remove(key);
    }

    public IEnumerator<Vec2> GetEnumerator() {
        return uvs.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    /// <summary>
    /// Get the UV coordinates for a particular model vertex
    /// </summary>
    /// <value>UV coordinate or zero if it does not exist</value>
    public Vec2 this[Vec3 vec] {
        get {
            if (uvs.ContainsKey(vec)) {
                return uvs[vec];
            } else {
                return Vec2.Zero;
            }
        }
    }

}

}