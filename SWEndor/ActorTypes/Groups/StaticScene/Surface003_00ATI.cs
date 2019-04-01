using MTV3D65;
using SWEndor.Actors;
using System.IO;

namespace SWEndor.ActorTypes
{
  public class Surface003_00ATI : StaticSceneGroup
  {
    private static Surface003_00ATI _instance;
    public static Surface003_00ATI Instance()
    {
      if (_instance == null) { _instance = new Surface003_00ATI(); }
      return _instance;
    }

    private Surface003_00ATI() : base("Surface003_00ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 1000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface003.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.Scale = new TV_3DVECTOR(4, 1, 4);
    }
  }
}

