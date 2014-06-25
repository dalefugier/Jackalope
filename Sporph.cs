using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Geometry.Morphs;

namespace Jackalope
{
  public class Sporph : MorphComponent
  {
    private int m_ge_index = -1; // geometry
    private int m_s0_index = -1; // base surface
    private int m_p0_index = -1; // base surface uv
    private int m_s1_index = -1; // target surface
    private int m_p1_index = -1; // target surface uv
    private int m_ps_index = -1; // preserve structure
    private int m_qp_index = -1; // quick preview
    
    /// <summary>
    /// Initializes a new instance of the Sporph class.
    /// </summary>
    public Sporph()
      : base("Sporph", "Sporph", "Deforms an object from a source surface to a target surface")
    {
    }

    /// <summary>
    /// Registers all the input parameters for this component.
    /// </summary>
    protected override void RegisterInputParams(GH_Component.GH_InputParamManager PM)
    {
      m_ge_index = PM.AddGeometryParameter("Geometry", "G", "Base geometry", GH_ParamAccess.list);
      m_s0_index = PM.AddSurfaceParameter("Base", "S0", "Base surface", GH_ParamAccess.item);
      m_p0_index = PM.AddPointParameter("Parameter", "uv0", "U,V parameter on base surface used for orienting. If unset, then surface origin is used.", GH_ParamAccess.item, Rhino.Geometry.Point3d.Unset);
      m_s1_index = PM.AddSurfaceParameter("Target", "S1", "Target surface", GH_ParamAccess.item);
      m_p1_index = PM.AddPointParameter("Parameter", "uv1", "U,V parameter on target surface used for orienting. If unset, then surface origin is used.", GH_ParamAccess.item, Rhino.Geometry.Point3d.Unset);
      m_ps_index = PM.AddBooleanParameter("Preserve", "V", "If true, preserves the control point structure of the surface. If false, geometry is refit as needed with more control points to allow accurate deformation.", GH_ParamAccess.item, false);
      m_qp_index = PM.AddBooleanParameter("Quick", "Q", "If true, morph should be done as quickly as possible. If false, morph should be done as accurately as possible", GH_ParamAccess.item, false);

      PM[m_p0_index].Optional = true;
      PM[m_p1_index].Optional = true;
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
    protected override void SolveInstance(IGH_DataAccess DA)
    {
      List<GeometryBase> geometry = new List<GeometryBase>();
      Surface surface0 = null;
      Point3d point0 = Point3d.Unset;
      Surface surface1 = null;
      Point3d point1 = Point3d.Unset;
      bool bPreserveStructure = false;
      bool bQuickPreview = false;

      if (!DA.GetDataList(m_ge_index, geometry) || 0 == geometry.Count)
        return;
      if (!DA.GetData(m_s0_index, ref surface0))
        return;
      if (!DA.GetData(m_p0_index, ref point0))
        return;
      if (!DA.GetData(m_s1_index, ref surface1))
        return;
      if (!DA.GetData(m_p1_index, ref point1))
        return;
      if (!DA.GetData(m_ps_index, ref bPreserveStructure))
        return;
      if (!DA.GetData(m_qp_index, ref bQuickPreview))
        return;

      SporphSpaceMorph morph = new SporphSpaceMorph(surface0, surface1, new Point2d(point0), new Point2d(point1));
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
      get { return Jackalope.Properties.Resources.Sporph; }
    }

    /// <summary>
    /// Gets the unique ID for this component. Do not change this ID after release.
    /// </summary>
    public override Guid ComponentGuid
    {
      get { return new Guid("{59294139-c035-4442-bf95-f2028132ddd6}"); }
    }
  }
}