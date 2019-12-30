using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.Core;
using SWEndor.Explosions;
using SWEndor.FileFormat.INI;
using SWEndor.Models;
using System.IO;

namespace SWEndor.ExplosionTypes
{
  public partial class ExplosionTypeInfo : ITypeInfo<ExplosionInfo>
  {
    private const string sTimedLife = "TimedLife";
    private const string sShake = "Shake";
    private const string sRender = "Render";
    private const string sExplRender = "ExplRender";
    private const string sMesh = "Mesh";
    private const string sSound = "Sound";

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
    internal SoundData SoundData = SoundData.Default;

    public ComponentMask Mask { get; } = ComponentMask.EXPLOSION;

    public void LoadFromINI(string id)
    {
      ID = id;
      string filepath = Path.Combine(Globals.ExplosionTypeINIDirectory, id + ".ini");

      if (File.Exists(filepath))
      {
        INIFile f = new INIFile(filepath);
        Name = f.GetString("General", "Name", Name);

        TimedLifeData.LoadFromINI(f, sTimedLife);
        ShakeData.LoadFromINI(f, sShake);
        RenderData.LoadFromINI(f, sRender);
        ExplRenderData.LoadFromINI(f, sExplRender);
        MeshData.LoadFromINI(Engine, f, sMesh);
        SoundData.LoadFromINI(f, sSound);
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

      TimedLifeData.SaveToINI(f, sTimedLife);
      ShakeData.SaveToINI(f, sShake);
      RenderData.SaveToINI(f, sRender);
      ExplRenderData.SaveToINI(f, sExplRender);
      MeshData.SaveToINI(f, sMesh);
      SoundData.SaveToINI(f, sSound);

      f.SaveFile(filepath);
    }

    public void Init()
    {
    }

    public virtual void Initialize(Engine engine, ExplosionInfo ainfo)
    {
      // Sound
      SoundData.ProcessInitial(engine, ainfo);

      // Anim
      ainfo.AnimInfo.CyclePeriod = (ExplRenderData.AnimDuration == 0) ? TimedLifeData.TimedLife : ExplRenderData.AnimDuration;

      // Shake
      ShakeData.Process(engine, ainfo.Position);
    }

    public virtual void ProcessState(Engine engine, ExplosionInfo ainfo)
    {
      // sound
      if (engine.PlayerInfo.Actor != null && ainfo.Active)
        SoundData.Process(engine, ainfo);

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
