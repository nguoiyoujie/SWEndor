using MTV3D65;
using System.IO;

namespace SWEndor.Actors.Types
{
  public class Tower00ATI : SurfaceTowerGroup
  {
    private static Tower00ATI _instance;
    public static Tower00ATI Instance()
    {
      if (_instance == null) { _instance = new Tower00ATI(); }
      return _instance;
    }

    private Tower00ATI() : base("Advanced Turbolaser Tower")
    {
      MaxStrength = 120;
      ImpactDamage = 120;

      RadarSize = 2.5f;

      Score_perStrength = 50;
      Score_DestroyBonus = 5000;

      NoMove = true;
      NoAI = true;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"towers\tower_00.x");

      AddOns = new AddOnInfo[] 
      {
        new AddOnInfo("Turbolaser Turret", new TV_3DVECTOR(95, 155, 0), new TV_3DVECTOR(0, 0, 0), true)
        , new AddOnInfo("Turbolaser Turret", new TV_3DVECTOR(-95, 155, 0), new TV_3DVECTOR(0, 0, 0), true)
      };
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
        ActorCreationInfo acinfo = new ActorCreationInfo(Tower00_RuinsATI.Instance());
        acinfo.Position = ainfo.GetPosition() + new TV_3DVECTOR(0, -72, 0);
        acinfo.Rotation = ainfo.GetRotation();
        ActorInfo a = ActorInfo.Create(acinfo);
      }
      */
    }
  }
}

