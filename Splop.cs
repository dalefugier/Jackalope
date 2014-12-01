using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino.Geometry.Morphs;

namespace Jackalope
{
  public class Splop : MorphComponent
  {
    private int m_ge_index = -1; // geometry
    private int m_pl_index = -1; // plane
    private int m_sf_index = -1; // surface
    private int m_pt_index = -1; // surface point
    private int m_sc_index = -1; // scale
    private int m_an_index = -1; // angle
    private int m_ps_index = -1; // preserve structure
    private int m_qp_index = -1; // quick preview

    /// <summary>
    /// Initializes a new instance of the Splop class.
    /// </summary>
    public Splop()
      : base("Splop", "Splop", "Rotates, scales, and wraps objects on a surface.")
    {
    }

    /// <summary>
    /// Registers all the input parameters for this component.
    /// </summary>
    protected override void RegisterInputParams(GH_InputParamManager pm)
    {
      m_ge_index = pm.AddGeometryParameter("Geometry", "G", "Base geometry", GH_ParamAccess.list);
      m_pl_index = pm.AddPlaneParameter("Plane", "P", "Source plane of deformation", GH_ParamAccess.item);
      m_sf_index = pm.AddSurfaceParameter("Surface", "S", "Surface to wrap geometry onto", GH_ParamAccess.item);
      m_pt_index = pm.AddPointParameter("Parameter", "uv", "U,V parameter on surface used for orienting", GH_ParamAccess.item);
      m_sc_index = pm.AddNumberParameter("Factor", "F", "Scale factor", GH_ParamAccess.item, Rhino.RhinoMath.UnsetValue);
      m_an_index = pm.AddNumberParameter("Angle", "A", "Rotation angle in radians", GH_ParamAccess.item, Rhino.RhinoMath.UnsetValue);
      m_ps_index = pm.AddBooleanParameter("Preserve", "V", "If true, preserves the control point structure of the surface. If false, geometry is refit as needed with more control points to allow accurate deformation.", GH_ParamAccess.item, false);
      m_qp_index = pm.AddBooleanParameter("Quick", "Q", "If true, morph should be done as quickly as possible. If false, morph should be done as accurately as possible", GH_ParamAccess.item, false);

      pm[m_pl_index].Optional = true;
      pm[m_sc_index].Optional = true;
      pm[m_an_index].Optional = true;
      pm[m_ps_index].Optional = true;
      pm[m_qp_index].Optional = true;
    }

    /// <summary>
    /// Registers all the output parameters for this component.
    /// </summary>
    protected override void RegisterOutputParams(GH_OutputParamManager pm)
    {
      pm.AddGeometryParameter("Geometry", "G", "Morphed geometry", GH_ParamAccess.list);
    }

    /// <summary>
    /// This is the method that actually does the work.
    /// </summary>
    /// <param name="da">The DA object is used to retrieve from inputs and store in outputs.</param>
    protected override void SolveInstance(IGH_DataAccess da)
    {
      var geometry = new List<IGH_GeometricGoo>();
      var plane = Plane.WorldXY;
      Surface surface = null;
      var point = Point3d.Unset;
      var scale_factor = Rhino.RhinoMath.UnsetValue;
      var angle = Rhino.RhinoMath.UnsetValue;
      var preserve_structure = false;
      var quick_preview = false;

      if (!da.GetDataList(m_ge_index, geometry) || 0 == geometry.Count)
        return;
      if (!da.GetData(m_pl_index, ref plane))
        return;
      if (!da.GetData(m_sf_index, ref surface))
        return;
      if (!da.GetData(m_pt_index, ref point))
        return;
      if (!da.GetData(m_sc_index, ref scale_factor))
        return;
      if (!da.GetData(m_an_index, ref angle))
        return;
      if (!da.GetData(m_ps_index, ref preserve_structure))
        return;
      if (!da.GetData(m_qp_index, ref quick_preview))
        return;

      var morph = new SplopSpaceMorph(plane, surface, new Point2d(point), scale_factor, angle)
      {
        PreserveStructure = preserve_structure,
        QuickPreview = quick_preview
      };

      var output = geometry.Select(geom => geom.DuplicateGeometry().Morph(morph)).ToList();
      da.SetDataList(0, output);
    }

    /// <summary>
    /// Provides an Icon for the component.
    /// </summary>
    protected override System.Drawing.Bitmap Icon
    {
      get { return Properties.Resources.Splop; }
    }

    /// <summary>
    /// Gets the unique ID for this component. Do not change this ID after release.
    /// </summary>
    public override Guid ComponentGuid
    {
      get { return new Guid("{1816a17e-1a9e-411b-9fb8-67095eb99325}"); }
    }
  }
}