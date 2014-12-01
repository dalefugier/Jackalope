using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
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
    protected override void RegisterInputParams(GH_InputParamManager pm)
    {
      m_ge_index = pm.AddGeometryParameter("Geometry", "G", "Base geometry", GH_ParamAccess.list);
      m_c0_index = pm.AddCurveParameter("Base", "C0", "Base curve", GH_ParamAccess.item);
      m_c1_index = pm.AddCurveParameter("Target", "C1", "Target curve", GH_ParamAccess.item);
      m_r0_index = pm.AddBooleanParameter("Reverse Base", "R0", "If true, then direction of the base curve is reversed.", GH_ParamAccess.item, false);
      m_r1_index = pm.AddBooleanParameter("Reverse Target", "R1", "If true, then direction of the target curve is reversed.", GH_ParamAccess.item, false);
      m_pr_index = pm.AddBooleanParameter("Prevent", "X", "If true, the length of the objects along the curve directions are not changed. If false, objects are stretched or compressed in the curve direction so that the relationship to the target curve is the same as it is to the base curve.", GH_ParamAccess.item, false);
      m_ps_index = pm.AddBooleanParameter("Preserve", "V", "If true, preserves the control point structure of the surface. If false, geometry is refit as needed with more control points to allow accurate deformation.", GH_ParamAccess.item, false);
      m_qp_index = pm.AddBooleanParameter("Quick", "Q", "If true, morph should be done as quickly as possible. If false, morph should be done as accurately as possible", GH_ParamAccess.item, false);

      pm[m_r0_index].Optional = true;
      pm[m_r1_index].Optional = true;
      pm[m_pr_index].Optional = true;
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
    protected override void SolveInstance(IGH_DataAccess da)
    {
      var geometry = new List<IGH_GeometricGoo>();
      Curve curve0 = null;
      Curve curve1 = null;
      var reverse0 = false;
      var reverse1 = false;
      var prevent_stretching = false;
      var preserve_structure = false;
      var quick_preview = false;

      if (!da.GetDataList(m_ge_index, geometry) || 0 == geometry.Count)
        return;
      if (!da.GetData(m_c0_index, ref curve0))
        return;
      if (!da.GetData(m_c1_index, ref curve1))
        return;
      if (!da.GetData(m_r0_index, ref reverse0))
        return;
      if (!da.GetData(m_r1_index, ref reverse1))
        return;
      if (!da.GetData(m_pr_index, ref prevent_stretching))
        return;
      if (!da.GetData(m_ps_index, ref preserve_structure))
        return;
      if (!da.GetData(m_qp_index, ref quick_preview))
        return;

      var morph = new FlowSpaceMorph(curve0, curve1, reverse0, reverse1, prevent_stretching)
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
      get { return Properties.Resources.Flow; }
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