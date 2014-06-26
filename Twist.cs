using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Geometry.Morphs;

namespace Jackalope
{
  public class Twist : MorphComponent
  {
    private int m_ge_index = -1; // geometry
    private int m_ax_index = -1; // axis
    private int m_an_index = -1; // angle
    private int m_it_index = -1; // infinite twist
    private int m_ps_index = -1; // preserve structure
    private int m_qp_index = -1; // quick preview

    /// <summary>
    /// Initializes a new instance of the Twist class.
    /// </summary>
    public Twist()
      : base("Twist", "Twist", "Deforms objects by rotating them around an axis.")
    {
    }

    /// <summary>
    /// Registers all the input parameters for this component.
    /// </summary>
    protected override void RegisterInputParams(GH_Component.GH_InputParamManager PM)
    {
      m_ge_index = PM.AddGeometryParameter("Geometry", "G", "Base geometry", GH_ParamAccess.list);
      m_ax_index = PM.AddLineParameter("Axis", "X", "Twist axis", GH_ParamAccess.item);
      m_an_index = PM.AddAngleParameter("Angle", "A", "Twist angle in radians", GH_ParamAccess.item);
      m_it_index = PM.AddBooleanParameter("Infinite", "I", "If true, the deformation is constant throughout the object, even if the axis is shorter than the object. If false, the deformation takes place only the length of the axis.", GH_ParamAccess.item, false);
      m_ps_index = PM.AddBooleanParameter("Preserve", "V", "If true, preserves the control point structure of the surface. If false, geometry is refit as needed with more control points to allow accurate deformation.", GH_ParamAccess.item, false);
      m_qp_index = PM.AddBooleanParameter("Quick", "Q", "If true, morph should be done as quickly as possible. If false, morph should be done as accurately as possible", GH_ParamAccess.item, false);

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
      double angle = Rhino.RhinoMath.UnsetValue;
      bool bInfiniteTwist = false;
      bool bPreserveStructure = false;
      bool bQuickPreview = false;

      if (!DA.GetDataList(m_ge_index, geometry) || 0 == geometry.Count)
        return;
      if (!DA.GetData(m_ax_index, ref axis))
        return;
      if (!DA.GetData(m_an_index, ref angle))
        return;
      if (!DA.GetData(m_it_index, ref bInfiniteTwist))
        return;
      if (!DA.GetData(m_ps_index, ref bPreserveStructure))
        return;
      if (!DA.GetData(m_qp_index, ref bQuickPreview))
        return;

      TwistSpaceMorph morph = new TwistSpaceMorph();
      morph.TwistAxis = axis;
      morph.TwistAngleRadians = angle;
      morph.InfiniteTwist = bInfiniteTwist;
      morph.PreserveStructure = bPreserveStructure;
      morph.QuickPreview = bQuickPreview;

      List<GeometryBase> output = new List<GeometryBase>();
      foreach (GeometryBase geom in geometry)
        output.Add(MorphGeometry(morph, geom));

      DA.SetDataList(0, output);
    }

    /// <summary>
    /// Provides an Icon for every component that will be visible in the User Interface.
    /// Icons need to be 24x24 pixels.
    /// </summary>
    protected override System.Drawing.Bitmap Icon
    {
      get { return Jackalope.Properties.Resources.Twist; }
    }

    /// <summary>
    /// Each component must have a unique Guid to identify it. 
    /// It is vital this Guid doesn't change otherwise old ghx files 
    /// that use the old ID will partially fail during loading.
    /// </summary>
    public override Guid ComponentGuid
    {
      get { return new Guid("{1874c9cb-792e-4bf2-8959-4c41e9f7b3d5}"); }
    }
  }
}
