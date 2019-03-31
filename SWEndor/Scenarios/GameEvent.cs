using System;
using System.Collections.Generic;

namespace SWEndor.Scenarios
{
  //public enum EventType { ACTOR_EVENT, SCENARIO_EVENT, MESSAGE_EVENT }

  public delegate void ScenarioEvent(object[] parameters);

  public class GameEvent
  {
    private static Dictionary<string, GameEvent> EventList = new Dictionary<string, GameEvent>();

    public string Name { get; private set; }
    public ScenarioEvent Method { get; private set; }

    private GameEvent() { }

    public static void RegisterEvent(string name, ScenarioEvent method)
    {
      GameEvent ge = new GameEvent();
      ge.Name = name;
      ge.Method = method;
      
      if (method != null)
      {
        if (!EventList.ContainsKey(name))
          EventList.Add(name, ge);
        else
          EventList[name] = ge;
      }
    }

    public static void ClearEvents()
    {
      EventList.Clear();
    }

    public static GameEvent GetEvent(string name)
    {
      if (EventList.ContainsKey(name))
        return EventList[name];
      else
        throw new Exception(string.Format("Event {0} does not exist!", name));
    }

    public static void RunEvent(string name, object[] parameters)
    {
      if (EventList.ContainsKey(name))
      {
        EventList[name].Method(parameters);
      }
      else
        throw new Exception(string.Format("Event {0} does not exist!", name));
    }
  }
}
