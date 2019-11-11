using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.Core;
using SWEndor.Explosions;
using SWEndor.FileFormat.INI;
using SWEndor.Models;
using SWEndor.Player;
using SWEndor.Scenarios;
using System.IO;

namespace SWEndor.ExplosionTypes
{
  public partial class ExplosionTypeInfo : ITypeInfo<ExplosionInfo>
  {
    public ExplosionTypeInfo(Factory owner, string id, string name)
    {
      ActorTypeFactory = owner;
      if (id.Length > 0) { ID = id; }
      if (name.Length > 0) { Name = name; }
    }

    internal readonly Factory ActorTypeFactory;
    internal Engine Engine { get { return ActorTypeFactory.Engine; } }

    // Basic Info
    public string ID;
    public string Name;

    // Data
    internal TimedLifeData TimedLifeData;
    internal ShakeData ShakeData;
    internal RenderData RenderData = RenderData.Default;
    internal ExplRenderData ExplRenderData;
    internal MeshData MeshData = MeshData.Default;

    // Sound
    internal SoundSourceData[] InitialSoundSources = new SoundSourceData[0];
    internal SoundSourceData[] SoundSources = new SoundSourceData[0];

    public void LoadFromINI(string id)
    {
      ID = id;
      string filepath = Path.Combine(Globals.ExplosionTypeINIDirectory, id + ".ini");

      if (File.Exists(filepath))
      {
        INIFile f = new INIFile(filepath);
        Name = f.GetString("General", "Name", Name);

        TimedLifeData.LoadFromINI(f, "TimedLifeData");
        ShakeData.LoadFromINI(f, "ShakeData");
        RenderData.LoadFromINI(f, "RenderData");
        ExplRenderData.LoadFromINI(f, "ExplRenderData");
        MeshData.LoadFromINI(f, "MeshData");

        SoundSourceData.LoadFromINI(f, "SoundSourceData", "InitialSoundSources", out InitialSoundSources);
        SoundSourceData.LoadFromINI(f, "SoundSourceData", "SoundSources", out SoundSources);
      }
    }

    public void SaveToINI(string id)
    {
      ID = id;
      string filepath = Path.Combine(Globals.ExplosionTypeINIDirectory, id + ".ini");

      if (!File.Exists(filepath))
        File.Create(filepath).Close();

      INIFile f = new INIFile(filepath);

      f.SetString("General", "Name", Name);

      TimedLifeData.SaveToINI(f, "TimedLifeData");
      ShakeData.SaveToINI(f, "ShakeData");
      RenderData.SaveToINI(f, "RenderData");
      ExplRenderData.SaveToINI(f, "ExplRenderData");
      MeshData.SaveToINI(f, "MeshData");

      SoundSourceData.SaveToINI(f, "SoundSourceData", "InitialSoundSources", "ISN", InitialSoundSources);
      SoundSourceData.SaveToINI(f, "SoundSourceData", "SoundSources", "SND", SoundSources);
      f.SaveFile(filepath);
    }

    public void Init()
    {
    }

    public virtual void Initialize(Engine engine, ExplosionInfo ainfo)
    {
      // Sound
      foreach (SoundSourceData assi in InitialSoundSources)
        assi.Process(engine, ainfo);

      // Anim
      ainfo.AnimInfo.CyclePeriod = (ExplRenderData.AnimDuration == 0) ? TimedLifeData.TimedLife : ExplRenderData.AnimDuration;

      // Shake
      ShakeData.Process(engine, ainfo.Position);
    }

    public virtual void ProcessState(Engine engine, ExplosionInfo ainfo)
    {
      // sound
      if (engine.PlayerInfo.Actor != null && ainfo.Active)
        foreach (SoundSourceData assi in SoundSources)
          assi.Process(engine, ainfo);

      // billboard
      TV_3DVECTOR pos = ainfo.Engine.PlayerCameraInfo.Camera.GetWorldPosition(new TV_3DVECTOR(0, 0, -1000));
      ainfo.LookAt(pos);

      // anim
      ExplRenderData.Process(engine, ainfo);

      // check parent
      if (ainfo.AttachedActorID > -1)
      {
        ActorInfo p = engine.ActorFactory.Get(ainfo.AttachedActorID);
        if (p == null)
          ainfo.SetState_Dead();
      }
    }
  }
}
