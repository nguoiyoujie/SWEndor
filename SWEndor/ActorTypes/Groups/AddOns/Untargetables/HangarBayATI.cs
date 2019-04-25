﻿using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Groups;
using SWEndor.AI;
using SWEndor.AI.Actions;
using System.Collections.Generic;

namespace SWEndor.ActorTypes.Instances
{
  public class HangarBayATI : Groups.AddOn
  {
    internal HangarBayATI(Factory owner) : base(owner, "Hangar Bay")
    {
      // Combat
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = false;
      CollisionEnabled = false;
      RadarSize = 0;

      TargetType = TargetType.NULL;
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);

      ActorInfo p = Owner.Engine.ActorFactory.Get(ainfo.GetTopParent());

      if (p.SpawnerInfo != null
       && p.SpawnerInfo.Enabled
       && p.ActorState != ActorState.DEAD
       && p.ActorState != ActorState.HYPERSPACE
       && p.CreationState == CreationState.ACTIVE
       )
      {
        if (p.SpawnerInfo.SpawnMoveTime < Globals.Engine.Game.GameTime)
        {
          List<ActorInfo> rm = new List<ActorInfo>();
          foreach (int id in ainfo.GetAllChildren(1))
          {
            ActorInfo a = Owner.Engine.ActorFactory.Get(id);

            a.ActorState = ActorState.NORMAL;
            Owner.Engine.ActionManager.UnlockOne(id);
            Owner.Engine.ActionManager.QueueLast(id, new Hunt());

            if (a.IsPlayer())
              Globals.Engine.PlayerInfo.IsMovementControlsEnabled = true;

            rm.Add(a);
          }

          foreach (ActorInfo a in rm)
            a.RemoveParent();
        }

        if (p.ActorState != ActorState.DYING)
        {
          SpawnPlayer(ainfo, p);
          SpawnFighter(ainfo, p);
        }
      }

      foreach (int i in ainfo.GetAllChildren(1))
      {
        ActorInfo a = Owner.Engine.ActorFactory.Get(i);
        if (a != null && a.TypeInfo is Fighter)
        {
          if (p.SpawnerInfo.SpawnSpeed == -2)
            a.MovementInfo.Speed = a.MovementInfo.MaxSpeed;
          else if (p.SpawnerInfo.SpawnSpeed == -1)
            a.MovementInfo.Speed = p.MovementInfo.Speed;
          else
            a.MovementInfo.Speed = p.SpawnerInfo.SpawnSpeed;

          a.MoveRelative(p.SpawnerInfo.SpawnSpeedPositioningMult.x * p.MovementInfo.Speed * Owner.Engine.Game.TimeSinceRender * p.Scale.x
                       , p.SpawnerInfo.SpawnSpeedPositioningMult.y * p.MovementInfo.Speed * Owner.Engine.Game.TimeSinceRender * p.Scale.y
                       , p.SpawnerInfo.SpawnSpeedPositioningMult.z * p.MovementInfo.Speed * Owner.Engine.Game.TimeSinceRender * p.Scale.z);

          a.MoveRelative(p.SpawnerInfo.SpawnManualPositioningMult.x * Owner.Engine.Game.TimeSinceRender * p.Scale.x
                       , p.SpawnerInfo.SpawnManualPositioningMult.y * Owner.Engine.Game.TimeSinceRender * p.Scale.y
                       , p.SpawnerInfo.SpawnManualPositioningMult.z * Owner.Engine.Game.TimeSinceRender * p.Scale.z);

          if (a.IsPlayer())
            Globals.Engine.PlayerInfo.IsMovementControlsEnabled = false;
        }
      }
    }

