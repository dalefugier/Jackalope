using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
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
    protected override void RegisterInputParams(GH_Component.GH_InputParamManager PM)
    {
      m_ge_index = PM.AddGeometryParameter("Geometry", "G", "Base geometry", GH_ParamAccess.list);
      m_ax_index = PM.AddLineParameter("Axis", "X", "Axis that represents the original orientation of the geometry", GH_ParamAccess.item);
      m_pt_index = PM.AddPointParameter("Point", "P", "Point to bend through", GH_ParamAccess.item);
      m_an_index = PM.AddAngleParameter("Angle", "A", "Bend angle in radians. If unset, the point is used for bend direction.", GH_ParamAccess.item, Rhino.RhinoMath.UnsetValue);
      m_st_index = PM.AddBooleanParameter("Straight", "S", "If true, only the spine region is bent. If false, then the point determines the region to bend.", GH_ParamAccess.item, false);
      m_sy_index = PM.AddBooleanParameter("Symmetric", "Y", "If true, then the object will bend symmetrically around the center if you start the spine in the middle of the object. If false, then only one end of the object bends.", GH_ParamAccess.item, false);
      m_ps_index = PM.AddBooleanParameter("Preserve", "V", "If true, preserves the control point structure of the surface. If false, geometry is refit as needed with more control points to allow accurate deformation.", GH_ParamAccess.item, false);
      m_qp_index = PM.AddBooleanParameter("Quick", "Q", "If true, morph should be done as quickly as possible. If false, morph should be done as accurately as possible", GH_ParamAccess.item, false);

      PM[m_an_index].Optional = true;
      PM[m_st_index].Optional = true;
      PM[m_sy_index].Optional = true;
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
      double angle = Rhino.RhinoMath.UnsetValue;
      bool bStraight = false;
      bool bSymmetric = false;
      bool bPreserveStructure = false;
      bool bQuickPreview = false;

      if (!DA.GetDataList(m_ge_index, geometry) || 0 == geometry.Count)
        return;
      if (!DA.GetData(m_ax_index, ref axis))
        return;
      if (!DA.GetData(m_pt_index, ref point))
        return;
      if (!DA.GetData(m_an_index, ref angle))
        return;
      if (!DA.GetData(m_st_index, ref bStraight))
        return;
      if (!DA.GetData(m_sy_index, ref bSymmetric))
        return;
      if (!DA.GetData(m_ps_index, ref bPreserveStructure))
        return;
      if (!DA.GetData(m_qp_index, ref bQuickPreview))
        return;

      BendSpaceMorph morph = null;
      if (Rhino.RhinoMath.IsValidDouble(angle))
        morph = new BendSpaceMorph(axis.From, axis.To, point, angle, bStraight, bSymmetric);
      else
        morph = new BendSpaceMorph(axis.From, axis.To, point, bStraight, bSymmetric);
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
      get { return Jackalope.Properties.Resources.Bend; }
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