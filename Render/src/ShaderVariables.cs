using System.Collections.Generic;
using Qkmaxware.Geometry;

namespace Qkmaxware.Rendering {

/// <summary>
/// Variables that can be used by materials to apply shading
/// </summary>
public struct ShaderVariables {
    // Transformations
	/// <summary>
	/// Matrix from the current object being rendered to world space
	/// </summary>
    public Transformation ModelToWorld;                         // Assigned when model is chosen for rendering
	/// <summary>
	/// Matrix from world space to the current object being rendered
	/// </summary>
    public Transformation WorldToModel => ModelToWorld.Inverse;
    /// <summary>
    /// Position of the pixel in world space
    /// </summary>
	public Vec3 WorldPosition;                                  // Assigned when pixel is drawn
    /// <summary>
    /// Face normal in world space
    /// </summary>
	public Vec3 WorldNormal;                                    // Assigned when triangle is chosen for rendering

	// Camera & Screen
    /// <summary>
    /// Position of the camera in world space
    /// </summary>
	public Vec3 WorldCameraPosition;                            // Assigned when camera is selected for rendering
	/// <summary>
	/// Position of the pixel on screen being pushed to
	/// </summary>
    public Vec3 ScreenPixel;                                    // Assigned when coordinate is projected to the screen

    // Texturing
    /// <summary>
    /// Coordinates for UV mapping the current pixel
    /// </summary>
    public Vec2 UVCoordinates;                                  // Assigned when coordinate is projected to the screen
	
	// Lighting
    /// <summary>
    /// Currently active light sources to account for when rendering
    /// </summary>
	public IEnumerable<LightSource> LightSources;               // Assigned when camera is selected for rendering
}

}