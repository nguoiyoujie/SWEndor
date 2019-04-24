using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Groups;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.Player;
using SWEndor.Scenarios;
using System.Collections.Generic;

namespace SWEndor.ActorTypes.Instances
{
  public class HangarBayATI : Groups.AddOn
  {
    private static HangarBayATI _instance;
    public static HangarBayATI Instance()
    {
      if (_instance == null) { _instance = new HangarBayATI(); }
      return _instance;
    }

    private HangarBayATI() : base("Hangar Bay")
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

      ActorInfo p = ActorInfo.Factory.Get(ainfo.GetTopParent());

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
            ActorInfo a = ActorInfo.Factory.Get(id);

            a.ActorState = ActorState.NORMAL;
            ActionManager.UnlockOne(id);
            ActionManager.QueueLast(id, new Hunt());

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
        ActorInfo a = ActorInfo.Factory.Get(i);
        if (a != null && a.TypeInfo is Fighter)
        {
          if (p.SpawnerInfo.SpawnSpeed == -2)
            a.MovementInfo.Speed = a.MovementInfo.MaxSpeed;
          else if (p.SpawnerInfo.SpawnSpeed == -1)
            a.MovementInfo.Speed = p.MovementInfo.Speed;
          else
            a.MovementInfo.Speed = p.SpawnerInfo.SpawnSpeed;

          a.MoveRelative(p.SpawnerInfo.SpawnSpeedPositioningMult.x * p.MovementInfo.Speed * Globals.Engine.Game.TimeSinceRender * p.Scale.x
                       , p.SpawnerInfo.SpawnSpeedPositioningMult.y * p.MovementInfo.Speed * Globals.Engine.Game.TimeSinceRender * p.Scale.y
                       , p.SpawnerInfo.SpawnSpeedPositioningMult.z * p.MovementInfo.Speed * Globals.Engine.Game.TimeSinceRender * p.Scale.z);

          a.MoveRelative(p.SpawnerInfo.SpawnManualPositioningMult.x * Globals.Engine.Game.TimeSinceRender * p.Scale.x
                       , p.SpawnerInfo.SpawnManualPositioningMult.y * Globals.Engine.Game.TimeSinceRender * p.Scale.y
                       , p.SpawnerInfo.SpawnManualPositioningMult.z * Globals.Engine.Game.TimeSinceRender * p.Scale.z);

          if (a.IsPlayer())
            Globals.Engine.PlayerInfo.IsMovementControlsEnabled = false;
        }
      }
    }

    public bool SpawnPlayer(ActorInfo ainfo, ActorInfo p)
    {
      if (!Globals.Engine.PlayerInfo.RequestSpawn)
        return false;

      if (p.SpawnerInfo.NextSpawnTime < Globals.Engine.Game.GameTime + p.SpawnerInfo.SpawnPlayerDelay)
        p.SpawnerInfo.NextSpawnTime = Globals.Engine.Game.GameTime + p.SpawnerInfo.SpawnPlayerDelay;

      Globals.Engine.PlayerInfo.IsMovementControlsEnabled = false;

      ActorCreationInfo acinfo = new ActorCreationInfo(Globals.Engine.PlayerInfo.ActorType);

      TV_3DVECTOR clone = ainfo.GetRelativePositionXYZ(p.SpawnerInfo.PlayerSpawnLocation.x * ainfo.Scale.x, p.SpawnerInfo.PlayerSpawnLocation.y * ainfo.Scale.y, p.SpawnerInfo.PlayerSpawnLocation.z * ainfo.Scale.z);
      acinfo.Position = new TV_3DVECTOR(clone.x, clone.y, clone.z);
      acinfo.Rotation = new TV_3DVECTOR(p.Rotation.x, p.Rotation.y, p.Rotation.z);
      acinfo.Rotation += p.SpawnerInfo.SpawnRotation;

      acinfo.InitialState = ActorState.FREE;
      acinfo.Faction = ainfo.Faction;
      ActorInfo a = ActorInfo.Create(acinfo);
      a.AddParent(ainfo.ID);
      ActionManager.QueueNext(a.ID, new Lock());

      Globals.Engine.PlayerInfo.ActorID = a.ID;

      if (a.TypeInfo.TargetType.HasFlag(TargetType.FIGHTER) && a.Faction.WingLimit >= 0)
        a.Faction.WingLimit++;

      if (a.TypeInfo.TargetType.HasFlag(TargetType.SHIP) && a.Faction.ShipLimit >= 0)
        a.Faction.ShipLimit++;

      Globals.Engine.PlayerInfo.RequestSpawn = false;

      p.SpawnerInfo.SpawnMoveTime = Globals.Engine.Game.GameTime + p.SpawnerInfo.SpawnMoveDelay;
      return true;
    }

    public bool SpawnFighter(ActorInfo ainfo, ActorInfo p)
    {
      if (p.SpawnerInfo.NextSpawnTime < Globals.Engine.Game.GameTime
       && p.SpawnerInfo.SpawnsRemaining > 0
       && p.SpawnerInfo.SpawnTypes != null
       && p.SpawnerInfo.SpawnTypes.Length > 0)
      {
        ActorTypeInfo spawntype = p.SpawnerInfo.SpawnTypes[Globals.Engine.Random.Next(0, p.SpawnerInfo.SpawnTypes.Length)];
        if ((spawntype.TargetType.HasFlag(TargetType.FIGHTER)
          && (p.Faction.WingSpawnLimit == -1 || p.Faction.Wings.Count < p.Faction.WingSpawnLimit)
          && (p.Faction.WingLimit == -1 || p.Faction.Wings.Count < p.Faction.WingLimit)
          )
        || (spawntype.TargetType.HasFlag(TargetType.SHIP)
          && (p.Faction.ShipSpawnLimit == -1 || p.Faction.Ships.Count < p.Faction.ShipSpawnLimit))
          && (p.Faction.ShipLimit == -1 || p.Faction.Ships.Count < p.Faction.ShipLimit)
          )
        {
          p.SpawnerInfo.NextSpawnTime = Globals.Engine.Game.GameTime + p.SpawnerInfo.SpawnInterval;
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
            ActorInfo a = ActorInfo.Create(acinfo);
            a.AddParent(ainfo.ID);
            Globals.Engine.GameScenarioManager.Scenario?.RegisterEvents(a);
            ActionManager.QueueFirst(a.ID, new Lock());
          }
          p.SpawnerInfo.SpawnMoveTime = Globals.Engine.Game.GameTime + p.SpawnerInfo.SpawnMoveDelay;
        }
      }
      return true;
    }
  }
}


