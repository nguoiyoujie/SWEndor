using SWEndor.Actors;
using SWEndor.AI.Actions;
using System;
using System.Collections.Generic;

namespace SWEndor.AI.Squads
{
  public abstract class MissionInfo
  {
    public bool Complete = false;

    public abstract ActionInfo GetNewAction();
    public abstract void Process(Engine engine, Squadron squad);
  }
}
