using System.Collections.Generic;
using System.Drawing;
using Qkmaxware.Geometry;

namespace Qkmaxware.Rendering {

/// <summary>
/// Object that can be rendered by the camera
/// </summary>
public class Renderable : SceneNode {
    /// <summary>
    /// Mesh to render
    /// </summary>
    public IEnumerable<Triangle>? Mesh;
    /// <summary>
    /// Vertex UV coordinates
    /// </summary>
    public IUvMap? UVs;
    /// <summary>
    /// Render material
    /// </summary>
    public Material Material = new UnlitColour(Color.White);

    /// <summary>
    /// Create empty renderable
    /// </summary>
    public Renderable() {}

    /// <summary>
    /// Create renderable with geometry
    /// </summary>
    /// <param name="mesh">geometry</param>
    public Renderable(IEnumerable<Triangle> mesh) {
        this.Mesh = mesh;
    }

    /// <summary>
    /// Create renderable geometry with uv's and texture data
    /// </summary>
    /// <param name="mesh">geometry</param>
    /// <param name="material">material</param>
    /// <param name="uv">uv map</param>
    public Renderable(IEnumerable<Triangle> mesh, Material material, IUvMap? uv = null) {
        this.Mesh = mesh;
        this.UVs = uv;
        this.Material = material;
    }
}

}