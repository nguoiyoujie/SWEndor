using SWEndor.Actors;

namespace SWEndor.ActorTypes
{
  public class StarDestroyerGroup : ActorTypeInfo
  {
    internal StarDestroyerGroup(string name) : base(name)
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      CullDistance = 20000;

      ZTilt = 2.5f;
      ZNormFrac = 0.011f;
      RadarSize = 10;

      TargetType = TargetType.SHIP;
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.ExplosionInfo.EnableDeathExplosion = true;
      ainfo.ExplosionInfo.DeathExplosionType = "ExplosionLg";
      ainfo.ExplosionInfo.DeathExplosionSize = 2f;
      ainfo.ExplosionInfo.ExplosionRate = 0.5f;
      ainfo.ExplosionInfo.ExplosionSize = 1;
      ainfo.ExplosionInfo.ExplosionType = "ExplosionSm";

      ainfo.MovementInfo.DyingMovement = Actors.Components.DyingMovement.SINK;
      ainfo.MovementInfo.D_sink_pitch_rate = 0.06f;
      ainfo.MovementInfo.D_sink_down_rate = 15f;
      ainfo.MovementInfo.D_sink_forward_rate = 2.5f;
    }

    public override void ProcessNewState(ActorInfo ainfo)
    {
      base.ProcessNewState(ainfo);
      if (ainfo.ActorState == ActorState.DYING)
      {
        ainfo.CombatInfo.OnTimedLife = true;
        ainfo.CombatInfo.TimedLife = 25f;
        ainfo.CombatInfo.IsCombatObject = false;
      }
      else if (ainfo.ActorState == ActorState.DEAD)
      {
        ActorCreationInfo acinfo = new ActorCreationInfo(ExplosionWaveATI.Instance());
        acinfo.Position = ainfo.GetPosition();
        ActorInfo.Create(acinfo).AddParent(ainfo.ID);
      }
    }
  }
}

