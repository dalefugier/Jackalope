using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Geometry.Morphs;

namespace Jackalope
{
  public class Flow : MorphComponent
  {
    private int m_ge_index = -1; // geometry
    private int m_c0_index = -1; // base curve 
    private int m_c1_index = -1; // target curve
    private int m_r0_index = -1; // reverse base curve
    private int m_r1_index = -1; // reverse target curve
    private int m_pr_index = -1; // prevent stretching
    private int m_ps_index = -1; // preserve structure
    private int m_qp_index = -1; // quick preview

    /// <summary>
    /// Initializes a new instance of the Flow class.
    /// </summary>
    public Flow()
      : base("Flow", "Flow", "Re-aligns objects from a base curve to a target curve.")
    {
    }

    /// <summary>
    /// Registers all the input parameters for this component.
    /// </summary>
    protected override void RegisterInputParams(GH_Component.GH_InputParamManager PM)
    {
      m_ge_index = PM.AddGeometryParameter("Geometry", "G", "Base geometry", GH_ParamAccess.list);
      m_c0_index = PM.AddCurveParameter("Base", "C0", "Base curve", GH_ParamAccess.item);
      m_c1_index = PM.AddCurveParameter("Target", "C1", "Target curve", GH_ParamAccess.item);
      m_r0_index = PM.AddBooleanParameter("Reverse Base", "R0", "If true, then direction of the base curve is reversed.", GH_ParamAccess.item, false);
      m_r1_index = PM.AddBooleanParameter("Reverse Target", "R1", "If true, then direction of the target curve is reversed.", GH_ParamAccess.item, false);
      m_pr_index = PM.AddBooleanParameter("Prevent", "X", "If true, the length of the objects along the curve directions are not changed. If false, objects are stretched or compressed in the curve direction so that the relationship to the target curve is the same as it is to the base curve.", GH_ParamAccess.item, false);
      m_ps_index = PM.AddBooleanParameter("Preserve", "V", "If true, preserves the control point structure of the surface. If false, geometry is refit as needed with more control points to allow accurate deformation.", GH_ParamAccess.item, false);
      m_qp_index = PM.AddBooleanParameter("Quick", "Q", "If true, morph should be done as quickly as possible. If false, morph should be done as accurately as possible", GH_ParamAccess.item, false);

      PM[m_r0_index].Optional = true;
      PM[m_r1_index].Optional = true;
      PM[m_pr_index].Optional = true;
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
      Curve curve0 = null;
      Curve curve1 = null;
      bool bReverse0 = false;
      bool bReverse1 = false;
      bool bPreventStretching = false;
      bool bPreserveStructure = false;
      bool bQuickPreview = false;

      if (!DA.GetDataList(m_ge_index, geometry) || 0 == geometry.Count)
        return;
      if (!DA.GetData(m_c0_index, ref curve0))
        return;
      if (!DA.GetData(m_c1_index, ref curve1))
        return;
      if (!DA.GetData(m_r0_index, ref bReverse0))
        return;
      if (!DA.GetData(m_r1_index, ref bReverse1))
        return;
      if (!DA.GetData(m_pr_index, ref bPreventStretching))
        return;
      if (!DA.GetData(m_ps_index, ref bPreserveStructure))
        return;
      if (!DA.GetData(m_qp_index, ref bQuickPreview))
        return;

      FlowSpaceMorph morph = new FlowSpaceMorph(curve0, curve1, bReverse0, bReverse1, bPreventStretching);
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
      get { return Jackalope.Properties.Resources.Flow; }
    }

    /// <summary>
    /// Gets the unique ID for this component. Do not change this ID after release.
    /// </summary>
    public override Guid ComponentGuid
    {
      get { return new Guid("{2e526d22-8be1-4ffb-901c-b623b3bce0a0}"); }
    }
  }
}