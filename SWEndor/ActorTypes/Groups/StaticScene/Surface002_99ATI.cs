using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class Surface002_99ATI : StaticSceneGroup
  {
    private static Surface002_99ATI _instance;
    public static Surface002_99ATI Instance()
    {
      if (_instance == null) { _instance = new Surface002_99ATI(); }
      return _instance;
    }

    private Surface002_99ATI() : base("Surface002_99ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 200;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_99.x");
    }
  }
}

