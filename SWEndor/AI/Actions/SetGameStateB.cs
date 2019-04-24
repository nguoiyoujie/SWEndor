using SWEndor.Actors;
using SWEndor.Scenarios;

namespace SWEndor.AI.Actions
{
  public class SetGameStateB : ActionInfo
  {
    public SetGameStateB(string key, bool state) : base("SetGameStateB")
    {
      m_key = key;
      m_state = state;
      CanInterrupt = false;
    }

    private string m_key;
    private bool m_state;

    public override void Process(ActorInfo owner)
    {
      Globals.Engine.GameScenarioManager.SetGameStateB(m_key, m_state);
      Complete = true;
    }
  }
}
