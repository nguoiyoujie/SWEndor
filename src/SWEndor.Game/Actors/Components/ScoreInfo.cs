using SWEndor.Game.Core;
using SWEndor.Game.Models;
using Primrose.Primitives.Extensions;
using Primrose.Primitives.Factories;
using SWEndor.Game.Sound;

namespace SWEndor.Game.Actors.Components
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
    public Registry<float> DamageDealtByName { get; private set; }
    public Registry<int> KillsByName { get; private set; }
    public Registry<float> DamageTakenByName { get; private set; }
    public Registry<int> KilledByName { get; private set; }

    public ScoreInfo()
    {
      Score = 0;
      Hits = 0;
      Kills = 0;
      DamageDealt = 0;
      Deaths = 0;
      DamageTaken = 0;
      DamageDealtByName = new Registry<float>();
      KillsByName = new Registry<int>();
      DamageTakenByName = new Registry<float>();
      KilledByName = new Registry<int>();
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

    private void Increment(Registry<float> data, string key, float value)
    {
      if (!data.Contains(key))
        data.Add(key, value);
      else
        data[key] += value;
    }

    private void Increment(Registry<int> data, string key, int value)
    {
      if (!data.Contains(key))
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
        engine.SoundManager.SetMood(MoodState.DESTROY_FIGHTER);
      else if (victim.TypeInfo.AIData.TargetType.Has(TargetType.SHIP))
        engine.SoundManager.SetMood(MoodState.DESTROY_SHIP);

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
      //engine.SoundManager.Mood = -2;
      if (killedby != null)
        Increment(KilledByName, killedby.Name, 1);
    }
  }
}
