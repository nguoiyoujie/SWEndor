using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Groups;
using SWEndor.AI.Actions;

namespace SWEndor.ActorTypes.Instances
{
  public class HangarBayATI : Groups.AddOn
  {
    internal HangarBayATI(Factory owner) : base(owner, "Hangar Bay")
    {
      RadarSize = 0;

      TargetType = TargetType.NULL;
      RadarType = RadarType.NULL;

      Mask &= ~(ComponentMask.CAN_BECOLLIDED | ComponentMask.CAN_BETARGETED);
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);

      ActorInfo p = ActorFactory.Get(ainfo.TopParent);

      if (p.SpawnerInfo != null
       && p.SpawnerInfo.Enabled
       && p.ActorState != ActorState.DEAD
       && p.ActorState != ActorState.HYPERSPACE
       && p.CreationState == CreationState.ACTIVE
       )
      {
        if (p.SpawnerInfo.SpawnMoveTime < Game.GameTime)
        {
          foreach (int id in ainfo.Children)
          {
            ActorInfo a = ActorFactory.Get(id);

            a.ActorState = ActorState.NORMAL;
            ActionManager.UnlockOne(id);
            ActionManager.QueueLast(id, new Hunt());

            if (ActorInfo.IsPlayer(Engine, id))
              PlayerInfo.IsMovementControlsEnabled = true;

            ainfo.RemoveChild(id);
          }
        }

        if (p.ActorState != ActorState.DYING)
        {
          SpawnPlayer(ainfo, p);
          SpawnFighter(ainfo, p);
        }
      }

      foreach (int id in ainfo.Children)
      {
        ActorInfo a = ActorFactory.Get(id);
        if (a != null && a.TypeInfo is Fighter)
        {
          if (p.SpawnerInfo.SpawnSpeed == -2)
            a.MoveData.Speed = a.MoveData.MaxSpeed;
          else if (p.SpawnerInfo.SpawnSpeed == -1)
            a.MoveData.Speed = p.MoveData.Speed;
          else
            a.MoveData.Speed = p.SpawnerInfo.SpawnSpeed;

          float scale = Engine.MeshDataSet.Scale_get(p.ID);
          a.MoveRelative(p.SpawnerInfo.SpawnSpeedPositioningMult.x * p.MoveData.Speed * Game.TimeSinceRender * scale
                       , p.SpawnerInfo.SpawnSpeedPositioningMult.y * p.MoveData.Speed * Game.TimeSinceRender * scale
                       , p.SpawnerInfo.SpawnSpeedPositioningMult.z * p.MoveData.Speed * Game.TimeSinceRender * scale);

          a.MoveRelative(p.SpawnerInfo.SpawnManualPositioningMult.x * Game.TimeSinceRender * scale
                       , p.SpawnerInfo.SpawnManualPositioningMult.y * Game.TimeSinceRender * scale
                       , p.SpawnerInfo.SpawnManualPositioningMult.z * Game.TimeSinceRender * scale);

          if (ActorInfo.IsPlayer(Engine, id))
            PlayerInfo.IsMovementControlsEnabled = false;
        }
      }
    }

