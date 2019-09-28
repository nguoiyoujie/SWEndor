using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.Core;
using SWEndor.Player;
using SWEndor.Scenarios;
using SWEndor.Sound;

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

    public Session Game { get { return Engine.Game; } }
    public GameScenarioManager GameScenarioManager { get { return Engine.GameScenarioManager; } }
    public TrueVision TrueVision { get { return Engine.TrueVision; } }
    public Font.Factory FontFactory { get { return Engine.FontFactory; } }
    public ActorInfo.Factory<ActorInfo> ActorFactory { get { return Engine.ActorFactory; } }
    public ActorTypeInfo.Factory ActorTypeFactory { get { return Engine.ActorTypeFactory; } }
    public SoundManager SoundManager { get { return Engine.SoundManager; } }
    public LandInfo LandInfo { get { return Engine.LandInfo; } }
    public AtmosphereInfo AtmosphereInfo { get { return Engine.AtmosphereInfo; } }
    public PlayerInfo PlayerInfo { get { return Engine.PlayerInfo; } }
    public PlayerCameraInfo PlayerCameraInfo { get { return Engine.PlayerCameraInfo; } }
    public Screen2D Screen2D { get { return Engine.Screen2D; } }
    public Scenarios.Scripting.Expressions.Context ScriptContext { get { return Engine.ScriptContext; } }

    public TVScreen2DImmediate TVScreen2DImmediate { get { return Engine.TrueVision.TVScreen2DImmediate; } }
    public TVScreen2DText TVScreen2DText { get { return Engine.TrueVision.TVScreen2DText; } }

    // shortcut
    protected TV_COLOR pcolor { get { return PlayerInfo.FactionColor; } }
  }
}
