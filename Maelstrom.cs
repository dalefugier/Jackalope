using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
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
    protected override void RegisterInputParams(GH_InputParamManager pm)
    {
      m_ge_index = pm.AddGeometryParameter("Geometry", "G", "Base geometry", GH_ParamAccess.list);
      m_pl_index = pm.AddPlaneParameter("Plane", "P", "Plane on which the base circle will lie. Origin of the plane will be the center point of the circle.", GH_ParamAccess.item, Plane.WorldXY);
      m_r0_index = pm.AddNumberParameter("First", "R0", "First radius", GH_ParamAccess.item);
      m_r1_index = pm.AddNumberParameter("Second", "R1", "Second radius", GH_ParamAccess.item);
      m_an_index = pm.AddAngleParameter("Angle", "A", "Coil angle in radians", GH_ParamAccess.item);
      m_ps_index = pm.AddBooleanParameter("Preserve", "V", "If true, preserves the control point structure of the surface. If false, geometry is refit as needed with more control points to allow accurate deformation.", GH_ParamAccess.item, false);
      m_qp_index = pm.AddBooleanParameter("Quick", "Q", "If true, morph should be done as quickly as possible. If false, morph should be done as accurately as possible", GH_ParamAccess.item, false);

      pm[m_pl_index].Optional = true;
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
      var plane = Plane.WorldXY;
      var radius0 = Rhino.RhinoMath.UnsetValue;
      var radius1 = Rhino.RhinoMath.UnsetValue;
      var angle = Rhino.RhinoMath.UnsetValue;
      var preserve_structure = false;
      var quick_preview = false;

      if (!da.GetDataList(m_ge_index, geometry) || 0 == geometry.Count)
        return;
      if (!da.GetData(m_pl_index, ref plane))
        return;
      if (!da.GetData(m_r0_index, ref radius0))
        return;
      if (!da.GetData(m_r1_index, ref radius1))
        return;
      if (!da.GetData(m_an_index, ref angle))
        return;
      if (!da.GetData(m_ps_index, ref preserve_structure))
        return;
      if (!da.GetData(m_qp_index, ref quick_preview))
        return;

      var morph = new MaelstromSpaceMorph(plane, radius0, radius1, angle)
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
      get { return Properties.Resources.Maelstrom; }
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