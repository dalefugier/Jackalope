using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Geometry.Morphs;

namespace Jackalope
{
  public class Taper : MorphComponent
  {
    private int m_ge_index = -1; // geometry
    private int m_ax_index = -1; // axis
    private int m_r0_index = -1; // start radius
    private int m_r1_index = -1; // end radius
    private int m_fl_index = -1; // flat
    private int m_it_index = -1; // infinite
    private int m_ps_index = -1; // preserve structure
    private int m_qp_index = -1; // quick preview

    /// <summary>
    /// Initializes a new instance of the Taper class.
    /// </summary>
    public Taper()
      : base("Taper", "Taper", "Deforms objects toward or away from a specified axis")
    {
    }

    /// <summary>
    /// Registers all the input parameters for this component.
    /// </summary>
    protected override void RegisterInputParams(GH_Component.GH_InputParamManager PM)
    {
      m_ge_index = PM.AddGeometryParameter("Geometry", "G", "Base geometry", GH_ParamAccess.list);
      m_ax_index = PM.AddLineParameter("Axis", "X", "Taper axis", GH_ParamAccess.item);
      m_r0_index = PM.AddNumberParameter("Start", "R0", "Radius at start of taper axis", GH_ParamAccess.item);
      m_r1_index = PM.AddNumberParameter("End", "R1", "Radius at end of taper axis", GH_ParamAccess.item);
      m_fl_index = PM.AddBooleanParameter("Flat", "F", "If true, then a one-directional, one-dimensional taper is created.", GH_ParamAccess.item, false);
      m_it_index = PM.AddBooleanParameter("Infinite", "I", "If true, the deformation happens throughout the geometry, even if the axis is shorter. If false, the deformation takes place only the length of the axis.", GH_ParamAccess.item, false);
      m_ps_index = PM.AddBooleanParameter("Preserve", "V", "If true, preserves the control point structure of the surface. If false, geometry is refit as needed with more control points to allow accurate deformation.", GH_ParamAccess.item, false);
      m_qp_index = PM.AddBooleanParameter("Quick", "Q", "If true, morph should be done as quickly as possible. If false, morph should be done as accurately as possible", GH_ParamAccess.item, false);

      PM[m_fl_index].Optional = true;
      PM[m_it_index].Optional = true;
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
      double radius0 = Rhino.RhinoMath.UnsetValue;
      double radius1 = Rhino.RhinoMath.UnsetValue;
      bool bFlat = false;
      bool bInfiniteTaper = false;
      bool bPreserveStructure = false;
      bool bQuickPreview = false;

      if (!DA.GetDataList(m_ge_index, geometry) || 0 == geometry.Count)
        return;
      if (!DA.GetData(m_ax_index, ref axis))
        return;
      if (!DA.GetData(m_r0_index, ref radius0))
        return;
      if (!DA.GetData(m_r1_index, ref radius1))
        return;
      if (!DA.GetData(m_fl_index, ref bFlat))
        return;
      if (!DA.GetData(m_it_index, ref bInfiniteTaper))
        return;
      if (!DA.GetData(m_ps_index, ref bPreserveStructure))
        return;
      if (!DA.GetData(m_qp_index, ref bQuickPreview))
        return;

      TaperSpaceMorph morph = new TaperSpaceMorph(axis.From, axis.To, radius0, radius1, bFlat, bInfiniteTaper);
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
      get { return Jackalope.Properties.Resources.Taper; }
    }

    /// <summary>
    /// Gets the unique ID for this component. Do not change this ID after release.
    /// </summary>
    public override Guid ComponentGuid
    {
      get { return new Guid("{637f6100-4287-4c0b-a4a1-1111a9532d23}"); }
    }
  }
}