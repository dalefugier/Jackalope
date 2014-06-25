using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Geometry.Morphs;

namespace Jackalope
{
  public class Maelstrom : MorphComponent
  {
    private int m_ge_index = -1; // geometry
    private int m_pl_index = -1; // plane
    private int m_r0_index = -1; // first radius
    private int m_r1_index = -1; // second radius
    private int m_an_index = -1; // angle
    private int m_ps_index = -1; // preserve structure
    private int m_qp_index = -1; // quick preview

    /// <summary>
    /// Initializes a new instance of the Maelstrom class.
    /// </summary>
    public Maelstrom()
      : base("Maelstrom", "Maelstrom", "Deforms objects in a spiral as if they were caught in a whirlpool")
    {
    }

    /// <summary>
    /// Registers all the input parameters for this component.
    /// </summary>
    protected override void RegisterInputParams(GH_Component.GH_InputParamManager PM)
    {
      m_ge_index = PM.AddGeometryParameter("Geometry", "G", "Base geometry", GH_ParamAccess.list);
      m_pl_index = PM.AddPlaneParameter("Plane", "P", "Plane on which the base circle will lie. Origin of the plane will be the center point of the circle.", GH_ParamAccess.item, Rhino.Geometry.Plane.WorldXY);
      m_r0_index = PM.AddNumberParameter("First", "R0", "First radius", GH_ParamAccess.item);
      m_r1_index = PM.AddNumberParameter("Second", "R1", "Second radius", GH_ParamAccess.item);
      m_an_index = PM.AddAngleParameter("Angle", "A", "Coil angle in radians", GH_ParamAccess.item);
      m_ps_index = PM.AddBooleanParameter("Preserve", "V", "If true, preserves the control point structure of the surface. If false, geometry is refit as needed with more control points to allow accurate deformation.", GH_ParamAccess.item, false);
      m_qp_index = PM.AddBooleanParameter("Quick", "Q", "If true, morph should be done as quickly as possible. If false, morph should be done as accurately as possible", GH_ParamAccess.item, false);

      PM[m_pl_index].Optional = true;
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
      Plane plane = Plane.WorldXY;
      double radius0 = Rhino.RhinoMath.UnsetValue;
      double radius1 = Rhino.RhinoMath.UnsetValue;
      double angle = Rhino.RhinoMath.UnsetValue;
      bool bPreserveStructure = false;
      bool bQuickPreview = false;

      if (!DA.GetDataList(m_ge_index, geometry) || 0 == geometry.Count)
        return;
      if (!DA.GetData(m_pl_index, ref plane))
        return;
      if (!DA.GetData(m_r0_index, ref radius0))
        return;
      if (!DA.GetData(m_r1_index, ref radius1))
        return;
      if (!DA.GetData(m_an_index, ref angle))
        return;
      if (!DA.GetData(m_ps_index, ref bPreserveStructure))
        return;
      if (!DA.GetData(m_qp_index, ref bQuickPreview))
        return;

      MaelstromSpaceMorph morph = new MaelstromSpaceMorph(plane, radius0, radius1, angle);
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
      get { return Jackalope.Properties.Resources.Maelstrom; }
    }

    /// <summary>
    /// Gets the unique ID for this component. Do not change this ID after release.
    /// </summary>
    public override Guid ComponentGuid
    {
      get { return new Guid("{b7376804-d5ae-4f14-bce3-5302753a7081}"); }
    }
  }
}