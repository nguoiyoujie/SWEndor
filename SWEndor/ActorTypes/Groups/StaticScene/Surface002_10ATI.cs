using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class Surface002_10ATI : StaticSceneGroup
  {
    private static Surface002_10ATI _instance;
    public static Surface002_10ATI Instance()
    {
      if (_instance == null) { _instance = new Surface002_10ATI(); }
      return _instance;
    }

    private Surface002_10ATI() : base("Surface002_10ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 200;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_10.x");
    }
  }
}

