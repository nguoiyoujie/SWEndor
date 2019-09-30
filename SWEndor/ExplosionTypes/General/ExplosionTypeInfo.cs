using SWEndor.ActorTypes.Components;
using SWEndor.Core;
using SWEndor.Explosions;
using SWEndor.Models;
using SWEndor.Player;
using SWEndor.Scenarios;

namespace SWEndor.ExplosionTypes
{
  public partial class ExplosionTypeInfo : ITypeInfo<ExplosionInfo>
  {
    public ExplosionTypeInfo(Factory owner, string name = "")
    {
      ActorTypeFactory = owner;
      if (name.Length > 0) { Name = name; }
    }

    public readonly Factory ActorTypeFactory;
    public Engine Engine { get { return ActorTypeFactory.Engine; } }

    public Session Game { get { return Engine.Game; } }
    public GameScenarioManager GameScenarioManager { get { return Engine.GameScenarioManager; } }
    public TrueVision TrueVision { get { return Engine.TrueVision; } }
    public LandInfo LandInfo { get { return Engine.LandInfo; } }
    public AtmosphereInfo AtmosphereInfo { get { return Engine.AtmosphereInfo; } }
    public PlayerInfo PlayerInfo { get { return Engine.PlayerInfo; } }
    public PlayerCameraInfo PlayerCameraInfo { get { return Engine.PlayerCameraInfo; } }
    public Screen2D Screen2D { get { return Engine.Screen2D; } }

    // Basic Info
    public string Name;

    // Data
    public TimedLifeData TimedLifeData;
    public RenderData RenderData = RenderData.Default;
    public MeshData MeshData = MeshData.Default;
    
    // Sound
    public SoundSourceData[] InitialSoundSources = new SoundSourceData[0];
    public SoundSourceData[] SoundSources = new SoundSourceData[0];

    public void LoadFromINI()
    {
      /*
      string filepath = Path.Combine(Globals.ActorTypeINIDirectory, Name + ".ini");

      if (File.Exists(filepath))
      {
        INIFile f = new INIFile(filepath);
        RenderData.LoadFromINI(f, "RenderData");
      }
      else
      {
        File.Create(filepath).Close();
        INIFile f = new INIFile(filepath);
        RenderData.SaveToINI(f, "RenderData");
        f.SaveFile(filepath);
      }
      */
    }

    public void Init()
    {
    }

    public virtual void Initialize(Engine engine, ExplosionInfo ainfo)
    {
      // Sound
      //if (!(engine.GameScenarioManager?.Scenario is GSMainMenu))
        foreach (SoundSourceData assi in InitialSoundSources)
          assi.Process(engine, ainfo);
    }

    public virtual void ProcessState(Engine engine, ExplosionInfo ainfo)
    {
      // sound
      if (PlayerInfo.Actor != null
        && ainfo.Active) 
        //&& !(engine.GameScenarioManager?.Scenario is GSMainMenu))
      {
        foreach (SoundSourceData assi in SoundSources)
          assi.Process(engine, ainfo);
      }
    }
  }
}
