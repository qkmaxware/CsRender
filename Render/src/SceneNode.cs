using System;
using System.Collections;
using System.Collections.Generic;
using Qkmaxware.Geometry;

namespace Qkmaxware.Rendering {

/// <summary>
/// A node which exists within the scene graph
/// </summary>
public class SceneNode : IEnumerable<SceneNode> {
    /// <summary>
    /// Scene in which this node belongs
    /// </summary>
    /// <value></value>
    public Scene? Scene => (Parent != null) ? Parent.Scene : this.root_scene;

    // Use this to hide the property from doxygen
    /*! \cond PRIVATE */
    internal Scene? root_scene = null;
    /*! \endcond */

    /// <summary>
    /// Parent node in the scene graph if it exists
    /// </summary>
    public SceneNode? Parent {get; private set;} = null;
    /// <summary>
    /// Local transformation matrix representing the coordinate frame for this node
    /// </summary>
    public Transformation Transform = Transformation.Identity();
    /// <summary>
    /// World position of this node
    /// </summary>
    public Vec3 Position => LocalToWorldMatrix * Vec3.Zero;
    /// <summary>
    /// World aligned forward vector
    /// </summary>
    public Vec3 Forward => LocalToWorldMatrix * Vec3.J;
    /// <summary>
    /// World aligned backwards vector
    /// </summary>
    public Vec3 Backward => -Forward;
    /// <summary>
    /// World aligned up vector
    /// </summary>
    public Vec3 Up => LocalToWorldMatrix * Vec3.K;
    /// <summary>
    /// World aligned down vector
    /// </summary>
    public Vec3 Down => -Up;
    /// <summary>
    /// World aligned left vector
    /// </summary>
    public Vec3 Left => -Right;
    /// <summary>
    /// World aligned right vector
    /// </summary>
    public Vec3 Right => LocalToWorldMatrix * Vec3.I;

    /// <summary>
    /// Matrix to convert from local to world space
    /// </summary>
    public Transformation LocalToWorldMatrix => (Parent != null) ? Parent.LocalToWorldMatrix * this.Transform : this.Transform;
    
    /// <summary>
    /// Matrix to convert from world to local space
    /// </summary>
    public Transformation WorldToLocalMatrix => LocalToWorldMatrix.Inverse;

    private List<SceneNode> children = new List<SceneNode>();

    /// <summary>
    /// Add a node to the scene graph under this node
    /// </summary>
    /// <param name="node">node to add</param>
    public void Add(SceneNode node) {
        // Remove from old parent
        node.Parent?.Remove(node);
        // Add to new parent
        this.children.Add(node);
        node.Parent = this;
    }

    /// <summary>
    /// Remove a node from the scene graph
    /// </summary>
    /// <param name="node">node to remove</param>
    public void Remove(SceneNode node) {
        if (this.children.Remove(node)) {
            node.Parent = null;
        }
    }

    /// <summary>
    /// Remove this node from the scene graph
    /// </summary>
    public void Detach() {
        this.Parent?.Remove(this);
    }

    public IEnumerator<SceneNode> GetEnumerator() {
        foreach (var child in children) {
            yield return child;
            foreach (var subchild in child) {
                yield return subchild;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    /// <summary>
    /// Rotate this coordinate frame about an axis
    /// </summary>
    /// <param name="axis">axis to rotate</param>
    /// <param name="angle">rotation amount</param>
    public void Rotate(Vec3 axis, double angle) {
        var norm = axis.Normalized;
        double c = Math.Cos(angle);
        double s = Math.Sin(angle);
        double t = 1.0 - c;
        double x = norm.X;
        double y = norm.Y;
        double z = norm.Z;

        var rotation = new Transformation(
            t * x * x + c,          t * x * y - z * s,      t * x * z + y * s,      0,
            t * x * y + z * s,      t * y * y + c,          t * y * z - x * s,      0,
            t * x * z - y * s,      t * y * z + x * s,      t * z * z + c,          0
        );

        this.Transform = rotation * this.Transform;
    }

    /// <summary>
    /// Rotate this node around a point in space
    /// </summary>
    /// <param name="point">point to orbit</param>
    /// <param name="axis">axis of rotation</param>
    /// <param name="angle">rotation amount</param>
    public void RotateAround(Vec3 point, Vec3 axis, double angle) {
        var norm = axis.Normalized;
        double c = Math.Cos(angle);
        double s = Math.Sin(angle);
        double t = 1.0 - c;
        double x = norm.X;
        double y = norm.Y;
        double z = norm.Z;

        var rotation = new Transformation(
            t * x * x + c,          t * x * y - z * s,      t * x * z + y * s,      0,
            t * x * y + z * s,      t * y * y + c,          t * y * z - x * s,      0,
            t * x * z - y * s,      t * y * z + x * s,      t * z * z + c,          0
        );

        this.Transform = (Transformation.Offset(point) * rotation * Transformation.Offset(-point)) * this.Transform;
    }

    /// <summary>
    /// Move this node 
    /// </summary>
    /// <param name="delta">amount to move by</param>
    public void Move(Vec3 delta) {
        this.Transform = Transformation.Offset(delta) * this.Transform;
    }
}

}