﻿using MTV3D65;
using SWEndor.Actors;
using SWEndor.AI;
using SWEndor.AI.Actions;

namespace SWEndor.ActorTypes.Instances
{
  public class PlayerSpawnerATI : Groups.AddOn
  {
    internal PlayerSpawnerATI(Factory owner) : base(owner, "Player Spawner")
    {
      RadarSize = 0;

      TargetType = TargetType.NULL;
      RadarType = RadarType.NULL;

      Mask &= ~(ComponentMask.CAN_BECOLLIDED | ComponentMask.CAN_BETARGETED);
    }

    public override void Initialize(ActorInfo ainfo)
    {
      ainfo.SpawnerInfo = new PlayerSpawner(ainfo);
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);

      ActorInfo p = ainfo.TopParent;

      if (p.SpawnerInfo != null
       && p.SpawnerInfo.Enabled
       && p.ActorState != ActorState.DEAD
       && p.ActorState != ActorState.HYPERSPACE
       && p.CreationState == CreationState.ACTIVE
       )
      {
        if (p.SpawnerInfo.SpawnMoveTime < Game.GameTime)
        {
          foreach (ActorInfo a in ainfo.Children)
          {
            a.ActorState = ActorState.NORMAL;
            a.UnlockOne();
            a.QueueLast(new Hunt());

            if (a.IsPlayer)
              PlayerInfo.IsMovementControlsEnabled = true;

            ainfo.RemoveChild(a);
          }
        }

        if (p.ActorState != ActorState.DYING)
        {
          SpawnPlayer(ainfo, p);
        }
      }

      foreach (ActorInfo a in ainfo.Children)
      {
        if (a != null && a.TypeInfo is Groups.Fighter)
        {
          if (p.SpawnerInfo.SpawnSpeed == -2)
            a.MoveData.Speed = a.MoveData.MaxSpeed;
          else if (p.SpawnerInfo.SpawnSpeed == -1)
            a.MoveData.Speed = p.MoveData.Speed;
          else
            a.MoveData.Speed = p.SpawnerInfo.SpawnSpeed;

          float scale = Engine.MeshDataSet.Scale_get(p);
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

      float scale = Engine.MeshDataSet.Scale_get(ainfo);
      TV_3DVECTOR clone = ainfo.GetRelativePositionXYZ(p.SpawnerInfo.PlayerSpawnLocation.x * scale, p.SpawnerInfo.PlayerSpawnLocation.y * scale, p.SpawnerInfo.PlayerSpawnLocation.z * scale);
      acinfo.Position = new TV_3DVECTOR(clone.x, clone.y, clone.z);
      acinfo.Rotation = new TV_3DVECTOR(p.CoordData.Rotation.x, p.CoordData.Rotation.y, p.CoordData.Rotation.z);
      acinfo.Rotation += p.SpawnerInfo.SpawnRotation;

      acinfo.InitialState = ActorState.FREE;
      acinfo.Faction = ainfo.Faction;
      ActorInfo a = ActorFactory.Create(acinfo);
      ainfo.AddChild(a);
      a.QueueNext(new Lock());

      PlayerInfo.ActorID = a.ID;

      if (a.TypeInfo.TargetType.HasFlag(TargetType.FIGHTER) && a.Faction.WingLimit >= 0)
        a.Faction.WingLimit++;

      if (a.TypeInfo.TargetType.HasFlag(TargetType.SHIP) && a.Faction.ShipLimit >= 0)
        a.Faction.ShipLimit++;

      PlayerInfo.RequestSpawn = false;

      p.SpawnerInfo.SpawnMoveTime = Game.GameTime + p.SpawnerInfo.SpawnMoveDelay;
      return true;
    }
  }
}

