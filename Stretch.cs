using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
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
    protected override void RegisterInputParams(GH_InputParamManager pm)
    {
      m_ge_index = pm.AddGeometryParameter("Geometry", "G", "Base geometry", GH_ParamAccess.list);
      m_ax_index = pm.AddLineParameter("Axis", "X", "Stretch axis", GH_ParamAccess.item);
      m_pt_index = pm.AddPointParameter("Point", "P", "End point of new stretch axis. If unset, stretch factor is used.", GH_ParamAccess.item, Point3d.Unset);
      m_sf_index = pm.AddNumberParameter("Length", "L", "Length of new stretch axis. If unset, point is used.", GH_ParamAccess.item, Rhino.RhinoMath.UnsetValue);
      m_ps_index = pm.AddBooleanParameter("Preserve", "V", "If true, preserves the control point structure of the surface. If false, geometry is refit as needed with more control points to allow accurate deformation.", GH_ParamAccess.item, false);
      m_qp_index = pm.AddBooleanParameter("Quick", "Q", "If true, morph should be done as quickly as possible. If false, morph should be done as accurately as possible", GH_ParamAccess.item, false);

      pm[m_pt_index].Optional = true;
      pm[m_sf_index].Optional = true;
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
      var point = Point3d.Unset;
      var stretch_factor = Rhino.RhinoMath.UnsetValue;
      var preserve_structure = false;
      var quick_preview = false;

      if (!da.GetDataList(m_ge_index, geometry) || 0 == geometry.Count)
        return;
      if (!da.GetData(m_ax_index, ref axis))
        return;
      if (!da.GetData(m_pt_index, ref point))
        return;
      if (!da.GetData(m_sf_index, ref stretch_factor))
        return;
      if (!da.GetData(m_ps_index, ref preserve_structure))
        return;
      if (!da.GetData(m_qp_index, ref quick_preview))
        return;

      StretchSpaceMorph morph = point.IsValid ?
        new StretchSpaceMorph(axis.From, axis.To, point) : 
        new StretchSpaceMorph(axis.From, axis.To, stretch_factor);
      morph.PreserveStructure = preserve_structure;
      morph.QuickPreview = quick_preview;

      var output = geometry.Select(geom => geom.DuplicateGeometry().Morph(morph)).ToList();
      da.SetDataList(0, output);
    }

    /// <summary>
    /// Provides an Icon for the component.
    /// </summary>
    protected override System.Drawing.Bitmap Icon
    {
      get { return Properties.Resources.Stretch; }
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