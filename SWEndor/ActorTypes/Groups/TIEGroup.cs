using MTV3D65;

namespace SWEndor.Actors.Types
{
  public class TIEGroup : ActorTypeInfo
  {
    internal TIEGroup(string name): base(name)
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      ZTilt = 2.5f;
      ZNormFrac = 0.01f;
      RadarSize = 2;

      CullDistance = 7200;

      TargetType = TargetType.FIGHTER;

      CanEvade = true;
      CanRetaliate = true;
      CanCheckCollisionAhead = true;

      HuntWeight = 5;

      SoundSources = new SoundSourceInfo[]{ new SoundSourceInfo("engine_tie", new TV_3DVECTOR(0, 0, -30), 200, true) };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.MovementInfo.DyingMovement = Components.DyingMovement.SPIN;
      ainfo.MovementInfo.D_spin_min_rate = 180;
      ainfo.MovementInfo.D_spin_max_rate = 270;
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
        ActorInfo.Create(acinfo).AddParent(ainfo);
      }
    }

    public override void ProcessHit(ActorInfo ainfo, ActorInfo hitby, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      if (ainfo.ActorState == ActorState.DYING && ainfo.CombatInfo.HitWhileDyingLeadsToDeath)
        ainfo.ActorState = ActorState.DEAD;

      base.ProcessHit(ainfo, hitby, impact, normal);
    }
  }
}

