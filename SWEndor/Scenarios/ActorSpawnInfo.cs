using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.AI.Actions;
using System.Collections.Generic;

namespace SWEndor.Scenarios
{
  public struct ActorSpawnInfo
  {
    public ActorTypeInfo Type;
    public string Name;
    public string RegisterName;
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
      ainfo = ActorInfo.Create(scenario.GetEngine().ActorFactory, acinfo);
      ainfo.SideBarName = SidebarName;

      if (Actions != null)
        foreach (ActionInfo act in Actions)
          scenario.GetEngine().ActionManager.QueueLast(ainfo.ID, act);

      if (Registries != null)
      {
        foreach (string s in Registries)
        {
          Dictionary<string, int> reg = scenario.GetRegister(s);
          if (reg != null)
          {
            if (RegisterName != null && RegisterName != "")
              reg.Add(RegisterName, ainfo.ID);
            else
              reg.Add(ainfo.Key, ainfo.ID);
          }
        }
      }

      scenario.RegisterEvents(ainfo);
      return ainfo;
    }
  }
}
