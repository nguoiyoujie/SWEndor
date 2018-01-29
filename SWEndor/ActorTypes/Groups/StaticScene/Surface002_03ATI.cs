using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class Surface002_03ATI : StaticSceneGroup
  {
    private static Surface002_03ATI _instance;
    public static Surface002_03ATI Instance()
    {
      if (_instance == null) { _instance = new Surface002_03ATI(); }
      return _instance;
    }

    private Surface002_03ATI() : base("Surface002_03ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 200;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_03.x");
    }
  }
}

