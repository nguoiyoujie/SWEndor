using SWEndor.Actors;
using SWEndor.Actors.Components;

namespace SWEndor.ActorTypes.Groups
{
  public class Fighter : ActorTypeInfo
  {
    internal Fighter(Factory owner, string name): base(owner, name)
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
      ainfo.DyingMovement = new DyingSpinInfo(180, 270);

      ainfo.CombatInfo.HitWhileDyingLeadsToDeath = true;
    }

    public override void ProcessNewState(ActorInfo ainfo)
    {
      base.ProcessNewState(ainfo);
      if (ainfo.ActorState.IsDying())
      {
        ainfo.MovementInfo.ApplyZBalance = false;
        ainfo.CombatInfo.OnTimedLife = true;

        if (ainfo.GetAllParents(1).Count > 0 || (ainfo.CombatInfo.HitWhileDyingLeadsToDeath && Globals.Engine.Random.NextDouble() < 0.3f))
        {
          ainfo.CombatInfo.TimedLife = 0.1f;
        }
        else
        {
          ainfo.CombatInfo.TimedLife = 5f;
        }

        ainfo.CombatInfo.IsCombatObject = false;

        ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Get("Electro"));
        acinfo.Position = ainfo.GetPosition();
        ActorInfo.Create(ActorFactory, acinfo).AddParent(ainfo.ID);
      }
    }
  }
}

