using MTV3D65;

namespace SWEndor.Actors.Types
{
  public class FighterGroup : ActorTypeInfo
  {
    internal FighterGroup(string name): base(name)
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;

      MinSpeed = 100.0f;
      ZTilt = 2.5f;
      ZNormFrac = 0.01f;
      RadarSize = 2;

      CullDistance = 7500;

      TargetType = TargetType.FIGHTER;

      CanEvade = true;
      CanRetaliate = true;
      CanCheckCollisionAhead = true;

      HuntWeight = 5;

      SoundSources = new SoundSourceInfo[] { new SoundSourceInfo("xwing_engine", new TV_3DVECTOR(0, 0, -30), 200, true) };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.MovementInfo.DyingMovement = Components.DyingMovement.SPIN;
      ainfo.MovementInfo.D_spin_min_rate = 120;
      ainfo.MovementInfo.D_spin_max_rate = 270;
    }

    public override void ProcessNewState(ActorInfo ainfo)
    {
      base.ProcessNewState(ainfo);
      if (ainfo.ActorState == ActorState.DYING)
      {
        ainfo.MovementInfo.ApplyZBalance = false;
        ainfo.CombatInfo.OnTimedLife = true;

        if (ainfo.GetAllParents(1).Count > 0 || (!ainfo.GetStateB("No2ndKill") && Engine.Instance().Random.NextDouble() < 0.3f))
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
      if (ainfo.ActorState == ActorState.DYING && !ainfo.GetStateB("No2ndKill"))
        ainfo.ActorState = ActorState.DEAD;

      base.ProcessHit(ainfo, hitby, impact, normal);
    }
  }
}

