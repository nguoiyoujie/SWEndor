using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class Surface002_02ATI : StaticSceneGroup
  {
    private static Surface002_02ATI _instance;
    public static Surface002_02ATI Instance()
    {
      if (_instance == null) { _instance = new Surface002_02ATI(); }
      return _instance;
    }

    private Surface002_02ATI() : base("Surface002_02ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 200;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_02.x");
    }
  }
}

