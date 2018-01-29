using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class ImperialIStaticATI : StaticSceneGroup
  {
    private static ImperialIStaticATI _instance;
    public static ImperialIStaticATI Instance()
    {
      if (_instance == null) { _instance = new ImperialIStaticATI(); }
      return _instance;
    }

    private ImperialIStaticATI() : base("Imperial-I Static")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"stardestroyer\star_destroyer_static.x");
    }
  }
}

