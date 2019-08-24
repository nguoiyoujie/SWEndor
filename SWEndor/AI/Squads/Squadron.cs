using SWEndor.Actors;
using SWEndor.AI.Actions;
using System.Collections;
using System.Collections.Generic;

namespace SWEndor.AI.Squads
{
  public partial class Squadron
  {
    public int ID;
    public string Name;
    private LinkedList<ActorInfo> _members = new LinkedList<ActorInfo>();
    private LinkedList<ActorInfo> _threats = new LinkedList<ActorInfo>();

    //public Squadron Parent;
    public Scenarios.GSFunctions.SquadFormation Formation = Scenarios.GSFunctions.SquadFormation.LINE;

    public LinkedList<MissionInfo> Missions = new LinkedList<MissionInfo>();

    public void Process(Engine engine)
    {
      foreach (ActorInfo a in new List<ActorInfo>(_threats))
        if (a == null || a.DisposingOrDisposed || a.IsDyingOrDead || a.Faction.Allies.Contains(Leader.Faction))
          _threats.Remove(a);

      if (_members.Count == 0)
        engine.SquadronFactory.Remove(ID);
    }

    public ActionInfo GetNewAction()
    {
      if (Missions.Count > 0)
        return Missions.First.Value.GetNewAction();

      if (_threats.Count > 0)
        return new Actions.AttackActor(_threats.First.Value.ID);

      return new Actions.Idle();
    }

    public void Add(ActorInfo a, bool isLeader = false)
    {
      a.Squad?.Remove(a);
      a.Squad = this;
      if (isLeader)
        _members.AddFirst(a);
      else
        _members.AddLast(a);
    }

    public void Remove(ActorInfo a)
    {
      a.Squad = null;
      _members.Remove(a);
    }

    public void AddThreat(ActorInfo a, bool priorityThreat = false)
    {
      if (priorityThreat)
        _threats.AddFirst(a);
      else
        _threats.AddLast(a);
    }

    public void RemoveThreat(ActorInfo a)
    {
      _threats.Remove(a);
    }


    public IEnumerable<ActorInfo> Members { get { return _members; } }
    public IEnumerable<ActorInfo> Threats { get { return _threats; } }
    public ActorInfo Leader { get { return (_members.Count > 0) ? _members.First.Value : null; } }
  }
}
