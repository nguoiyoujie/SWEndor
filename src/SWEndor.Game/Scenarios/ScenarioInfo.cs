using SWEndor.Game.ActorTypes;
using SWEndor.Game.Sound;

namespace SWEndor.Game.Scenarios
{
  public class ScenarioInfo
  {
    private static readonly ActorTypeInfo[] _defaultAllowedWings = new ActorTypeInfo[0];
    private static readonly string[] _defaultAllowedDifficulties = new string[] { "normal" };

    public string Name;
    public string Description;
    public ActorTypeInfo[] AllowedWings;
    public string[] AllowedDifficulties;
    public string Music_Win;
    public string Music_Lose;

    public ScenarioInfo InitDefault()
    {
      Name = "Untitled Scenario";
      Description = "";
      AllowedWings = _defaultAllowedWings;
      AllowedDifficulties = _defaultAllowedDifficulties;
      Music_Win = MusicGlobals.DefaultWin;
      Music_Lose = MusicGlobals.DefaultLose;
      return this;
    }
  }
}
