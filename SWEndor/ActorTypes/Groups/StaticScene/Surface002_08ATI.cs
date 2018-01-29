using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class Surface002_08ATI : StaticSceneGroup
  {
    private static Surface002_08ATI _instance;
    public static Surface002_08ATI Instance()
    {
      if (_instance == null) { _instance = new Surface002_08ATI(); }
      return _instance;
    }

    private Surface002_08ATI() : base("Surface002_08ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 200;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_08.x");
    }
  }
}

