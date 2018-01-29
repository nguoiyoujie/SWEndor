using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class Surface002_09ATI : StaticSceneGroup
  {
    private static Surface002_09ATI _instance;
    public static Surface002_09ATI Instance()
    {
      if (_instance == null) { _instance = new Surface002_09ATI(); }
      return _instance;
    }

    private Surface002_09ATI() : base("Surface002_09ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 200;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_09.x");
    }
  }
}

