using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Jackalope
{
  public class JackalopeInfo : GH_AssemblyInfo
  {
    public override string Name
    {
      get
      {
        return "Jackalope";
      }
    }
    
    public override Bitmap Icon
    {
      get { return Jackalope.Properties.Resources.Jackalope; }
    }

    public override string Description
    {
      get
      {
        return "Jackalope component for Grasshopper";
      }
    }
    public override Guid Id
    {
      get
      {
        return new Guid("85a3bd90-e6c1-4ce2-a50c-bfaaa24b8df0");
      }
    }

    public override string AuthorName
    {
      get
      {
        return "Robert McNeel & Associates";
      }
    }

    public override string AuthorContact
    {
      get
      {
        return "http://www.rhino3d.com";
      }
    }
  }
}
