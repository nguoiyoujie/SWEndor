using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.AI.Squads;
using SWEndor.Core;
using SWEndor.Models;

namespace SWEndor
{
  public struct SpawnerInfo
  {
    public bool Enabled;

    public float SpawnMoveTime;
    public float NextSpawnTime;

    public string[] SpawnTypes;
    public float SpawnMoveDelay;
    public float SpawnInterval;
    public float SpawnPlayerDelay;
    public int SpawnsRemaining;

    public TV_3DVECTOR[] SpawnLocations;
    public TV_3DVECTOR PlayerSpawnLocation;

    public float SpawnSpeed; // -1 means follow spawner, -2 means maxSpeed of spawned
    public TV_3DVECTOR SpawnRotation;
    public TV_3DVECTOR SpawnManualPositioningMult;
    public TV_3DVECTOR SpawnSpeedPositioningMult;

    public static SpawnerInfo Default = new SpawnerInfo
    {
      Enabled = false,
      SpawnTypes = new string[0],
      SpawnMoveDelay = 4,
      SpawnInterval = 5,
      SpawnPlayerDelay = 10,
      SpawnsRemaining = 30,
      SpawnLocations = new TV_3DVECTOR[0],
      SpawnSpeed = -1,
    };

    internal void Process(Engine engine, ActorInfo ainfo, ActorInfo p)
    {
      UnlockSpawns(engine, ainfo, p);

      foreach (ActorInfo a in ainfo.Children)
        if (!a.UseParentCoords)
          MoveSpawns(engine, a, p);
    }

    private bool SpawnPlayer(Engine engine, ActorInfo ainfo)
    {
      if (!engine.PlayerInfo.RequestSpawn)
        return false;

      ReleaseSpawns(engine, ainfo);

      if (NextSpawnTime < engine.Game.GameTime + SpawnPlayerDelay)
        NextSpawnTime = engine.Game.GameTime + SpawnPlayerDelay;

      engine.PlayerInfo.IsMovementControlsEnabled = false;

      ActorCreationInfo acinfo = new ActorCreationInfo(engine.PlayerInfo.ActorType);

      float scale = ainfo.Scale;
      acinfo.Position = ainfo.GetRelativePositionXYZ(PlayerSpawnLocation.x * scale, PlayerSpawnLocation.y * scale, PlayerSpawnLocation.z * scale);
      acinfo.Rotation = ainfo.GetGlobalRotation();
      acinfo.Rotation += SpawnRotation;

      acinfo.FreeSpeed = true;
      acinfo.Faction = ainfo.Faction;
      ActorInfo a = engine.ActorFactory.Create(acinfo);
      ainfo.AddChild(a);
      a.QueueNext(new Lock());

      a.SetPlayer();
      engine.PlayerCameraInfo.SetPlayerLook();

      if (a.TypeInfo.AIData.TargetType.Has(TargetType.FIGHTER) && a.Faction.WingLimit >= 0)
        a.Faction.WingLimit++;

      if (a.TypeInfo.AIData.TargetType.Has(TargetType.SHIP) && a.Faction.ShipLimit >= 0)
        a.Faction.ShipLimit++;

      engine.PlayerInfo.RequestSpawn = false;

      SpawnMoveTime = engine.Game.GameTime + SpawnMoveDelay;
      return true;
    }

    private bool SpawnFighter(Engine engine, ActorInfo ainfo)
    {
      if (NextSpawnTime < engine.Game.GameTime
       && SpawnsRemaining > 0
       && SpawnTypes != null
       && SpawnTypes.Length > 0)
      {
        ActorTypeInfo spawntype = engine.ActorTypeFactory.Get(SpawnTypes[engine.Random.Next(0, SpawnTypes.Length)]);
        if ((spawntype.AIData.TargetType.Has(TargetType.FIGHTER)
          && (ainfo.Faction.WingSpawnLimit < 0 || ainfo.Faction.WingCount < ainfo.Faction.WingSpawnLimit)
          && (ainfo.Faction.WingLimit < 0 || ainfo.Faction.WingCount < ainfo.Faction.WingLimit)
          )
        || (spawntype.AIData.TargetType.Has(TargetType.SHIP)
          && (ainfo.Faction.ShipSpawnLimit < 0 || ainfo.Faction.WingCount < ainfo.Faction.ShipSpawnLimit))
          && (ainfo.Faction.ShipLimit < 0 || ainfo.Faction.WingCount < ainfo.Faction.ShipLimit)
          )
        {
          NextSpawnTime = engine.Game.GameTime + SpawnInterval;
          SpawnsRemaining--;

          Squadron squad = ainfo.Engine.SquadronFactory.Create();
          foreach (TV_3DVECTOR sv in SpawnLocations)
          {
            ActorCreationInfo acinfo = new ActorCreationInfo(spawntype);

            float scale = ainfo.Scale;
            acinfo.Position = ainfo.GetRelativePositionXYZ(sv.x * scale, sv.y * scale, sv.z * scale);
            acinfo.Rotation = ainfo.GetGlobalRotation();
            acinfo.Rotation += SpawnRotation;

            acinfo.FreeSpeed = true;
            acinfo.Faction = ainfo.Faction;
            ActorInfo a = engine.ActorFactory.Create(acinfo);
            a.Squad = squad;
            ainfo.AddChild(a);
            a.QueueFirst(new Lock());
          }

          SpawnMoveTime = engine.Game.GameTime + SpawnMoveDelay;
        }
      }
      return true;
    }

    private void MoveSpawns(Engine engine, ActorInfo a, ActorInfo p)
    {
      if (SpawnSpeed == -2)
        a.MoveData.Speed = a.MoveData.MaxSpeed;
      else if (SpawnSpeed == -1)
        a.MoveData.Speed = p.MoveData.Speed;
      else
        a.MoveData.Speed = SpawnSpeed;

      a.Rotation += p.Rotation - p.PrevRotation;
      a.Position += p.Position - p.PrevPosition;

      float m1 = p.MoveData.Speed * engine.Game.TimeSinceRender * p.Scale;
      float m2 = engine.Game.TimeSinceRender * p.Scale;
      a.MoveRelative(SpawnSpeedPositioningMult.x * m1
                   , SpawnSpeedPositioningMult.y * m1
                   , SpawnSpeedPositioningMult.z * m1);

      a.MoveRelative(SpawnManualPositioningMult.x * m2
                   , SpawnManualPositioningMult.y * m2
                   , SpawnManualPositioningMult.z * m2);

      if (a.IsPlayer)
        engine.PlayerInfo.IsMovementControlsEnabled = false;
    }

    private void ReleaseSpawns(Engine engine, ActorInfo ainfo)
    {
      foreach (ActorInfo a in ainfo.Children)
      {
        a.MoveData.FreeSpeed = false;
        a.UnlockOne();

        if (a.IsPlayer)
          engine.PlayerInfo.IsMovementControlsEnabled = true;

        ainfo.RemoveChild(a);
      }
    }

    private void UnlockSpawns(Engine engine, ActorInfo ainfo, ActorInfo p)
    {
      if (Enabled
       && !p.IsDead
       && p.Active
       )
      {
        if (SpawnMoveTime < engine.Game.GameTime)
          ReleaseSpawns(engine,  ainfo);

        // Spawn new
        if (!p.IsDying
          && !(p.CurrentAction is HyperspaceIn || p.CurrentAction is HyperspaceOut))
        {
          SpawnPlayer(engine, ainfo);
          SpawnFighter(engine, ainfo);
        }
      }
    }
  }
}
