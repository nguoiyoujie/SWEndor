using MTV3D65;
using SWEndor.Game.Actors;
using SWEndor.Game.ActorTypes;
using SWEndor.Game.ActorTypes.Components;
using SWEndor.Game.AI;
using SWEndor.Game.AI.Actions;
using SWEndor.Game.Core;
using SWEndor.Game.Models;
using SWEndor.Game.Primitives.Extensions;

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
      SpawnSpeed = -1
    };

    internal void Init(ref SpawnerData data)
    {
      SpawnTypes = data.SpawnTypes;
      SpawnMoveDelay = data.SpawnMoveDelay;
      SpawnInterval = data.SpawnInterval;
      SpawnPlayerDelay = data.SpawnPlayerDelay;
      SpawnsRemaining = data.SpawnsRemaining;

      SpawnLocations = data.SpawnLocations.ToVec3Array();
      PlayerSpawnLocation = data.PlayerSpawnLocation.ToVec3();

      SpawnSpeed = data.SpawnSpeed;
      SpawnRotation = data.SpawnRotation.ToVec3();
      SpawnManualPositioningMult = data.SpawnManualPositioningMult.ToVec3();
      SpawnSpeedPositioningMult = data.SpawnSpeedPositioningMult.ToVec3();
    }

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

      //ReleaseSpawns(engine, ainfo);

      if (NextSpawnTime < engine.Game.GameTime + SpawnPlayerDelay)
        NextSpawnTime = engine.Game.GameTime + SpawnPlayerDelay;

      engine.PlayerInfo.SystemLockMovement = true;

      ActorCreationInfo acinfo = new ActorCreationInfo(engine.PlayerInfo.ActorType);

      float scale = ainfo.Scale;
      acinfo.Position = ainfo.GetRelativePositionXYZ(PlayerSpawnLocation.x * scale, PlayerSpawnLocation.y * scale, PlayerSpawnLocation.z * scale);
      acinfo.Rotation = ainfo.GetGlobalRotation();
      acinfo.Rotation += SpawnRotation;

      acinfo.Name = engine.PlayerInfo.Name;
      acinfo.FreeSpeed = true;
      acinfo.Faction = ainfo.Faction;
      ActorInfo a = engine.ActorFactory.Create(acinfo);
      ainfo.AddChild(a);
      a.QueueNext(Lock.GetOrCreate());

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

    public void QueueFighter(ActorInfo a, ActorInfo p)
    {
      a.SetReserved();
        p.SpawnQueue.Enqueue(a);
    }

    public void DiscardQueuedFighters(ActorInfo p)
    {
      ActorInfo a;
      while (p.SpawnQueue.TryDequeue(out a))
        a.Delete();
    }

    private bool SpawnFighter(Engine engine, ActorInfo ainfo, ActorInfo p)
    {
      if (NextSpawnTime < engine.Game.GameTime
       && SpawnsRemaining > 0
       && SpawnTypes != null
       && SpawnTypes.Length > 0)
      {
        ActorTypeInfo spawntype = engine.ActorTypeFactory.Get(SpawnTypes[engine.Random.Next(0, SpawnTypes.Length)]);

        if (p.SpawnQueue.Count > 0)
        {
          NextSpawnTime = engine.Game.GameTime + SpawnInterval;

          ActorInfo first = null;
          foreach (TV_3DVECTOR sv in SpawnLocations)
          {
            ActorInfo a;
            if (p.SpawnQueue.TryDequeue(out a))
            {
              float scale = ainfo.Scale;
              a.Position = ainfo.GetRelativePositionXYZ(sv.x * scale, sv.y * scale, sv.z * scale);
              a.Rotation = ainfo.GetGlobalRotation();
              a.Rotation += SpawnRotation;
              a.MoveData.FreeSpeed = true;
              if (first == null)
              {
                first = a;
                first.CreateNewSquad();
              }
              else
              {
                a.JoinSquad(first);
              }
              ainfo.AddChild(a);
              a.QueueFirst(Lock.GetOrCreate());
              a.SetUnreserved();
            }
          }
          SpawnMoveTime = engine.Game.GameTime + SpawnMoveDelay;
        }
        else if ((spawntype.AIData.TargetType.Has(TargetType.FIGHTER)
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

          ActorInfo first = null;
          foreach (TV_3DVECTOR sv in SpawnLocations)
          {
            ActorInfo a;
            ActorCreationInfo acinfo = new ActorCreationInfo(spawntype);

            float scale = ainfo.Scale;
            acinfo.Position = ainfo.GetRelativePositionXYZ(sv.x * scale, sv.y * scale, sv.z * scale);
            acinfo.Rotation = ainfo.GetGlobalRotation();
            acinfo.Rotation += SpawnRotation;

            acinfo.FreeSpeed = true;
            acinfo.Faction = ainfo.Faction;
            a = engine.ActorFactory.Create(acinfo);

            if (first == null)
            {
              first = a;
              first.CreateNewSquad();
            }
            else
            {
              a.JoinSquad(first);
            }
            ainfo.AddChild(a);
            a.QueueFirst(Lock.GetOrCreate());
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
        engine.PlayerInfo.SystemLockMovement = true;
    }

    private void ReleaseSpawns(Engine engine, ActorInfo ainfo)
    {
      foreach (ActorInfo a in ainfo.Children)
      {
        a.MoveData.FreeSpeed = false;
        a.UnlockOne();

        if (a.IsPlayer)
        {
          engine.PlayerInfo.SystemLockMovement = false;
          engine.PlayerInfo.PlayerLockMovement = false;
        }

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
          SpawnFighter(engine, ainfo, p);
        }
      }
    }
  }
}