    public bool SpawnPlayer(ActorInfo ainfo, ActorInfo p)
    {
      if (!Globals.Engine.PlayerInfo.RequestSpawn)
        return false;

      if (p.SpawnerInfo.NextSpawnTime < Owner.Engine.Game.GameTime + p.SpawnerInfo.SpawnPlayerDelay)
        p.SpawnerInfo.NextSpawnTime = Owner.Engine.Game.GameTime + p.SpawnerInfo.SpawnPlayerDelay;

      Owner.Engine.PlayerInfo.IsMovementControlsEnabled = false;

      ActorCreationInfo acinfo = new ActorCreationInfo(Owner.Engine.PlayerInfo.ActorType);

      TV_3DVECTOR clone = ainfo.GetRelativePositionXYZ(p.SpawnerInfo.PlayerSpawnLocation.x * ainfo.Scale.x, p.SpawnerInfo.PlayerSpawnLocation.y * ainfo.Scale.y, p.SpawnerInfo.PlayerSpawnLocation.z * ainfo.Scale.z);
      acinfo.Position = new TV_3DVECTOR(clone.x, clone.y, clone.z);
      acinfo.Rotation = new TV_3DVECTOR(p.Rotation.x, p.Rotation.y, p.Rotation.z);
      acinfo.Rotation += p.SpawnerInfo.SpawnRotation;

      acinfo.InitialState = ActorState.FREE;
      acinfo.Faction = ainfo.Faction;
      ActorInfo a = ActorInfo.Create(Owner.Engine.ActorFactory, acinfo);
      a.AddParent(ainfo.ID);
      Owner.Engine.ActionManager.QueueNext(a.ID, new Lock());

      Owner.Engine.PlayerInfo.ActorID = a.ID;

      if (a.TypeInfo.TargetType.HasFlag(TargetType.FIGHTER) && a.Faction.WingLimit >= 0)
        a.Faction.WingLimit++;

      if (a.TypeInfo.TargetType.HasFlag(TargetType.SHIP) && a.Faction.ShipLimit >= 0)
        a.Faction.ShipLimit++;

      Owner.Engine.PlayerInfo.RequestSpawn = false;

      p.SpawnerInfo.SpawnMoveTime = Owner.Engine.Game.GameTime + p.SpawnerInfo.SpawnMoveDelay;
      return true;
    }

    public bool SpawnFighter(ActorInfo ainfo, ActorInfo p)
    {
      if (p.SpawnerInfo.NextSpawnTime < Owner.Engine.Game.GameTime
       && p.SpawnerInfo.SpawnsRemaining > 0
       && p.SpawnerInfo.SpawnTypes != null
       && p.SpawnerInfo.SpawnTypes.Length > 0)
      {
        ActorTypeInfo spawntype = p.SpawnerInfo.SpawnTypes[Owner.Engine.Random.Next(0, p.SpawnerInfo.SpawnTypes.Length)];
        if ((spawntype.TargetType.HasFlag(TargetType.FIGHTER)
          && (p.Faction.WingSpawnLimit == -1 || p.Faction.Wings.Count < p.Faction.WingSpawnLimit)
          && (p.Faction.WingLimit == -1 || p.Faction.Wings.Count < p.Faction.WingLimit)
          )
        || (spawntype.TargetType.HasFlag(TargetType.SHIP)
          && (p.Faction.ShipSpawnLimit == -1 || p.Faction.Ships.Count < p.Faction.ShipSpawnLimit))
          && (p.Faction.ShipLimit == -1 || p.Faction.Ships.Count < p.Faction.ShipLimit)
          )
        {
          p.SpawnerInfo.NextSpawnTime = Owner.Engine.Game.GameTime + p.SpawnerInfo.SpawnInterval;
          p.SpawnerInfo.SpawnsRemaining--;

          foreach (TV_3DVECTOR sv in p.SpawnerInfo.SpawnLocations)
          {
            ActorCreationInfo acinfo = new ActorCreationInfo(spawntype);
            TV_3DVECTOR clone = ainfo.GetRelativePositionXYZ(sv.x * ainfo.Scale.x, sv.y * ainfo.Scale.y, sv.z * ainfo.Scale.z);
            acinfo.Position = new TV_3DVECTOR(clone.x, clone.y, clone.z);
            acinfo.Rotation = new TV_3DVECTOR(p.Rotation.x, p.Rotation.y, p.Rotation.z);
            acinfo.Rotation += p.SpawnerInfo.SpawnRotation;

            acinfo.InitialState = ActorState.FREE;
            acinfo.Faction = ainfo.Faction;
            ActorInfo a = ActorInfo.Create(Owner.Engine.ActorFactory, acinfo);
            a.AddParent(ainfo.ID);
            Owner.Engine.GameScenarioManager.Scenario?.RegisterEvents(a);
            Owner.Engine.ActionManager.QueueFirst(a.ID, new Lock());
          }
          p.SpawnerInfo.SpawnMoveTime = Owner.Engine.Game.GameTime + p.SpawnerInfo.SpawnMoveDelay;
        }
      }
      return true;
    }
  }
}


