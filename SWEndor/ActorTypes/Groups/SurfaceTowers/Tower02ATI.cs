using MTV3D65;
using System.IO;

namespace SWEndor.Actors.Types
{
  public class Tower02ATI : SurfaceTowerGroup
  {
    private static Tower02ATI _instance;
    public static Tower02ATI Instance()
    {
      if (_instance == null) { _instance = new Tower02ATI(); }
      return _instance;
    }

    private Tower02ATI() : base("Gun Tower")
    {
      MaxStrength = 25;
      ImpactDamage = 120;

      Score_perStrength = 50;
      Score_DestroyBonus = 5000;

      NoMove = true;
      NoAI = true;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"towers\tower_02.x");
      AddOns = new AddOnInfo[] { new AddOnInfo("Turbolaser Turret", new TV_3DVECTOR(0, 50, 0), new TV_3DVECTOR(0, 0, 0), true) };
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
        ActorCreationInfo acinfo = new ActorCreationInfo(Tower02_RuinsATI.Instance());
        acinfo.Position = ainfo.GetPosition() + new TV_3DVECTOR(0, -21, 0);
        acinfo.Rotation = ainfo.GetRotation();
        ActorInfo a = ActorInfo.Create(acinfo);
      }
      */
    }
  }
}

