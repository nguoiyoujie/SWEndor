using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class Surface002_12ATI : StaticSceneGroup
  {
    private static Surface002_12ATI _instance;
    public static Surface002_12ATI Instance()
    {
      if (_instance == null) { _instance = new Surface002_12ATI(); }
      return _instance;
    }

    private Surface002_12ATI() : base("Surface002_12ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 200;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_12.x");
    }
  }
}

