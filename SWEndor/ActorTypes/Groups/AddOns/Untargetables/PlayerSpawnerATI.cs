using MTV3D65;
using SWEndor.Actors;
using SWEndor.AI;
using SWEndor.AI.Actions;

namespace SWEndor.ActorTypes.Instances
{
  public class PlayerSpawnerATI : Groups.AddOn
  {
    internal PlayerSpawnerATI(Factory owner) : base(owner, "Player Spawner")
    {
      RenderData.RadarSize = 0;

      AIData.TargetType = TargetType.NULL;
      RenderData.RadarType = RadarType.NULL;

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
      SpawnerInfo s = p.SpawnerInfo;

      if (s != null
       && s.Enabled
       && !p.IsDead
       && !(p.CurrentAction is HyperspaceIn || p.CurrentAction is HyperspaceOut)//ActorState != ActorState.HYPERSPACE
       && p.Active
       )
      {
        if (s.SpawnMoveTime < Game.GameTime)
        {
          foreach (ActorInfo a in ainfo.Children)
          {
            a.MoveData.FreeSpeed = false;
            a.UnlockOne();
            a.QueueLast(new Hunt());

            if (a.IsPlayer)
              PlayerInfo.IsMovementControlsEnabled = true;

            ainfo.RemoveChild(a);
          }
        }

        if (!p.IsDying)
        {
          SpawnPlayer(ainfo, p);
        }
      }

      foreach (ActorInfo a in ainfo.Children)
      {
        if (a != null && a.TypeInfo is Groups.Fighter)
        {
          if (s.SpawnSpeed == -2)
            a.MoveData.Speed = a.MoveData.MaxSpeed;
          else if (s.SpawnSpeed == -1)
            a.MoveData.Speed = p.MoveData.Speed;
          else
            a.MoveData.Speed = s.SpawnSpeed;

          float scale = p.Scale;
          a.MoveRelative(s.SpawnSpeedPositioningMult.x * p.MoveData.Speed * Game.TimeSinceRender * scale
                       , s.SpawnSpeedPositioningMult.y * p.MoveData.Speed * Game.TimeSinceRender * scale
                       , s.SpawnSpeedPositioningMult.z * p.MoveData.Speed * Game.TimeSinceRender * scale);

          a.MoveRelative(s.SpawnManualPositioningMult.x * Game.TimeSinceRender * scale
                       , s.SpawnManualPositioningMult.y * Game.TimeSinceRender * scale
                       , s.SpawnManualPositioningMult.z * Game.TimeSinceRender * scale);

          if (a.IsPlayer)
            PlayerInfo.IsMovementControlsEnabled = false;
        }
      }
    }

    private bool SpawnPlayer(ActorInfo ainfo, ActorInfo p)
    {
      if (!PlayerInfo.RequestSpawn)
        return false;

      SpawnerInfo s = p.SpawnerInfo;

      if (s.NextSpawnTime < Game.GameTime + s.SpawnPlayerDelay)
        s.NextSpawnTime = Game.GameTime + s.SpawnPlayerDelay;

      PlayerInfo.IsMovementControlsEnabled = false;

      ActorCreationInfo acinfo = new ActorCreationInfo(PlayerInfo.ActorType);

      float scale = ainfo.Scale;
      TV_3DVECTOR clone = ainfo.GetRelativePositionXYZ(s.PlayerSpawnLocation.x * scale, s.PlayerSpawnLocation.y * scale, s.PlayerSpawnLocation.z * scale);
      acinfo.Position = new TV_3DVECTOR(clone.x, clone.y, clone.z);
      acinfo.Rotation = ainfo.GetGlobalRotation();
      acinfo.Rotation += s.SpawnRotation;

      acinfo.FreeSpeed = true;
      acinfo.Faction = ainfo.Faction;
      ActorInfo a = ActorFactory.Create(acinfo);
      ainfo.AddChild(a);
      a.QueueNext(new Lock());

      PlayerInfo.ActorID = a.ID;

      if (a.TypeInfo.AIData.TargetType.Has(TargetType.FIGHTER) && a.Faction.WingLimit >= 0)
        a.Faction.WingLimit++;

      if (a.TypeInfo.AIData.TargetType.Has(TargetType.SHIP) && a.Faction.ShipLimit >= 0)
        a.Faction.ShipLimit++;

      PlayerInfo.RequestSpawn = false;

      s.SpawnMoveTime = Game.GameTime + s.SpawnMoveDelay;
      return true;
    }
  }
}

