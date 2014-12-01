using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
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
    protected override void RegisterInputParams(GH_InputParamManager pm)
    {
      m_ge_index = pm.AddGeometryParameter("Geometry", "G", "Base geometry", GH_ParamAccess.list);
      m_ax_index = pm.AddLineParameter("Axis", "X", "Taper axis", GH_ParamAccess.item);
      m_r0_index = pm.AddNumberParameter("Start", "R0", "Radius at start of taper axis", GH_ParamAccess.item);
      m_r1_index = pm.AddNumberParameter("End", "R1", "Radius at end of taper axis", GH_ParamAccess.item);
      m_fl_index = pm.AddBooleanParameter("Flat", "F", "If true, then a one-directional, one-dimensional taper is created.", GH_ParamAccess.item, false);
      m_it_index = pm.AddBooleanParameter("Infinite", "I", "If true, the deformation happens throughout the geometry, even if the axis is shorter. If false, the deformation takes place only the length of the axis.", GH_ParamAccess.item, false);
      m_ps_index = pm.AddBooleanParameter("Preserve", "V", "If true, preserves the control point structure of the surface. If false, geometry is refit as needed with more control points to allow accurate deformation.", GH_ParamAccess.item, false);
      m_qp_index = pm.AddBooleanParameter("Quick", "Q", "If true, morph should be done as quickly as possible. If false, morph should be done as accurately as possible", GH_ParamAccess.item, false);

      pm[m_fl_index].Optional = true;
      pm[m_it_index].Optional = true;
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
      var axis = Line.Unset;
      var radius0 = Rhino.RhinoMath.UnsetValue;
      var radius1 = Rhino.RhinoMath.UnsetValue;
      var flat = false;
      var infinite_taper = false;
      var preserve_structure = false;
      var quick_preview = false;

      if (!da.GetDataList(m_ge_index, geometry) || 0 == geometry.Count)
        return;
      if (!da.GetData(m_ax_index, ref axis))
        return;
      if (!da.GetData(m_r0_index, ref radius0))
        return;
      if (!da.GetData(m_r1_index, ref radius1))
        return;
      if (!da.GetData(m_fl_index, ref flat))
        return;
      if (!da.GetData(m_it_index, ref infinite_taper))
        return;
      if (!da.GetData(m_ps_index, ref preserve_structure))
        return;
      if (!da.GetData(m_qp_index, ref quick_preview))
        return;

      var morph = new TaperSpaceMorph(axis.From, axis.To, radius0, radius1, flat, infinite_taper)
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
      get { return Properties.Resources.Taper; }
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