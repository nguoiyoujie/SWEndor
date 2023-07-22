using SWEndor.Game.Actors;
using SWEndor.Game.AI.Actions;
using SWEndor.Game.Core;
using Primrose.Primitives;
using Primrose.Primitives.Extensions;
using System;
using System.Collections.Generic;
using SWEndor.Game.Models;

namespace SWEndor.Game.AI.Squads
{
  public partial class Squadron
  {
    public int ID;
    public string Name;
    public bool IsNamedSquadron { get { return Name != null; } }
    private LinkedList<ActorInfo> _members = new LinkedList<ActorInfo>();
    private LinkedList<ActorInfo> _threats = new LinkedList<ActorInfo>();
    private object lockthreat = new object();
    private object lockmember = new object();
    public MissionInfo Mission;

    public static readonly Squadron Neutral = new Squadron();
    public bool IsNull { get { return this == Neutral; } }

    private static Predicate<ActorInfo> rm_func = (a) => a == null || a.DisposingOrDisposed || a.IsDyingOrDead; // || (Leader != null && a.Faction.Allies.Contains(Leader.Faction));
    public void Process(Engine engine)
    {
      if (IsNull)
        return;

      lock (lockthreat)
        _threats.RemoveAll(rm_func);
    }

    public ActionInfo GetNewAction(ActorInfo actor, Engine engine)
    {
      if (IsNull)
        return Idle.GetOrCreate();

      if (Mission != null)
      {
        Mission.Process(engine, this);
        if (!Mission.Complete)
          return Mission.GetNewAction(engine, this);
      }

      lock (lockthreat)
      {
        ActorInfo threat = GetThreatFirst(engine);
        for (int t = 0; t < 16; t++)
        {
          if (threat == null)
            break;

          // verify that you can shoot this threat
          foreach (Weapons.WeaponShotInfo weapon in actor.WeaponDefinitions.AIWeapons)
          {
            if (weapon.Weapon.CanTarget(threat))
              return AttackActor.GetOrCreate(_threats.First.Value.ID);
          }
          threat = threat.Next;
        }
      }
      return Idle.GetOrCreate();
    }

    public ActorInfo GetThreatFirst(Engine engine)
    {
      if (IsNull)
        return null;

      lock (lockthreat)
        if (_threats.Count > 0)
          return _threats.First.Value;

      return null;
    }

    public ActorInfo GetThreatRandom(Engine engine)
    {
      if (IsNull)
        return null;

      lock (lockthreat)
        if (_threats.Count > 0)
          return _threats.Random(engine.Random);

      return null;
    }

    public ActorInfo GetMemberRandom(Engine engine)
    {
      if (IsNull)
        return null;

      lock (lockmember)
        if (_members.Count > 0)
          return _members.Random(engine.Random);

      return null;
    }

    public void Add(ActorInfo a, bool isLeader = false)
    {
      lock (lockmember)
        if (_members.Contains(a))
          return;

      if (isLeader)
        lock (lockmember)
          _members.AddFirst(a);
      else
        lock (lockmember)
          _members.AddLast(a);
    }

    public void Remove(ActorInfo a)
    {
      if (IsNull)
        return;

      lock (lockmember)
        _members.Remove(a);

      if (_members.Count == 0)
        a.Engine.SquadronFactory.Return(this);
    }

    public void Join(Squadron destination)
    {
      foreach (ActorInfo a in Members) //MembersCopy
        a.Squad = destination;
    }

    public void MakeLeader(ActorInfo a)
    {
      if (!IsNull && Leader != a)
      {
        Remove(a);
        Add(a, true);
      }
    }

    public void AddThreat(ActorInfo a, bool priorityThreat = false)
    {
      if (IsNull)
        return;

      if (priorityThreat)
        lock (lockthreat)
          _threats.AddFirst(a);
      else
        lock (lockthreat)
          _threats.AddLast(a);
    }

    public void RemoveThreat(ActorInfo a)
    {
      if (IsNull)
        return;

      lock (lockthreat)
        _threats.Remove(a);
    }

    public void Reset()
    {
      if (IsNull)
        return;

      lock (lockthreat)
        _threats.Clear();

      lock (lockmember)
      {
        foreach (ActorInfo a in _members)
          a.Squad = null;

        _members.Clear();
      }
    }

    private ActorInfo[] _empty = new ActorInfo[0];
    public LinkedListEnumerable<ActorInfo> Members { get { if (IsNull) return LinkedListEnumerable<ActorInfo>.Empty; else return new LinkedListEnumerable<ActorInfo>(_members); } }
    public ActorInfo[] MembersCopy
    {
      get
      {
        if (IsNull)
          return _empty;
        else
        {
          lock (lockmember)
          {
            ActorInfo[] ret = new ActorInfo[_members.Count];
            _members.CopyTo(ret, 0);
            return ret;
          }
        }
      }
    }
    public LinkedListEnumerable<ActorInfo> Threats { get { if (IsNull) return LinkedListEnumerable<ActorInfo>.Empty; else return new LinkedListEnumerable<ActorInfo>(_threats); } }

    public ActorInfo Leader { get { return (IsNull || _members.Count == 0) ? null : _members.First.Value; } }
  }
}
