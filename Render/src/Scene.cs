using System;
using System.Collections;
using System.Collections.Generic;
using Qkmaxware.Geometry;

/// <summary>
/// 3D Soft-rendering 
/// </summary>
namespace Qkmaxware.Rendering {

/// <summary>
/// Scene for object management
/// </summary>
public class Scene : IEnumerable<SceneNode> {
    private List<SceneNode> children = new List<SceneNode>();

    /// <summary>
    /// Add a node to the scene graph's root level
    /// </summary>
    /// <param name="node">node to add</param>
    public void Add(SceneNode node) {
        this.children.Add(node);
        node.root_scene = this;
    }

    /// <summary>
    /// Remove a node from the scene graph's root level
    /// </summary>
    /// <param name="node">node to remove</param>
    public void Remove(SceneNode node) {
        if(this.children.Remove(node)) {
            node.root_scene = null;
        }
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
}

}