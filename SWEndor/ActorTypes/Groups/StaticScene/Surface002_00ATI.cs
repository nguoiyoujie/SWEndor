using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class Surface002_00ATI : StaticSceneGroup
  {
    private static Surface002_00ATI _instance;
    public static Surface002_00ATI Instance()
    {
      if (_instance == null) { _instance = new Surface002_00ATI(); }
      return _instance;
    }

    private Surface002_00ATI() : base("Surface002_00ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 200;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_00.x");
    }
  }
}

