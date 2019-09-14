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
      RenderData.RadarSize = 0;

      AIData.TargetType = TargetType.NULL;
      RenderData.RadarType = RadarType.NULL;

      Mask &= ~(ComponentMask.CAN_BECOLLIDED | ComponentMask.CAN_BETARGETED);
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);

      ActorInfo p = ainfo.TopParent;
      SpawnerInfo s = p.SpawnerInfo;

      if (s == null)
        return;

      if (s.Enabled
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
          if (s.SpawnSpeed == -2)
            a.MoveData.Speed = a.MoveData.MaxSpeed;
          else if (s.SpawnSpeed == -1)
            a.MoveData.Speed = p.MoveData.Speed;
          else
            a.MoveData.Speed = s.SpawnSpeed;

          a.Rotation += p.Rotation - p.PrevRotation;
          a.Position += p.Position - p.PrevPosition;

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

    private bool SpawnFighter(ActorInfo ainfo, ActorInfo p)
    {
      SpawnerInfo s = p.SpawnerInfo;

      if (s.NextSpawnTime < Game.GameTime
       && s.SpawnsRemaining > 0
       && s.SpawnTypes != null
       && s.SpawnTypes.Length > 0)
      {
        ActorTypeInfo spawntype = s.SpawnTypes[Engine.Random.Next(0, s.SpawnTypes.Length)];
        if ((spawntype.AIData.TargetType.Has(TargetType.FIGHTER)
          && (p.Faction.WingSpawnLimit < 0 || p.Faction.WingCount < p.Faction.WingSpawnLimit)
          && (p.Faction.WingLimit < 0 || p.Faction.WingCount < p.Faction.WingLimit)
          )
        || (spawntype.AIData.TargetType.Has(TargetType.SHIP)
          && (p.Faction.ShipSpawnLimit < 0 || p.Faction.WingCount < p.Faction.ShipSpawnLimit))
          && (p.Faction.ShipLimit < 0 || p.Faction.WingCount < p.Faction.ShipLimit)
          )
        {
          s.NextSpawnTime = Game.GameTime + s.SpawnInterval;
          s.SpawnsRemaining--;

          Squadron squad = ainfo.Engine.SquadronFactory.Create();
          foreach (TV_3DVECTOR sv in s.SpawnLocations)
          {
            ActorCreationInfo acinfo = new ActorCreationInfo(spawntype);

            float scale = ainfo.Scale;
            TV_3DVECTOR clone = ainfo.GetRelativePositionXYZ(sv.x * scale, sv.y * scale, sv.z * scale);
            acinfo.Position = new TV_3DVECTOR(clone.x, clone.y, clone.z);
            acinfo.Rotation = ainfo.GetGlobalRotation();
            acinfo.Rotation += s.SpawnRotation;

            acinfo.FreeSpeed = true;
            acinfo.Faction = ainfo.Faction;
            ActorInfo a = ActorFactory.Create(acinfo);
            a.Squad = squad;
            ainfo.AddChild(a);
            GameScenarioManager.Scenario?.RegisterEvents(a);
            a.QueueFirst(new Lock());
          }
          s.SpawnMoveTime = Game.GameTime + s.SpawnMoveDelay;
        }
      }
      return true;
    }
  }
}


