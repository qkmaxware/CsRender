using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Qkmaxware.Rendering {

/// <summary>
/// Animated scene node
/// </summary>
public interface IAnimator {
    /// <summary>
    /// Called before update
    /// </summary>
    /// <param name="dt">time since last update</param>
    void OnEarlyUpdate(TimeSpan dt) {}
    /// <summary>
    /// Called during animation update
    /// </summary>
    /// <param name="dt">time since last update</param>
	void OnUpdate(TimeSpan dt) {}
    /// <summary>
    /// Called after update
    /// </summary>
    /// <param name="dt">time since last update</param>
	void OnLateUpdate(TimeSpan dt) {}
}

/// <summary>
/// Animate all animated scene nodes across several frames
/// </summary>
public class AnimatedScene : IEnumerable<Color[,]> {

    /// <summary>
    /// Scene being animated
    /// </summary>
	public Scene Scene;
    /// <summary>
    /// Camera used for rendering animation
    /// </summary>
	public BaseCamera Camera {get; private set;}
    /// <summary>
    /// Number of frames per second
    /// </summary>
	public int FPS = 30;
    /// <summary>
    /// Time between frames
    /// </summary>
	public TimeSpan DeltaTime => TimeSpan.FromSeconds(1.0/FPS);
	
    /// <summary>
    /// Create an animated rendering of the given scene with a default camera
    /// </summary>
    /// <param name="scene">scene to animate</param>
	public AnimatedScene(Scene scene) : this(new PerspectiveCamera(new Size(640, 480)), scene) {}
    
    /// <summary>
    /// Create an animated rendering of the given scene with the given camera
    /// </summary>
    /// <param name="camera">camera</param>
    /// <param name="scene">scene to animate</param>
	public AnimatedScene(BaseCamera camera, Scene scene) {
		this.Camera = camera;
        this.Scene = scene;
	}

    public IEnumerator<Color[,]> GetEnumerator() {
        while(true) {
			var objects = Scene.OfType<IAnimator>();
			var dt = DeltaTime;
			foreach (var obj in objects) {
				obj.OnEarlyUpdate(dt);
			}
			foreach (var obj in objects) {
				obj.OnUpdate(dt);
			}
			foreach (var obj in objects) {
				obj.OnLateUpdate(dt);
			}
			Camera.Render(Scene);
			yield return Camera.PixelBuffer;
		}
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}

}