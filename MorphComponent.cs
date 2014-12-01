using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Jackalope
{
  public abstract class MorphComponent : GH_Component
  {
    /// <summary>
    /// Constructor
    /// </summary>
    protected MorphComponent(string name, string nickname, string description)
      : base(name, nickname, description, "Transform", "Jackalope")
    {
    }

    /// <summary>
    /// Morph helper function
    /// </summary>
    protected static GeometryBase MorphGeometry(SpaceMorph morph, GeometryBase oldGeometry)
    {
      if (null == morph || null == oldGeometry || !oldGeometry.IsValid)
        return null;

      bool is_morphable = SpaceMorph.IsMorphable(oldGeometry);

      var old_curve = oldGeometry as Curve;
      NurbsCurve new_curve = null;
      if (null != old_curve && (null == old_curve as NurbsCurve || !is_morphable))
      {
        new_curve = old_curve.ToNurbsCurve();
        if (null != new_curve)
          is_morphable = true;
      }

      var old_surface = oldGeometry as Surface;
      Brep new_surface = null;
      if (null != old_surface && (null == old_surface as NurbsSurface || !is_morphable))
      {
        new_surface = Brep.CreateFromSurface(old_surface);
        if (null != new_surface)
          is_morphable = true;
      }

      var old_extrusion =  oldGeometry as Extrusion;
      Brep new_extrusion = null;
      if (null != old_extrusion && !is_morphable )
      {
        new_extrusion = old_extrusion.ToBrep(true);
        if (null != new_extrusion)
          is_morphable = true;
      }

      GeometryBase new_geometry = null;
      if (is_morphable)
      {
        if (null != new_curve)
          new_geometry = new_curve;
        else if (null != new_surface)
          new_geometry = new_surface;
        else if (null != new_extrusion)
          new_geometry = new_extrusion;
        else
          new_geometry = oldGeometry.Duplicate();

        if (null != new_geometry)
        {
          if (morph.Morph(new_geometry) )
          {
            var brep = new_geometry as Brep;
            if (null != brep )
            {
              brep.Faces.SplitKinkyFaces(Rhino.RhinoMath.DefaultAngleTolerance, true);
              if (BrepSolidOrientation.Inward == brep.SolidOrientation)
                brep.Flip();
              brep.Compact();
            }
          }
        }
      }

      return new_geometry;
    }
  }
}
