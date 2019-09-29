using MTV3D65;
using SWEndor.Core;
using SWEndor.Player;
using SWEndor.Scenarios;

namespace SWEndor.UI
{
  public class Widget
  {
    public Widget(Screen2D owner, string name)
    {
      Owner = owner;
      Name = name;
    }

    public virtual bool Visible { get; }
    public virtual string Name { get; }
    public virtual void Draw() { }

    public readonly Screen2D Owner;
    public Engine Engine { get { return Owner.Engine; } }

    public GameScenarioManager GameScenarioManager { get { return Engine.GameScenarioManager; } }
    public Font.Factory FontFactory { get { return Engine.FontFactory; } }
    public PlayerInfo PlayerInfo { get { return Engine.PlayerInfo; } }
    public PlayerCameraInfo PlayerCameraInfo { get { return Engine.PlayerCameraInfo; } }

    public TVScreen2DImmediate TVScreen2DImmediate { get { return Engine.TrueVision.TVScreen2DImmediate; } }
    public TVScreen2DText TVScreen2DText { get { return Engine.TrueVision.TVScreen2DText; } }

    // shortcut
    protected TV_COLOR pcolor { get { return PlayerInfo.FactionColor; } }
  }
}
