using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
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
    protected override void RegisterInputParams(GH_Component.GH_InputParamManager PM)
    {
      m_ge_index = PM.AddGeometryParameter("Geometry", "G", "Base geometry", GH_ParamAccess.list);
      m_pl_index = PM.AddPlaneParameter("Plane", "P", "Source plane of deformation", GH_ParamAccess.item);
      m_sf_index = PM.AddSurfaceParameter("Surface", "S", "Surface to wrap geometry onto", GH_ParamAccess.item);
      m_pt_index = PM.AddPointParameter("Parameter", "uv", "U,V parameter on surface used for orienting", GH_ParamAccess.item);
      m_sc_index = PM.AddNumberParameter("Factor", "F", "Scale factor", GH_ParamAccess.item, Rhino.RhinoMath.UnsetValue);
      m_an_index = PM.AddNumberParameter("Angle", "A", "Rotation angle in radians", GH_ParamAccess.item, Rhino.RhinoMath.UnsetValue);
      m_ps_index = PM.AddBooleanParameter("Preserve", "V", "If true, preserves the control point structure of the surface. If false, geometry is refit as needed with more control points to allow accurate deformation.", GH_ParamAccess.item, false);
      m_qp_index = PM.AddBooleanParameter("Quick", "Q", "If true, morph should be done as quickly as possible. If false, morph should be done as accurately as possible", GH_ParamAccess.item, false);

      PM[m_pl_index].Optional = true;
      PM[m_sc_index].Optional = true;
      PM[m_an_index].Optional = true;
      PM[m_ps_index].Optional = true;
      PM[m_qp_index].Optional = true;
    }

    /// <summary>
    /// Registers all the output parameters for this component.
    /// </summary>
    protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager PM)
    {
      PM.AddGeometryParameter("Geometry", "G", "Morphed geometry", GH_ParamAccess.list);
    }

    /// <summary>
    /// This is the method that actually does the work.
    /// </summary>
    /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
    protected override void SolveInstance(IGH_DataAccess DA)
    {
      List<GeometryBase> geometry = new List<GeometryBase>();
      Plane plane = Plane.WorldXY;
      Surface surface = null;
      Point3d point = Point3d.Unset;
      double scale_factor = Rhino.RhinoMath.UnsetValue;
      double angle = Rhino.RhinoMath.UnsetValue;
      bool bPreserveStructure = false;
      bool bQuickPreview = false;

      if (!DA.GetDataList(m_ge_index, geometry) || 0 == geometry.Count)
        return;
      if (!DA.GetData(m_pl_index, ref plane))
        return;
      if (!DA.GetData(m_sf_index, ref surface))
        return;
      if (!DA.GetData(m_pt_index, ref point))
        return;
      if (!DA.GetData(m_sc_index, ref scale_factor))
        return;
      if (!DA.GetData(m_an_index, ref angle))
        return;
      if (!DA.GetData(m_ps_index, ref bPreserveStructure))
        return;
      if (!DA.GetData(m_qp_index, ref bQuickPreview))
        return;

      SplopSpaceMorph morph = new SplopSpaceMorph(plane, surface, new Point2d(point), scale_factor, angle);
      morph.PreserveStructure = bPreserveStructure;
      morph.QuickPreview = bQuickPreview;

      List<GeometryBase> output = new List<GeometryBase>();
      foreach (GeometryBase geom in geometry)
        output.Add(MorphGeometry(morph, geom));

      DA.SetDataList(0, output);
    }

    /// <summary>
    /// Provides an Icon for the component.
    /// </summary>
    protected override System.Drawing.Bitmap Icon
    {
      get { return Jackalope.Properties.Resources.Splop; }
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