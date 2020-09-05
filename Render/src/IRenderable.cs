using System.Collections.Generic;
using Qkmaxware.Geometry;

namespace Qkmaxware.Rendering {

/// <summary>
/// Interface for any renderable 3d objects
/// </summary>
public interface IRenderable {
    /// <summary>
    /// Mesh to render
    /// </summary>
    IEnumerable<Triangle>? Mesh {get;}
    /// <summary>
    /// Vertex UV coordinates
    /// </summary>
    IUvMap? UVs {get;}
    /// <summary>
    /// Render material
    /// </summary>
    Material? Material {get;}
    // <summary>
    /// Matrix to convert from local to world space
    /// </summary>
    Transformation LocalToWorldMatrix {get;}
}

}