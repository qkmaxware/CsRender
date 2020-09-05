using System.Collections.Generic;
using System.Drawing;
using Qkmaxware.Geometry;

namespace Qkmaxware.Rendering {

/// <summary>
/// Object that can be rendered by the camera
/// </summary>
public class MeshRenderer : SceneNode, IRenderable {
    /// <summary>
    /// Mesh to render
    /// </summary>
    /// <returns>mesh</returns>
    public IEnumerable<Triangle>? Mesh {get; set;}
    /// <summary>
    /// Vertex UV coordinates
    /// </summary>
    /// <returns>uv map</returns>
    public IUvMap? UVs {get; set;}
    /// <summary>
    /// Render material
    /// </summary>
    /// <returns>material</returns>
    public Material? Material {get; set;} = new UnlitColour(Color.White);

    /// <summary>
    /// Create empty renderable
    /// </summary>
    public MeshRenderer() {}

    /// <summary>
    /// Create renderable with geometry
    /// </summary>
    /// <param name="mesh">geometry</param>
    public MeshRenderer(IEnumerable<Triangle> mesh) {
        this.Mesh = mesh;
    }

    /// <summary>
    /// Create renderable geometry with uv's and texture data
    /// </summary>
    /// <param name="mesh">geometry</param>
    /// <param name="material">material</param>
    /// <param name="uv">uv map</param>
    public MeshRenderer(IEnumerable<Triangle> mesh, Material material, IUvMap? uv = null) {
        this.Mesh = mesh;
        this.UVs = uv;
        this.Material = material;
    }
}

}