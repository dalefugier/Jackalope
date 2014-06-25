using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Geometry.Morphs;

namespace Jackalope
{
  public class Stretch : MorphComponent
  {
    private int m_ge_index = -1; // geometry
    private int m_ax_index = -1; // axis
    private int m_pt_index = -1; // point
    private int m_sf_index = -1; // stretch factor
    private int m_ps_index = -1; // preserve structure
    private int m_qp_index = -1; // quick preview

    /// <summary>
    /// Initializes a new instance of the Stretch class.
    /// </summary>
    public Stretch()
      : base("Stretch", "Stretch", "Deforms objects toward or away from a specified axis.")
    {
    }

    /// <summary>
    /// Registers all the input parameters for this component.
    /// </summary>
    protected override void RegisterInputParams(GH_Component.GH_InputParamManager PM)
    {
      m_ge_index = PM.AddGeometryParameter("Geometry", "G", "Base geometry", GH_ParamAccess.list);
      m_ax_index = PM.AddLineParameter("Axis", "X", "Stretch axis", GH_ParamAccess.item);
      m_pt_index = PM.AddPointParameter("Point", "P", "End point of new stretch axis. If unset, stretch factor is used.", GH_ParamAccess.item, Rhino.Geometry.Point3d.Unset);
      m_sf_index = PM.AddNumberParameter("Length", "L", "Length of new stretch axis. If unset, point is used.", GH_ParamAccess.item, Rhino.RhinoMath.UnsetValue);
      m_ps_index = PM.AddBooleanParameter("Preserve", "V", "If true, preserves the control point structure of the surface. If false, geometry is refit as needed with more control points to allow accurate deformation.", GH_ParamAccess.item, false);
      m_qp_index = PM.AddBooleanParameter("Quick", "Q", "If true, morph should be done as quickly as possible. If false, morph should be done as accurately as possible", GH_ParamAccess.item, false);

      PM[m_pt_index].Optional = true;
      PM[m_sf_index].Optional = true;
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
      Line axis = Line.Unset;
      Point3d point = Point3d.Unset;
      double stretch_factor = Rhino.RhinoMath.UnsetValue;
      bool bPreserveStructure = false;
      bool bQuickPreview = false;

      if (!DA.GetDataList(m_ge_index, geometry) || 0 == geometry.Count)
        return;
      if (!DA.GetData(m_ax_index, ref axis))
        return;
      if (!DA.GetData(m_pt_index, ref point))
        return;
      if (!DA.GetData(m_sf_index, ref stretch_factor))
        return;
      if (!DA.GetData(m_ps_index, ref bPreserveStructure))
        return;
      if (!DA.GetData(m_qp_index, ref bQuickPreview))
        return;

      StretchSpaceMorph morph = null;
      if (point.IsValid)
        morph = new StretchSpaceMorph(axis.From, axis.To, point);
      else 
        morph = new StretchSpaceMorph(axis.From, axis.To, stretch_factor);
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
      get { return Jackalope.Properties.Resources.Stretch; }
    }

    /// <summary>
    /// Gets the unique ID for this component. Do not change this ID after release.
    /// </summary>
    public override Guid ComponentGuid
    {
      get { return new Guid("{5629c523-90d7-4424-8094-3898d25d2d62}"); }
    }
  }
}