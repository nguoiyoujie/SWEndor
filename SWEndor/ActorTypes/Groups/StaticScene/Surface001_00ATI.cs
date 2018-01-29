using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class Surface001_00ATI : StaticSceneGroup
  {
    private static Surface001_00ATI _instance;
    public static Surface001_00ATI Instance()
    {
      if (_instance == null) { _instance = new Surface001_00ATI(); }
      return _instance;
    }

    private Surface001_00ATI() : base("Surface001_00ATI")
    {
      //CollisionEnabled = true;
      EnableDistanceCull = true;
      CullDistance = 20000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface001_00.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.Scale = new TV_3DVECTOR(4, 1, 4);
    }
  }
}

