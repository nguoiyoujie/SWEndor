using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class Surface002_07ATI : StaticSceneGroup
  {
    private static Surface002_07ATI _instance;
    public static Surface002_07ATI Instance()
    {
      if (_instance == null) { _instance = new Surface002_07ATI(); }
      return _instance;
    }

    private Surface002_07ATI() : base("Surface002_07ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 200;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_07.x");
    }
  }
}

