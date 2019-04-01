using SWEndor.Actors;
using System.IO;

namespace SWEndor.ActorTypes
{
  public class Tower03ATI : SurfaceTowerGroup
  {
    private static Tower03ATI _instance;
    public static Tower03ATI Instance()
    {
      if (_instance == null) { _instance = new Tower03ATI(); }
      return _instance;
    }

    private Tower03ATI() : base("Radar Tower")
    {
      MaxStrength = 75;
      ImpactDamage = 120;

      Score_perStrength = 50;
      Score_DestroyBonus = 5000;

      RadarSize = 2.5f;

      NoMove = true;
      NoAI = true;

      AlwaysShowInRadar = true;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"towers\tower_03.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      // Tower Gun
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
        ActorCreationInfo acinfo = new ActorCreationInfo(Tower03_RuinsATI.Instance());
        acinfo.Position = ainfo.GetPosition() + new TV_3DVECTOR(0, -72, 0);
        acinfo.Rotation = ainfo.GetRotation();
        ActorInfo a = ActorInfo.Create(acinfo);
      }
      */
    }
  }
}

