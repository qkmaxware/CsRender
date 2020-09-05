using System.Drawing;
using Qkmaxware.Geometry;
using Qkmaxware.Geometry.Primitives;

namespace Qkmaxware.Rendering {

/// <summary>
/// XYZ axis helper
/// </summary>
public class AxisHelper : SceneNode {

    private MeshRenderer XObject;
    private MeshRenderer XLabelObject;
    private MeshRenderer YObject;
    private MeshRenderer YLabelObject;
    private MeshRenderer ZObject;
    private MeshRenderer ZLabelObject;

    /// <summary>
    /// Create a new axis helper
    /// </summary>
    public AxisHelper() {
        var rootX = CreateAxis(new UnlitColour(Color.Red), out XObject, out XLabelObject);
        var rootY = CreateAxis(new UnlitColour(Color.Green), out YObject, out YLabelObject);
        var rootZ = CreateAxis(new UnlitColour(Color.Blue), out ZObject, out ZLabelObject);

        rootX.Transform = Transformation.Ry(-90 * Angle.Deg2Rad);
        rootY.Transform = Transformation.Rx(-90 * Angle.Deg2Rad);
    }

    private string? xLabel;

    /// <summary>
    /// The label used for the x-Axis
    /// </summary>
    /// <value>the label</value>
    public string? XLabel {
        get {
            return xLabel;
        }
        set {
            this.xLabel = value;
            if (value != null) {
                var mesh = new TextMesh(value);
                XLabelObject.Mesh = mesh;
                XLabelObject.Material = XObject.Material;
            }
        }
    }

    private string? yLabel;
    /// <summary>
    /// The label used for the y-Axis
    /// </summary>
    /// <value>the label</value>
    public string? YLabel {
        get {
            return yLabel;
        }
        set {
            this.yLabel = value;
            if (value != null) {
                var mesh = new TextMesh (value);
                YLabelObject.Mesh = mesh;
                YLabelObject.Material = YObject.Material;
            }
        }
    }

    private string? zLabel;
    /// <summary>
    /// The label used for the z-Axis
    /// </summary>
    /// <value>the label</value>
    public string? ZLabel {
        get {
            return zLabel;
        }
        set {
            this.zLabel = value;
            if (value != null) {
                var mesh = new TextMesh(value);
                ZLabelObject.Mesh = mesh;
                ZLabelObject.Material = ZObject.Material;
            }
        }
    }

    private SceneNode CreateAxis(Material mat, out MeshRenderer arrow, out MeshRenderer label) {
        var arrowMesh = new Arrow(length: 1);
        arrow = new MeshRenderer(mesh: arrowMesh, material: mat);
        label = new MeshRenderer();
        label.Transform = Transformation.Offset(new Vec3(0, 0, 1.1)) * Transformation.Ry(90 * Angle.Deg2Rad);
        
        var root = new SceneNode();
        root.Add(arrow);
        root.Add(label);

        this.Add(root);
        return root;
    }
}

}