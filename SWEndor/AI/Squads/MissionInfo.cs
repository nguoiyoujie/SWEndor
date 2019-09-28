using SWEndor.AI.Actions;
using SWEndor.Core;

namespace SWEndor.AI.Squads
{
  public abstract class MissionInfo
  {
    public bool Complete = false;

    public abstract ActionInfo GetNewAction(Engine engine, Squadron squad);
    public abstract void Process(Engine engine, Squadron squad);
  }
}
