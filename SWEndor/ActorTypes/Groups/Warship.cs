using SWEndor.Actors;
using SWEndor.ActorTypes.Instances;

namespace SWEndor.ActorTypes.Groups
{
  public class Warship : ActorTypeInfo
  {
    internal Warship(Factory owner, string name): base(owner, name)
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      CullDistance = 20000;

      ZTilt = 7.5f;
      ZNormFrac = 0.005f;
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
        ainfo.CombatInfo.TimedLife = 20f;
        ainfo.CombatInfo.IsCombatObject = false;
      }
      else if (ainfo.ActorState == ActorState.DEAD)
      {
        ActorCreationInfo acinfo = new ActorCreationInfo(Owner.Get("Explosion Wave"));
        acinfo.Position = ainfo.GetPosition();
        ActorInfo.Create(Owner.Engine.ActorFactory, acinfo).AddParent(ainfo.ID);
      }
    }
  }
}

