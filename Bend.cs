using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino.Geometry.Morphs;

namespace Jackalope
{
  public class Bend : MorphComponent
  {
    private int m_ge_index = -1; // geometry
    private int m_ax_index = -1; // axis
    private int m_pt_index = -1; // point
    private int m_an_index = -1; // angle
    private int m_st_index = -1; // straight
    private int m_sy_index = -1; // symmetric
    private int m_ps_index = -1; // preserve structure
    private int m_qp_index = -1; // quick preview

    /// <summary>
    /// Initializes a new instance of the Bend class.
    /// </summary>
    public Bend()
      : base("Bend", "Bend", "Deforms objects by bending along a spine arc.")
    {
    }

    /// <summary>
    /// Registers all the input parameters for this component.
    /// </summary>
    protected override void RegisterInputParams(GH_InputParamManager pm)
    {
      m_ge_index = pm.AddGeometryParameter("Geometry", "G", "Base geometry", GH_ParamAccess.list);
      m_ax_index = pm.AddLineParameter("Axis", "X", "Axis that represents the original orientation of the geometry", GH_ParamAccess.item);
      m_pt_index = pm.AddPointParameter("Point", "P", "Point to bend through", GH_ParamAccess.item);
      m_an_index = pm.AddAngleParameter("Angle", "A", "Bend angle in radians. If unset, the point is used for bend direction.", GH_ParamAccess.item, Rhino.RhinoMath.UnsetValue);
      m_st_index = pm.AddBooleanParameter("Straight", "S", "If true, only the spine region is bent. If false, then the point determines the region to bend.", GH_ParamAccess.item, false);
      m_sy_index = pm.AddBooleanParameter("Symmetric", "Y", "If true, then the object will bend symmetrically around the center if you start the spine in the middle of the object. If false, then only one end of the object bends.", GH_ParamAccess.item, false);
      m_ps_index = pm.AddBooleanParameter("Preserve", "V", "If true, preserves the control point structure of the surface. If false, geometry is refit as needed with more control points to allow accurate deformation.", GH_ParamAccess.item, false);
      m_qp_index = pm.AddBooleanParameter("Quick", "Q", "If true, morph should be done as quickly as possible. If false, morph should be done as accurately as possible", GH_ParamAccess.item, false);

      pm[m_an_index].Optional = true;
      pm[m_st_index].Optional = true;
      pm[m_sy_index].Optional = true;
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
      var angle = Rhino.RhinoMath.UnsetValue;
      var straight = false;
      var symmetric = false;
      var preserve_structure = false;
      var quick_preview = false;

      if (!da.GetDataList(m_ge_index, geometry) || 0 == geometry.Count)
        return;
      if (!da.GetData(m_ax_index, ref axis))
        return;
      if (!da.GetData(m_pt_index, ref point))
        return;
      if (!da.GetData(m_an_index, ref angle))
        return;
      if (!da.GetData(m_st_index, ref straight))
        return;
      if (!da.GetData(m_sy_index, ref symmetric))
        return;
      if (!da.GetData(m_ps_index, ref preserve_structure))
        return;
      if (!da.GetData(m_qp_index, ref quick_preview))
        return;

      var morph = Rhino.RhinoMath.IsValidDouble(angle) ? 
        new BendSpaceMorph(axis.From, axis.To, point, angle, straight, symmetric) : 
        new BendSpaceMorph(axis.From, axis.To, point, straight, symmetric);
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
      get { return Properties.Resources.Bend; }
    }

    /// <summary>
    /// Gets the unique ID for this component. Do not change this ID after release.
    /// </summary>
    public override Guid ComponentGuid
    {
      get { return new Guid("{7277e12c-a464-46ab-b44d-6fc9ed2de31f}"); }
    }
  }
}