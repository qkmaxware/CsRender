# Usage

## Scene management
All scene management start off from the `Scene` class which acts as the root of a hierarchy of scene objects, not all of which are able to be rendered to the camera.

```cs
Scene scene = new Scene();
```

We can attach `SceneNode` objects to the scene and to other scene nodes in order to fill out the scene hierarchy. Currently there are 3 types of scene nodes. The base `SceneNode` class is used purely to construct the hierarchy and provides no additional functionality. The `MeshRenderer` class is used to render a geometric object to the camera. This object can be provided with a UV map which is used to map geometry vertices to texture coordinates as well as a material which defines how the object is coloured. Additionally the `Animator` class is used with an `AnimatedScene` to apply actions to nodes between animation frames which is covered the section [Animated Scenes](#animated-scenes).

```cs
SceneNode emptyNode = new SceneNode();
scene.Add(emptyNode);

MeshRenderer mesh = new MeshRenderer(
    mesh: new Sphere(radius: 1, centre: Vec3.Zero),
    uv: null,
    material: new Wireframe(Color.Red) 
);
emptyNode.Add(mesh);
```

## Rendering
Rendering is performed by any camera object. This framework only implements a simple "perspective" camera using the `PerspectiveCamera` class. When the camera renders a scene the camera's `PixelBuffer` property will contain the pixels of the image.

```cs
BaseCamera camera = new PerspectiveCamera(Resolutions.Aspect4x3.Display480p);
camera.Render(scene);
```

Since the camera does not actually create an image, you may use whatever framework you wish for saving the images to file or creating video files. For instance, using my [Media](https://github.com/qkmaxware/CsMedia) library, this is one way that an image file could be saved from the results of a camera rendering. 

```cs
TgaSerializer serializer = new TgaSerializer();
using (var writer = new BinaryWriter("myrender.tga", FileMode.Create))) {
    serializer.Serialize(writer, camera.PixelBuffer.GetSampler());
}
```
### Materials
Rendering will only render `Renderable` scene nodes to the camera's `PixelBuffer`. To control how these objects are rendered, materials can be defined. Materials like `Wireframe` will only render a the edges of a mesh whereas `UnlitColour` will render all faces but as a solid colour. Materials have 3 customizable methods `Vert`, `Edge`, and `Fragment` similar to how shaders work in other frameworks. The `Vert` method is called on every vertex being rendered. The `Edge` method is called for all pixels lying on the edge of a rendered triangle. The `Fragment` method is called for all pixels on the surface of a triangular face. 

```cs
public class CustomMaterial : Material {
    public override Color Vert(ShaderVariables variables) {...}
    public override Color Edge(ShaderVariables variables) {...}
    public override Color Fragment(ShaderVariables variables) {...}
}
```

Shader variables can be used to help determine what colour should be returned from the material for a given pixel. Some of the properties passed to the material through the `ShaderVariables` object include:

| Property | Description |
|----------|-------------|
| ModelToWorld | Matrix from the current object being rendered to world space |
| WorldToModel | Matrix from world space to the current object being rendered |
| WorldPosition | Position of the pixel in world space |
| WorldNormal | Normal vector of the face in world space |
| WorldCameraPosition | Position of the camera in world space |
| ScreenPixel | Position of the pixel on screen being pushed to |
| UVCoordinates | Coordinates for UV mapping the current pixel |
| LightSources | Currently active light sources in the scene to account for when rendering |

## Animated Scenes
More often then not, one would want to animate a scene and export each frame to file or to an animation. This starts from the `AnimatedScene` base class which allows for one to enumerate through animations frame by frame. The `FixedFpsAnimatedScene` uses a pre-asigned FPS property to determine how much time occurs between frames whereas the `RealtimeAnimatedScene` computes the amount of real time between successive renders. 

```cs
FixedFpsAnimatedScene animation = new FixedFpsAnimatedScene(camera, scene);
```

To create animated behaviours the `IAnimator` interface can be applied to SceneNode classes in order to apply events on each frame. Each `IAnimator` can choose to implement one of three methods. `OnEarlyUpdate`, `OnUpdate`, and `OnLateUpdate` which are called in sequence each frame. Additionally one can implement the `OnStart` method which is run when the animation first starts. Create an implementor of the `IAnimator` interface to define how the animation plays. The example `IAnimator` below will cause the node to bob vertically up and down over time. 

```cs
public class BobbingAnimator : SceneNode, IAnimator {
    public double Amplitude = 1;
    public double SpeedFactor = 1;
    private TimeSpan playTime = TimeSpan.Zero;

    public void OnUpdate(TimeSpan dt) {
        playTime += SpeedFactor * dt;

        var old = this.Transform.Position;
        var @new = Transformation.Offset(new Vector3(old.X, old.Y, Amplitude * Math.Sin(playTime.TotalSeconds)));

        this.Transform = @new;
    }
}
```

```cs
var bobber = new BobbingAnimator();
bobber.Add(mesh);
scene.Add(bobber);
```

To export animations, simple iterate over the frames of the animation that you want to keep an export them to file. Similar to how rendering works for a single camera, a third part library like my [Media](https://github.com/qkmaxware/CsMedia) library can be used to export the frames to image files. 

```cs
TgaSerializer serializer = new TgaSerializer();
var frameId = 0;

// Save 30 frames of the animation to files
foreach (var frame in animation.Take(30)) {
    using (var writer = new BinaryWriter($"render.{frameId++}.tga", FileMode.Create))) {
        serializer.Serialize(writer, frame.GetSampler());
    }
}
```