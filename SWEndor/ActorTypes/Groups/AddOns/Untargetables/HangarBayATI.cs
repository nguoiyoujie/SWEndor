using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Groups;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.AI.Squads;

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

      ActorInfo p = ainfo.TopParent;

      if (p.SpawnerInfo != null
       && p.SpawnerInfo.Enabled
       && !p.IsDead
       && !(p.CurrentAction is HyperspaceIn || p.CurrentAction is HyperspaceOut)//ActorState != ActorState.HYPERSPACE
       && p.Active
       )
      {
        if (p.SpawnerInfo.SpawnMoveTime < Game.GameTime)
        {
          foreach (ActorInfo a in ainfo.Children)
          {
            a.MoveData.FreeSpeed = false;
            a.UnlockOne();
            //a.QueueLast(new Hunt());

            if (a.IsPlayer)
              PlayerInfo.IsMovementControlsEnabled = true;

            ainfo.RemoveChild(a);
          }
        }

        if (!p.IsDying)
        {
          SpawnPlayer(ainfo, p);
          SpawnFighter(ainfo, p);
        }
      }

      foreach (ActorInfo a in ainfo.Children)
      {
        if (a != null && a.TypeInfo is Fighter)
        {
          if (p.SpawnerInfo.SpawnSpeed == -2)
            a.MoveData.Speed = a.MoveData.MaxSpeed;
          else if (p.SpawnerInfo.SpawnSpeed == -1)
            a.MoveData.Speed = p.MoveData.Speed;
          else
            a.MoveData.Speed = p.SpawnerInfo.SpawnSpeed;

          a.Rotation += p.Rotation - p.PrevRotation;
          a.Position += p.Position - p.PrevPosition;

          float scale = p.Scale;

          a.MoveRelative(p.SpawnerInfo.SpawnSpeedPositioningMult.x * p.MoveData.Speed * Game.TimeSinceRender * scale
                       , p.SpawnerInfo.SpawnSpeedPositioningMult.y * p.MoveData.Speed * Game.TimeSinceRender * scale
                       , p.SpawnerInfo.SpawnSpeedPositioningMult.z * p.MoveData.Speed * Game.TimeSinceRender * scale);

          a.MoveRelative(p.SpawnerInfo.SpawnManualPositioningMult.x * Game.TimeSinceRender * scale
                       , p.SpawnerInfo.SpawnManualPositioningMult.y * Game.TimeSinceRender * scale
                       , p.SpawnerInfo.SpawnManualPositioningMult.z * Game.TimeSinceRender * scale);

          if (a.IsPlayer)
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

      float scale = ainfo.Scale;
      TV_3DVECTOR clone = ainfo.GetRelativePositionXYZ(p.SpawnerInfo.PlayerSpawnLocation.x * scale, p.SpawnerInfo.PlayerSpawnLocation.y * scale, p.SpawnerInfo.PlayerSpawnLocation.z * scale);
      acinfo.Position = new TV_3DVECTOR(clone.x, clone.y, clone.z);
      acinfo.Rotation = ainfo.GetGlobalRotation();
      acinfo.Rotation += p.SpawnerInfo.SpawnRotation;

      acinfo.FreeSpeed = true;
      acinfo.Faction = ainfo.Faction;
      ActorInfo a = ActorFactory.Create(acinfo);
      ainfo.AddChild(a);
      a.QueueNext(new Lock());

      PlayerInfo.ActorID = a.ID;

      if (a.TypeInfo.TargetType.Has(TargetType.FIGHTER) && a.Faction.WingLimit >= 0)
        a.Faction.WingLimit++;

      if (a.TypeInfo.TargetType.Has(TargetType.SHIP) && a.Faction.ShipLimit >= 0)
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
        if ((spawntype.TargetType.Has(TargetType.FIGHTER)
          && (p.Faction.WingSpawnLimit == -1 || p.Faction.Wings.Count < p.Faction.WingSpawnLimit)
          && (p.Faction.WingLimit == -1 || p.Faction.Wings.Count < p.Faction.WingLimit)
          )
        || (spawntype.TargetType.Has(TargetType.SHIP)
          && (p.Faction.ShipSpawnLimit == -1 || p.Faction.Ships.Count < p.Faction.ShipSpawnLimit))
          && (p.Faction.ShipLimit == -1 || p.Faction.Ships.Count < p.Faction.ShipLimit)
          )
        {
          p.SpawnerInfo.NextSpawnTime = Game.GameTime + p.SpawnerInfo.SpawnInterval;
          p.SpawnerInfo.SpawnsRemaining--;

          Squadron squad = ainfo.Engine.SquadronFactory.Create();
          foreach (TV_3DVECTOR sv in p.SpawnerInfo.SpawnLocations)
          {
            ActorCreationInfo acinfo = new ActorCreationInfo(spawntype);

            float scale = ainfo.Scale;
            TV_3DVECTOR clone = ainfo.GetRelativePositionXYZ(sv.x * scale, sv.y * scale, sv.z * scale);
            acinfo.Position = new TV_3DVECTOR(clone.x, clone.y, clone.z);
            acinfo.Rotation = ainfo.GetGlobalRotation();
            acinfo.Rotation += p.SpawnerInfo.SpawnRotation;

            acinfo.FreeSpeed = true;
            acinfo.Faction = ainfo.Faction;
            ActorInfo a = ActorFactory.Create(acinfo);
            a.Squad = squad;
            ainfo.AddChild(a);
            GameScenarioManager.Scenario?.RegisterEvents(a);
            a.QueueFirst(new Lock());
          }
          p.SpawnerInfo.SpawnMoveTime = Game.GameTime + p.SpawnerInfo.SpawnMoveDelay;
        }
      }
      return true;
    }
  }
}


