using Primrose.Primitives.Factories;
using Primrose.Primitives.Extensions;
using System.Collections.Generic;
using SWEndor.Game.ActorTypes;

namespace SWEndor.Game.Actors.Components
{
  internal class ScoreInfo
  {
    public static readonly ScoreInfo Player = new ScoreInfo();

    public enum EntryType
    {
      /// <summary>I hit something</summary>
      HIT,

      /// <summary>I kill something</summary>
      KILL,

      /// <summary> I achieved an objective</summary>
      OBJECTIVE_MET,

      /// <summary> I did not achieve an objective</summary>
      OBJECTIVE_FAILED,

      /// <summary>I hit a friendly</summary>
      FRIENDLY_HIT,

      /// <summary>I killed a friendly</summary>
      FRIENDLY_KILL,

      /// <summary>I was hit</summary>
      DAMAGE_TAKEN,

      /// <summary>I was killed</summary>
      DEATH,

      /// <summary>I heard something</summary>
      MESSAGE_LOG,

      /// <summary>I heard something (split)</summary>
      MESSAGE_LOG_SPLIT,
    }

    public struct Entry
    {
      public float GameTime;
      public EntryType Type;
      public string TargetName; // I hit who / I was hit by who / what objective did I complete?
      public ActorTypeInfo TargetType;
      public float DamageValue; // after armor modificiations
      public int ScoreValue; // bonus score

      public Entry(float gameTime, EntryType type, string targetName, ActorTypeInfo targetType, float damageValue, int scoreValue)
      {
        GameTime = gameTime;
        Type = type;
        TargetName = targetName;
        TargetType = targetType;
        DamageValue = damageValue;
        ScoreValue = scoreValue;
      }
    }

    public int Score { get; private set; }
    public int Hits { get; private set; }
    public int FriendlyHits { get; private set; }
    public int Kills { get; private set; }
    public int FriendlyKills { get; private set; }
    public float DamageDealt { get; private set; }
    public float FriendlyDamageDealt { get; private set; }
    public int Deaths { get; private set; }
    public float DamageTaken { get; private set; }
    public readonly Registry<float> DamageDealtByName;
    public readonly Registry<int> KillsByName;
    public readonly Registry<float> FriendlyDamageDealtByName;
    public readonly Registry<int> FriendlyKillsByName;
    public readonly Registry<float> DamageTakenByName;
    public readonly Registry<int> KilledByName;
    public readonly List<Entry> Entries;


    public ScoreInfo()
    {
      Score = 0;
      Hits = 0;
      FriendlyHits = 0;
      Kills = 0;
      FriendlyKills = 0;
      DamageDealt = 0;
      FriendlyDamageDealt = 0;
      Deaths = 0;
      DamageTaken = 0;
      DamageDealtByName = new Registry<float>();
      KillsByName = new Registry<int>();
      FriendlyDamageDealtByName = new Registry<float>();
      FriendlyKillsByName = new Registry<int>();
      DamageTakenByName = new Registry<float>();
      KilledByName = new Registry<int>();
      Entries = new List<Entry>();
    }

    public void Reset()
    {
      Score = 0;
      Hits = 0;
      FriendlyHits = 0;
      Kills = 0;
      FriendlyKills = 0;
      DamageDealt = 0;
      FriendlyDamageDealt = 0;
      Deaths = 0;
      DamageTaken = 0;
      DamageDealtByName.Clear();
      KillsByName.Clear();
      FriendlyDamageDealtByName.Clear();
      FriendlyKillsByName.Clear();
      DamageTakenByName.Clear();
      KilledByName.Clear();
      Entries.Clear();
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

    public void AddDirect(int score)
    {
      Score += score;
    }

    public void AddEntry(Entry entry)
    {
      Entries.Add(entry);
      switch (entry.Type)
      {
        case EntryType.HIT:
          {
            Hits++;
            DamageDealt += entry.DamageValue;
            Score += entry.ScoreValue;
            if (entry.TargetName != null)
            {
              Increment(DamageDealtByName, entry.TargetName, entry.DamageValue);
            }
          }
          break;
        case EntryType.KILL:
          {
            Kills++;
            if (entry.TargetName != null)
            {
              Increment(KillsByName, entry.TargetName, 1);
            }
            Score += entry.ScoreValue;
          }
          break;
        case EntryType.OBJECTIVE_MET:
          {
            Score += entry.ScoreValue;
          }
          break;
        case EntryType.OBJECTIVE_FAILED:
          {
            Score -= entry.ScoreValue;
          }
          break;
        case EntryType.FRIENDLY_HIT:
          {
            FriendlyHits++;
            FriendlyDamageDealt += entry.DamageValue;
            if (entry.TargetName != null)
            {
              Increment(FriendlyDamageDealtByName, entry.TargetName, entry.DamageValue);
            }
            Score -= entry.ScoreValue;
          }
          break;
        case EntryType.FRIENDLY_KILL:
          {
            FriendlyKills++;
            if (entry.TargetName != null)
            {
              Increment(FriendlyKillsByName, entry.TargetName, 1);
            }
            Score -= entry.ScoreValue;
          }
          break;
        case EntryType.DAMAGE_TAKEN:
          {
            DamageTaken += entry.DamageValue;
            Score -= entry.ScoreValue;
            if (entry.TargetName != null)
            {
              Increment(DamageTakenByName, entry.TargetName, entry.DamageValue);
            }
          }
          break;
        case EntryType.DEATH:
          {
            Deaths++;
            Score -= entry.ScoreValue;
            if (entry.TargetName != null)
            {
              Increment(KilledByName, entry.TargetName, 1);
            }
          }
          break;
        case EntryType.MESSAGE_LOG:
          {
            if (!string.IsNullOrEmpty(entry.TargetName) && entry.TargetName.Length > 62 && entry.TargetName.Contains(" "))
            {
              Entries.RemoveAt(Entries.Count - 1);
              int i = 0;
              foreach (string line in entry.TargetName.SplitToLines(62))
              {
                Entry e0 = entry;
                e0.TargetName = line;
                if (i > 0)
                {
                  e0.Type = EntryType.MESSAGE_LOG_SPLIT; 
                }
                AddEntry(e0);
                i++;
              }
            }
          }
          break;
        default:
          break;
      }
    }
  }
}
