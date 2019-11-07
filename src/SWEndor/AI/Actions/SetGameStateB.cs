using SWEndor.Actors;
using SWEndor.Core;

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

    public override void Process(Engine engine, ActorInfo actor)
    {
      engine.GameScenarioManager.SetGameStateB(m_key, m_state);
      Complete = true;
    }
  }
}
