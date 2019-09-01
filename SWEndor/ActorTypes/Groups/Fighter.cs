﻿using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Groups
{
  public class Fighter : ActorTypeInfo
  {
    internal Fighter(Factory owner, string name) : base(owner, name)
    {
      // Combat
      CombatData = CombatData.DefaultFighter;
      Armor = ArmorInfo.Default;
      Explodes = new ExplodeInfo[]
      {
        new ExplodeInfo("ExpS00", 0.75f, 1, ExplodeTrigger.ON_DYING | ExplodeTrigger.CREATE_ON_MESHVERTICES),
        new ExplodeInfo("ExpL00", 1, 1, ExplodeTrigger.ON_DEATH)
      };

      ZTilt = 2.5f;
      ZNormFrac = 0.01f;
      RadarSize = 2;

      CullDistance = 7500;

      TargetType = TargetType.FIGHTER;
      RadarType = RadarType.FILLED_CIRCLE_S;

      Mask = ComponentMask.ACTOR;

      CanEvade = true;
      CanRetaliate = true;
      CanCheckCollisionAhead = true;

      HuntWeight = 5;
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.DyingMoveComponent = new DyingSpin(180, 270);
    }

    public override void Dying(ActorInfo ainfo)
    {
      base.Dying(ainfo);

      ainfo.MoveData.ApplyZBalance = false;

      if (ainfo.Parent != null || (ainfo.ActorDataSet.CombatData[ainfo.dataID].HitWhileDyingLeadsToDeath && Engine.Random.NextDouble() < 0.3f))
        ainfo.DyingTimerSet(0.1f, true);
      else
        ainfo.DyingTimerSet(5, true);

      CombatSystem.Deactivate(Engine, ainfo);

      ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Get("Electro"));
      acinfo.Position = ainfo.GetGlobalPosition();
      ainfo.AddChild(ActorFactory.Create(acinfo));
    }
  }
}

