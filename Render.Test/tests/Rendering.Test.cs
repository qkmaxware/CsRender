using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qkmaxware.Rendering;
using Qkmaxware.Geometry;
using Qkmaxware.Geometry.Primitives;
using Qkmaxware.Media.Image;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Qkmaxware.Testing.Rendering {

[TestClass]
public class RenderingTest {

    [TestMethod]
    public void SimpleRender() {
        // Create scene
        var camera = new PerspectiveCamera(new Size(120, 120));
        camera.Transform = Transformation.Offset(new Vec3(0, -5, 0));
        var scene = new Scene();
        
        // Create object
        var mesh = new Geometry.Primitives.Cube(size: 1, centre: Vec3.Zero);
        var obj = new Renderable(mesh: mesh, uv: UV.Spherical(mesh), material: new Wireframe(Color.Red));
        scene.Add(obj);

        // Render
        SaveAnimation("render.simple", SpinAnimation(camera, obj, frames: 16));
    }

    [TestMethod]
    public void BasicRender() {
        // Create scene
        var camera = new PerspectiveCamera(new Size(120, 120));
        camera.Transform = Transformation.Offset(new Vec3(0, -5, 0));
        var scene = new Scene();

        // Create objects
        var ablock  = Transformation.Scale(new Vec3(0.2, 1, 0.2)) * new Cube(size: 1, centre: Vec3.Zero);
        var bblock  = Transformation.Scale(new Vec3(0.2, 1, 0.2)) * new Cube(size: 1, centre: Vec3.Zero);
        var ball    = new Sphere(radius: 0.2, centre: Vec3.Zero);

        var aobj    = new Renderable(mesh: ablock, material: new UnlitColour(Color.Red));
        var bobj    = new Renderable(mesh: bblock, material: new UnlitColour(Color.Blue));
        var ballobj = new Renderable(mesh: ball, material: new UnlitColour(Color.Yellow));
        
        aobj.Transform = Transformation.Offset(new Vec3(-1, 0, 0));
        bobj.Transform = Transformation.Offset(new Vec3(1, 0, 0));

        scene.Add(aobj);
        scene.Add(bobj);
        scene.Add(ballobj);

        // Render
        SaveAnimation("render.basic", PongAnimation(camera, ballobj, 32));
    }

    [TestMethod]
    public void ComplexRender() {
        // Create scene
        var camera = new PerspectiveCamera(new Size(120, 120));
        camera.Transform = Transformation.Offset(new Vec3(0, -5, 0));
        var scene = new Scene();

        // Set the skybox
        camera.Skybox = new GradientSkybox(Color.FromArgb (255, 58, 58, 82), Color.FromArgb (255, 2, 1, 17));

        // Create objects
        var planet = new Geometry.Primitives.Sphere(radius: 1, centre: Vec3.Zero, horizontalResolution: 32, verticalResolution: 32);
        var planetTexture = LoadImage("assets/planet.png");

        var satellite = new Geometry.Primitives.Sphere(radius: 0.2, centre: Vec3.Zero, horizontalResolution: 12, verticalResolution: 12);
        var satelliteTexture = LoadImage("assets/moon.png");

        var hierarchy = new SceneNode();
        scene.Add(hierarchy);

        var planetObj = new Renderable(mesh: planet, uv: UV.Spherical(planet), material: new UnlitTexture(planetTexture));
        hierarchy.Add(planetObj);

        var satelliteObj = new Renderable(mesh: satellite, uv: UV.Spherical(satellite), material: new UnlitTexture(satelliteTexture)); 
        satelliteObj.Transform = Transformation.Offset(new Vec3(-1.6, 1.6, 0.1));
        hierarchy.Add(satelliteObj);

        // Render
        SaveAnimation("render.complex", SpinAnimation(camera, hierarchy, frames: 64));
    }

    private IEnumerable<IColourSampler> SpinAnimation (BaseCamera camera, SceneNode spinnable, int frames = 16) {
        var increment = 360.0 / (frames + 1);
        for (var i = 0; i <= frames; i++) {
            spinnable.Rotate(Vec3.K, Angle.Deg2Rad * increment);
            if (spinnable.Scene != null)
                camera.Render(spinnable.Scene);
            yield return camera.PixelBuffer.GetSampler();
        }
    }

    private IEnumerable<IColourSampler> PongAnimation(BaseCamera camera1, SceneNode pong, int frames) {
        var dt = (frames + 1) / (2*Math.PI);
        var time = 0.0;
        for (var i = 0; i <= frames; i++) {
            var x = Math.Sin(time);
            time += dt;
            pong.Transform = Transformation.Offset(new Vec3(x, 0, 0));
            if (pong.Scene != null)
                camera1.Render(pong.Scene);
            yield return camera1.PixelBuffer.GetSampler();
        }
    }

    private void SaveAnimation(string animation_name, IEnumerable<IColourSampler> frames) {
        var path = Path.Combine("data", "animation", animation_name);
        if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

        TgaSerializer serializer = new TgaSerializer();
        var frameId = 0;
        foreach (var frame in frames) {
            using (var writer = new BinaryWriter(File.Open(Path.Join(path,$"frame.{frameId}.tga"), FileMode.Create))) {
                serializer.Serialize(writer, frame);
            }
            frameId++;
        }
    }

    private Texture2D LoadImage(string image) {
        Bitmap bmp = new Bitmap("../../../"+image);
        var pixels = new Color[bmp.Height,bmp.Width];
        for (var row = 0; row < bmp.Height; row++) {
            for (var col = 0; col < bmp.Width; col++) {
                pixels[row, col] = bmp.GetPixel(col, row);
            }
        }
        return new PixelTexture(pixels);
    }
}

}