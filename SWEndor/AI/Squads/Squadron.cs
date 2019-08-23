using SWEndor.Actors;
using SWEndor.AI.Actions;
using System.Collections.Generic;

namespace SWEndor.AI.Squads
{
  public partial class Squadron
  {
    public int ID;
    public string Name;
    public LinkedList<ActorInfo> Members = new LinkedList<ActorInfo>();
    public LinkedList<ActorInfo> Threats = new LinkedList<ActorInfo>();

    //public Squadron Parent;
    public Scenarios.GSFunctions.SquadFormation Formation = Scenarios.GSFunctions.SquadFormation.LINE;

    public LinkedList<MissionInfo> Missions = new LinkedList<MissionInfo>();

    public void Process(Engine engine)
    {
      foreach (ActorInfo a in new List<ActorInfo>(Threats))
        if (a == null || a.DisposingOrDisposed || a.IsDyingOrDead)
          Threats.Remove(a);

      /*
      foreach (ActorInfo a in new List<ActorInfo>(Members))
        if (a == null || a.DisposingOrDisposed || a.IsDyingOrDead)
          Members.Remove(a);
          */

      if (Members.Count == 0)
        engine.SquadronFactory.Remove(ID);
    }

    public ActionInfo GetNewAction()
    {
      if (Missions.Count > 0)
        return Missions.First.Value.GetNewAction();

      if (Threats.Count > 0)
        return new Actions.AttackActor(Threats.First.Value.ID);

      return new Actions.Idle();
    }
  }
}
