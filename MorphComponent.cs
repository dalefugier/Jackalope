using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;

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
    protected static Rhino.Geometry.GeometryBase MorphGeometry(Rhino.Geometry.SpaceMorph morph, Rhino.Geometry.GeometryBase old_geometry)
    {
      if (null == morph || null == old_geometry || !old_geometry.IsValid)
        return null;

      bool bIsMorphable = Rhino.Geometry.SpaceMorph.IsMorphable(old_geometry);

      Rhino.Geometry.Curve old_curve = old_geometry as Rhino.Geometry.Curve;
      Rhino.Geometry.NurbsCurve new_curve = null;
      if (null != old_curve && (null == old_curve as Rhino.Geometry.NurbsCurve || !bIsMorphable))
      {
        new_curve = old_curve.ToNurbsCurve();
        if (null != new_curve)
          bIsMorphable = true;
      }

      Rhino.Geometry.Surface old_surface = old_geometry as Rhino.Geometry.Surface;
      Rhino.Geometry.Brep new_surface = null;
      if (null != old_surface && (null == old_surface as Rhino.Geometry.NurbsSurface || !bIsMorphable))
      {
        new_surface = Rhino.Geometry.Brep.CreateFromSurface(old_surface);
        if (null != new_surface)
          bIsMorphable = true;
      }

      Rhino.Geometry.Extrusion old_extrusion =  old_geometry as Rhino.Geometry.Extrusion;
      Rhino.Geometry.Brep new_extrusion = null;
      if (null != old_extrusion && !bIsMorphable )
      {
        new_extrusion = old_extrusion.ToBrep(true);
        if (null != new_extrusion)
          bIsMorphable = true;
      }

      Rhino.Geometry.GeometryBase new_geometry = null;
      if (bIsMorphable)
      {
        if (null != new_curve)
          new_geometry = new_curve;
        else if (null != new_surface)
          new_geometry = new_surface;
        else if (null != new_extrusion)
          new_geometry = new_extrusion;
        else
          new_geometry = old_geometry.Duplicate();

        if (null != new_geometry)
        {
          if (morph.Morph(new_geometry) )
          {
            Rhino.Geometry.Brep brep = new_geometry as Rhino.Geometry.Brep;
            if (null != brep )
            {
              brep.Faces.SplitKinkyFaces(Rhino.RhinoMath.DefaultAngleTolerance, true);
              if (Rhino.Geometry.BrepSolidOrientation.Inward == brep.SolidOrientation)
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
