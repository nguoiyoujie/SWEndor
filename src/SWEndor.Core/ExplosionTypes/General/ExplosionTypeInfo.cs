using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.Core;
using SWEndor.Explosions;
using Primitives.FileFormat.INI;
using SWEndor.Models;
using System.IO;

namespace SWEndor.ExplosionTypes
{
  public partial class ExplosionTypeInfo : ITypeInfo<ExplosionInfo>
  {
    private const string sMesh = "Mesh";

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

    [INIValue("General", "Name")]
    public string Name;

    // Data
    [INIEmbedObject]
    internal TimedLifeData TimedLifeData;

    [INIEmbedObject]
    internal ShakeData ShakeData;

    [INIEmbedObject]
    internal RenderData RenderData = RenderData.Default;

    [INIEmbedObject]
    internal ExplRenderData ExplRenderData;

    [INIEmbedObject]
    internal MeshData MeshData = MeshData.Default;

    [INIEmbedObject]
    internal SoundData SoundData = SoundData.Default;

    public ComponentMask Mask { get; } = ComponentMask.EXPLOSION;

    public void LoadFromINI(string id, string filepath)
    {
      ID = id;
      if (File.Exists(filepath))
      {
        INIFile f = new INIFile(filepath);
        var self = this;
        f.LoadByAttribute(ref self);

        MeshData.Load(Engine, ID);
      }
    }

    public void SaveToINI(string id)
    {
      ID = id;
      string filepath = Path.Combine(Globals.ExplosionTypeINIDirectory, id + ".ini");

      if (!File.Exists(filepath))
        File.Create(filepath).Close();

      INIFile f = new INIFile(filepath);
      var self = this;
      f.UpdateByAttribute(ref self);
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
      if (MeshData.Mode == MeshMode.BILLBOARD_ANIM)
      {
        TV_3DVECTOR pos = ainfo.Engine.PlayerCameraInfo.Camera.GetWorldPosition(new TV_3DVECTOR(0, 0, -1000));
        ainfo.LookAt(pos);
      }

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
