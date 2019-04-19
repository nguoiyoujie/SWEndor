using MTV3D65;
using SWEndor.Actors;

namespace SWEndor.ActorTypes
{
  public class FighterGroup : ActorTypeInfo
  {
    internal FighterGroup(string name): base(name)
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      ZTilt = 2.5f;
      ZNormFrac = 0.01f;
      RadarSize = 2;

      CullDistance = 7500;

      TargetType = TargetType.FIGHTER;

      CanEvade = true;
      CanRetaliate = true;
      CanCheckCollisionAhead = true;

      HuntWeight = 5;
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.MovementInfo.DyingMovement = Actors.Components.DyingMovement.SPIN;
      ainfo.MovementInfo.D_spin_min_rate = 180;
      ainfo.MovementInfo.D_spin_max_rate = 270;

      ainfo.CombatInfo.HitWhileDyingLeadsToDeath = true;
    }

    public override void ProcessNewState(ActorInfo ainfo)
    {
      base.ProcessNewState(ainfo);
      if (ainfo.ActorState == ActorState.DYING)
      {
        ainfo.MovementInfo.ApplyZBalance = false;
        ainfo.CombatInfo.OnTimedLife = true;

        if (ainfo.GetAllParents(1).Count > 0 || (ainfo.CombatInfo.HitWhileDyingLeadsToDeath && Engine.Instance().Random.NextDouble() < 0.3f))
        {
          ainfo.CombatInfo.TimedLife = 0.1f;
        }
        else
        {
          ainfo.CombatInfo.TimedLife = 5f;
        }

        ainfo.CombatInfo.IsCombatObject = false;

        ActorCreationInfo acinfo = new ActorCreationInfo(ElectroATI.Instance());
        acinfo.Position = ainfo.GetPosition();
        ActorInfo.Create(acinfo).AddParent(ainfo.ID);
      }
    }
  }
}

