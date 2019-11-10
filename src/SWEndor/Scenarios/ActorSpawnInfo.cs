using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.Models;

namespace SWEndor.Scenarios
{
  public struct ActorSpawnInfo
  {
    public ActorTypeInfo Type;
    public string Name;
    public string Obsolete_var;
    public string SidebarName;
    public float SpawnTime;
    public FactionInfo Faction;
    public TV_3DVECTOR Position;
    public TV_3DVECTOR Rotation;
    public ActionInfo[] Actions;
    public string[] Registries;

    public ActorInfo Spawn(GameScenarioBase scenario)
    {
      ActorCreationInfo acinfo;
      ActorInfo ainfo;

      acinfo = new ActorCreationInfo(Type);
      if (Name != null && Name != "")
        acinfo.Name = Name;
      acinfo.Faction = Faction;
      acinfo.InitialState = ActorState.NORMAL;
      acinfo.CreationTime = SpawnTime;
      acinfo.Position = Position;
      acinfo.Rotation = Rotation;
      ainfo = scenario.Engine.ActorFactory.Create(acinfo);
      ainfo.SideBarName = SidebarName;

      if (Actions != null)
        foreach (ActionInfo act in Actions)
          ainfo.QueueLast(act);

      if (Registries != null)
        foreach (string s in Registries)
          scenario.GetRegister(s)?.Add(ainfo);

      return ainfo;
    }
  }
}
