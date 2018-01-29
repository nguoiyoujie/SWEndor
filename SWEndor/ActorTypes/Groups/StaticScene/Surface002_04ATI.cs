using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class Surface002_04ATI : StaticSceneGroup
  {
    private static Surface002_04ATI _instance;
    public static Surface002_04ATI Instance()
    {
      if (_instance == null) { _instance = new Surface002_04ATI(); }
      return _instance;
    }

    private Surface002_04ATI() : base("Surface002_04ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 200;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_04.x");
    }
  }
}

