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
    /// Called at the start of an animation
    /// </summary>
    void OnStart() {}
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
public abstract class AnimatedScene : IEnumerable<Color[,]> {

    /// <summary>
    /// Scene being animated
    /// </summary>
	public Scene Scene;
    /// <summary>
    /// Camera used for rendering animation
    /// </summary>
	public BaseCamera Camera {get; private set;}
	
    
    protected abstract TimeSpan DeltaTime();

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

    public virtual IEnumerator<Color[,]> GetEnumerator() {
        // First render
        var initialObjects = Scene.OfType<IAnimator>();
        foreach (var obj in initialObjects) {
            obj.OnStart();
        }
        Camera.Render(Scene);
        yield return Camera.PixelBuffer;

        // Subsequent render loop
        while(true) {
			var objects = Scene.OfType<IAnimator>();
			var dt = this.DeltaTime();
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

/// <summary>
/// Animated scene where the time between frames is fixed
/// </summary>
public class FixedFpsAnimatedScene : AnimatedScene {
    /// <summary>
    /// Number of frames per second
    /// </summary>
	public int FPS = 30;

    public FixedFpsAnimatedScene(Scene scene, int fps = 30) : base(scene) {
        this.FPS = fps;
    }

    public FixedFpsAnimatedScene(BaseCamera camera, Scene scene, int fps = 30) : base(camera, scene) {
        this.FPS = fps;
    }

    protected override TimeSpan DeltaTime() {
        return TimeSpan.FromSeconds(1.0/FPS);
    }
}

/// <summary>
/// Animated scene where the time between frames is determined by the delay between calls to Camera.Render
/// </summary>
public class RealtimeAnimatedScene : AnimatedScene {
    public RealtimeAnimatedScene(Scene scene) : base(scene) {}
    public RealtimeAnimatedScene(BaseCamera camera, Scene scene) : base(camera, scene) {}

    private DateTime lastFrame = DateTime.Now;
    protected override TimeSpan DeltaTime() {
        var now = DateTime.Now;
        var dt = now - lastFrame;
        lastFrame = now;
        return dt;
    }
}

}