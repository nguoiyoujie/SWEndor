using MTV3D65;
using SWEndor.Actors;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface001_01ATI : Groups.StaticScene
  {
    private static Surface001_01ATI _instance;
    public static Surface001_01ATI Instance()
    {
      if (_instance == null) { _instance = new Surface001_01ATI(); }
      return _instance;
    }

    private Surface001_01ATI() : base("Surface001_01ATI")
    {
      //CollisionEnabled = true;
      EnableDistanceCull = true;
      CullDistance = 20000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface001_01.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.Scale = new TV_3DVECTOR(4, 1, 4);
    }
  }
}