    private bool SpawnPlayer(ActorInfo ainfo, ActorInfo p)
    {
      if (!PlayerInfo.RequestSpawn)
        return false;

      if (p.SpawnerInfo.NextSpawnTime < Game.GameTime + p.SpawnerInfo.SpawnPlayerDelay)
        p.SpawnerInfo.NextSpawnTime = Game.GameTime + p.SpawnerInfo.SpawnPlayerDelay;

      PlayerInfo.IsMovementControlsEnabled = false;

      ActorCreationInfo acinfo = new ActorCreationInfo(PlayerInfo.ActorType);

      float scale = Engine.MeshDataSet.Scale_get(ainfo.ID);
      TV_3DVECTOR clone = ainfo.GetRelativePositionXYZ(p.SpawnerInfo.PlayerSpawnLocation.x * scale, p.SpawnerInfo.PlayerSpawnLocation.y * scale, p.SpawnerInfo.PlayerSpawnLocation.z * scale);
      acinfo.Position = new TV_3DVECTOR(clone.x, clone.y, clone.z);
      acinfo.Rotation = new TV_3DVECTOR(p.CoordData.Rotation.x, p.CoordData.Rotation.y, p.CoordData.Rotation.z);
      acinfo.Rotation += p.SpawnerInfo.SpawnRotation;

      acinfo.InitialState = ActorState.FREE;
      acinfo.Faction = ainfo.Faction;
      ActorInfo a = ActorInfo.Create(ActorFactory, acinfo);
      ainfo.AddChild(a.ID);
      ActionManager.QueueNext(a.ID, new Lock());

      PlayerInfo.ActorID = a.ID;

      if (a.TypeInfo.TargetType.HasFlag(TargetType.FIGHTER) && a.Faction.WingLimit >= 0)
        a.Faction.WingLimit++;

      if (a.TypeInfo.TargetType.HasFlag(TargetType.SHIP) && a.Faction.ShipLimit >= 0)
        a.Faction.ShipLimit++;

      PlayerInfo.RequestSpawn = false;

      p.SpawnerInfo.SpawnMoveTime = Game.GameTime + p.SpawnerInfo.SpawnMoveDelay;
      return true;
    }

    private bool SpawnFighter(ActorInfo ainfo, ActorInfo p)
    {
      if (p.SpawnerInfo.NextSpawnTime < Game.GameTime
       && p.SpawnerInfo.SpawnsRemaining > 0
       && p.SpawnerInfo.SpawnTypes != null
       && p.SpawnerInfo.SpawnTypes.Length > 0)
      {
        ActorTypeInfo spawntype = p.SpawnerInfo.SpawnTypes[Engine.Random.Next(0, p.SpawnerInfo.SpawnTypes.Length)];
        if ((spawntype.TargetType.HasFlag(TargetType.FIGHTER)
          && (p.Faction.WingSpawnLimit == -1 || p.Faction.Wings.Count < p.Faction.WingSpawnLimit)
          && (p.Faction.WingLimit == -1 || p.Faction.Wings.Count < p.Faction.WingLimit)
          )
        || (spawntype.TargetType.HasFlag(TargetType.SHIP)
          && (p.Faction.ShipSpawnLimit == -1 || p.Faction.Ships.Count < p.Faction.ShipSpawnLimit))
          && (p.Faction.ShipLimit == -1 || p.Faction.Ships.Count < p.Faction.ShipLimit)
          )
        {
          p.SpawnerInfo.NextSpawnTime = Game.GameTime + p.SpawnerInfo.SpawnInterval;
          p.SpawnerInfo.SpawnsRemaining--;

          foreach (TV_3DVECTOR sv in p.SpawnerInfo.SpawnLocations)
          {
            ActorCreationInfo acinfo = new ActorCreationInfo(spawntype);

            float scale = Engine.MeshDataSet.Scale_get(ainfo.ID);
            TV_3DVECTOR clone = ainfo.GetRelativePositionXYZ(sv.x * scale, sv.y * scale, sv.z * scale);
            acinfo.Position = new TV_3DVECTOR(clone.x, clone.y, clone.z);
            acinfo.Rotation = new TV_3DVECTOR(p.CoordData.Rotation.x, p.CoordData.Rotation.y, p.CoordData.Rotation.z);
            acinfo.Rotation += p.SpawnerInfo.SpawnRotation;

            acinfo.InitialState = ActorState.FREE;
            acinfo.Faction = ainfo.Faction;
            ActorInfo a = ActorInfo.Create(ActorFactory, acinfo);
            ainfo.AddChild(a.ID);
            GameScenarioManager.Scenario?.RegisterEvents(a);
            ActionManager.QueueFirst(a.ID, new Lock());
          }
          p.SpawnerInfo.SpawnMoveTime = Game.GameTime + p.SpawnerInfo.SpawnMoveDelay;
        }
      }
      return true;
    }
  }
}


