﻿using MTV3D65;
using Primrose.Primitives.Factories;
using SWEndor.Game.Actors;
using System.Collections.Generic;

namespace SWEndor.Game.Scenarios
{
  public class ScenarioStates
  {
    // Setup states
    public string Difficulty;
    public int StageNumber;
    public bool Launched = false;

    // Runtime states
    public bool IsCutsceneMode = false;

    public TV_3DVECTOR MaxBounds = new TV_3DVECTOR(20000, 1500, 20000);
    public TV_3DVECTOR MinBounds = new TV_3DVECTOR(-20000, -1500, -20000);
    public TV_3DVECTOR MaxAIBounds = new TV_3DVECTOR(20000, 1500, 20000);
    public TV_3DVECTOR MinAIBounds = new TV_3DVECTOR(-20000, -1500, -20000);

    public bool InformLostWing = false;
    public bool InformLostShip = false;
    public bool InformLostStructure = false;
    public float TimeSinceLostWing = -100;
    public float TimeSinceLostShip = -100;
    public float TimeSinceLostStructure = -100;
    internal float LastLostSoundTime = 0;

    public Registry<string, float> GameStatesF = new Registry<string, float>();
    public Registry<string, string> GameStatesS = new Registry<string, string>();
    public Registry<string, bool> GameStatesB = new Registry<string, bool>();

    // Registries for showing on the sidebar
    public HashSet<ActorInfo> MonitorAllies = new HashSet<ActorInfo>();
    public HashSet<ActorInfo> MonitorEnemies = new HashSet<ActorInfo>();

    public Octree Octree = new Octree(new TV_3DVECTOR(), 100000, 10);

    public List<MessageLog> MessageLogs = new List<MessageLog>();

    public bool GetGameStateB(string key) { return GameStatesB.Get(key); }
    public float GetGameStateF(string key) { return GameStatesF.Get(key); }
    public string GetGameStateS(string key) { return GameStatesS.Get(key); }
    public bool GetGameStateB(string key, bool defaultValue) { return GameStatesB.GetD(key, defaultValue); }
    public float GetGameStateF(string key, float defaultValue) { return GameStatesF.GetD(key, defaultValue); }
    public string GetGameStateS(string key, string defaultValue) { return GameStatesS.GetD(key, defaultValue); }
    public void SetGameStateB(string key, bool value) { GameStatesB.Put(key, value); }
    public void SetGameStateF(string key, float value) { GameStatesF.Put(key, value); }
    public void SetGameStateS(string key, string value) { GameStatesS.Put(key, value); }

    public void Reset()
    {
      MaxBounds = new TV_3DVECTOR(20000, 1500, 20000);
      MinBounds = new TV_3DVECTOR(-20000, -1500, -20000);
      MaxAIBounds = new TV_3DVECTOR(20000, 1500, 20000);
      MinAIBounds = new TV_3DVECTOR(-20000, -1500, -20000);

      InformLostWing = false;
      InformLostShip = false;
      InformLostStructure = false;
      TimeSinceLostWing = -100;
      TimeSinceLostShip = -100;
      TimeSinceLostStructure = -100;
      LastLostSoundTime = 0;

      MessageLogs.Clear();
      GameStatesF.Clear();
      GameStatesB.Clear();
      GameStatesS.Clear();
      MonitorAllies.Clear();
      MonitorEnemies.Clear();
    }
  }
}
