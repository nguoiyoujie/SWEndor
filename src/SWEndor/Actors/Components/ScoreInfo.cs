﻿using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.Models;
using Primrose.Primitives;
using Primrose.Primitives.Extensions;
using SWEndor.Scenarios;

namespace SWEndor
{
  internal class ScoreInfo
  {
    public static readonly ScoreInfo Player = new ScoreInfo();

    public float Score { get; private set; }
    public int Hits { get; private set; }
    public int Kills { get; private set; }
    public float DamageDealt { get; private set; }
    public int Deaths { get; private set; }
    public float DamageTaken { get; private set; }
    public ThreadSafeDictionary<string, float> DamageDealtByName { get; private set; }
    public ThreadSafeDictionary<string, int> KillsByName { get; private set; }
    public ThreadSafeDictionary<string, float> DamageTakenByName { get; private set; }
    public ThreadSafeDictionary<string, int> KilledByName { get; private set; }

    public ScoreInfo()
    {
      Score = 0;
      Hits = 0;
      Kills = 0;
      DamageDealt = 0;
      Deaths = 0;
      DamageTaken = 0;
      DamageDealtByName = new ThreadSafeDictionary<string, float>();
      KillsByName = new ThreadSafeDictionary<string, int>();
      DamageTakenByName = new ThreadSafeDictionary<string, float>();
      KilledByName = new ThreadSafeDictionary<string, int>();
    }

    public void Reset()
    {
      Score = 0;
      Hits = 0;
      Kills = 0;
      DamageDealt = 0;
      Deaths = 0;
      DamageTaken = 0;
      DamageDealtByName.Clear();
      KillsByName.Clear();
      DamageTakenByName.Clear();
      KilledByName.Clear();
    }

    private void Increment(ThreadSafeDictionary<string, float> data, string key, float value)
    {
      if (!data.ContainsKey(key))
        data.Add(key, value);
      else
        data[key] += value;
    }

    private void Increment(ThreadSafeDictionary<string, int> data, string key, int value)
    {
      if (!data.ContainsKey(key))
        data.Add(key, value);
      else
        data[key] += value;
    }

    public void AddDirect(float score)
    {
      Score += score;
    }

    public void AddHit(Engine engine, ActorInfo victim, float damage)
    {
      Hits++;
      DamageDealt += damage;
      Increment(DamageDealtByName, victim.Name, damage);
      Score += victim.TypeInfo.ScoreData.PerStrength * damage;

      if (engine.PlayerInfo.Actor != null)
        engine.Screen2D.MessageSystemsText(TextLocalization.Get(TextLocalKeys.ENEMY_HIT)
                                         , 0.5f
                                         , engine.PlayerInfo.FactionColor
                                         , -99);
    }

    public void AddKill(Engine engine, ActorInfo victim)
    {
      Kills++;

      if (victim.TypeInfo.AIData.TargetType.Has(TargetType.FIGHTER))
        engine.GameScenarioManager.Scenario.Mood = MoodStates.DESTROY_FIGHTER;
      else if (victim.TypeInfo.AIData.TargetType.Has(TargetType.SHIP))
        engine.GameScenarioManager.Scenario.Mood = MoodStates.DESTROY_SHIP;

      if (engine.PlayerInfo.Actor != null)
        engine.Screen2D.MessageSystemsText(TextLocalization.Get(TextLocalKeys.ENEMY_DESTROYED).F(victim.Name)
                                         , 2
                                         , ColorLocalization.Get(ColorLocalKeys.GAME_MESSAGE_NORMAL));

      Increment(KillsByName, victim.Name, 1);
      Score += victim.TypeInfo.ScoreData.DestroyBonus;
    }

    public void AddDamage(Engine engine, ActorInfo hitby, float damage)
    {
      DamageTaken += damage;
      if (hitby != null)
        Increment(DamageTakenByName, hitby.Name, damage);
    }

    public void AddDeath(Engine engine, ActorInfo killedby)
    {
      Deaths++;
      //engine.GameScenarioManager.Scenario.Mood = -2;
      if (killedby != null)
        Increment(KilledByName, killedby.Name, 1);
    }
  }
}