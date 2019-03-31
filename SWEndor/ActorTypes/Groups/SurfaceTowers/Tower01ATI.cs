using MTV3D65;
using System.IO;

namespace SWEndor.Actors.Types
{
  public class Tower01ATI : SurfaceTowerGroup
  {
    private static Tower01ATI _instance;
    public static Tower01ATI Instance()
    {
      if (_instance == null) { _instance = new Tower01ATI(); }
      return _instance;
    }

    private Tower01ATI() : base("Deflector Tower")
    {
      MaxStrength = 100;
      ImpactDamage = 120;

      Score_perStrength = 50;
      Score_DestroyBonus = 5000;

      NoMove = true;
      NoAI = true;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"towers\tower_01.x");
      AddOns = new AddOnInfo[] { new AddOnInfo("Advanced Turbolaser Turret", new TV_3DVECTOR(0, 135, 0), new TV_3DVECTOR(0, 0, 0), true) };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.ExplosionInfo.EnableDeathExplosion = true;
      ainfo.ExplosionInfo.DeathExplosionType = "ExplosionSm";
      ainfo.ExplosionInfo.DeathExplosionSize = 5;
    }

    public override void ProcessNewState(ActorInfo ainfo)
    {
      base.ProcessNewState(ainfo);

      /*
      if (ainfo.ActorState == ActorState.DEAD)
      {
        ActorCreationInfo acinfo = new ActorCreationInfo(Tower01_RuinsATI.Instance());
        acinfo.Position = ainfo.GetPosition() + new TV_3DVECTOR(0, -72, 0);
        acinfo.Rotation = ainfo.GetRotation();
        ActorInfo a = ActorInfo.Create(acinfo);
      }
      */
    }
  }
}

