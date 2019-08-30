using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.Input.Functions;
using SWEndor.Player;
using SWEndor.Scenarios;
using SWEndor.Sound;

namespace SWEndor.Input.Context
{
  public abstract class AInputContext
  {
    protected AInputContext(InputManager manager) { Manager = manager; }

    InputManager Manager;
    public Engine Engine { get { return Manager.Engine; } }

    public Game Game { get { return Engine.Game; } }
    public GameScenarioManager GameScenarioManager { get { return Engine.GameScenarioManager; } }
    public TrueVision TrueVision { get { return Engine.TrueVision; } }
    public ActorInfo.Factory ActorFactory { get { return Engine.ActorFactory; } }
    public ActorTypeInfo.Factory ActorTypeFactory { get { return Engine.ActorTypeFactory; } }
    public LandInfo LandInfo { get { return Engine.LandInfo; } }
    public AtmosphereInfo AtmosphereInfo { get { return Engine.AtmosphereInfo; } }
    public SoundManager SoundManager { get { return Engine.SoundManager; } }
    public PlayerInfo PlayerInfo { get { return Engine.PlayerInfo; } }
    public PlayerCameraInfo PlayerCameraInfo { get { return Engine.PlayerCameraInfo; } }
    public Screen2D Screen2D { get { return Engine.Screen2D; } }

    public virtual void Set()
    {
      foreach (InputFunction fn in InputFunction.Registry.Functions)
        if (fn != null)
          fn.Enabled = false;
    }
    
    public virtual void HandleKeyBuffer(TV_KEYDATA keydata) { }
    public virtual void HandleKeyState(byte[] keyPressedStates) { }
    public virtual void HandleMouse(int mouseX, int mouseY, bool button1, bool button2, bool button3, bool button4, int mouseScroll) { }
  }
}
