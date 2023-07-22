using SWEndor.Game.ActorTypes.Components;
using SWEndor.Game.Core;
using Primrose.FileFormat.INI;
using SWEndor.Game.Models;
using System.IO;
using SWEndor.Game.Particles;

namespace SWEndor.Game.ParticleTypes
{
  /// <summary>
  /// Defines a type holding data to create projectile instances.
  /// </summary>
  public partial class ParticleTypeInfo : ITypeInfo<ParticleInfo>
  {
    internal ParticleTypeInfo(Factory owner, string id, string name)
    {
      ActorTypeFactory = owner;
      if (id?.Length > 0) { ID = id; }
      if (name?.Length > 0) { Name = name; }
    }

    internal readonly Factory ActorTypeFactory;

    /// <summary>The Engine that owns this object</summary>
    public Engine Engine { get { return ActorTypeFactory.Engine; } }

    public ComponentMask Mask { get { return ComponentMask.STATIC_ACTOR; } }

    /// <summary>The ID of this object</summary>
    public string ID;

    /// <summary>The given name of this object</summary>
    [INIValue("General", "Name")]
    public string Name;

    // Data (structs)
    [INIEmbedObject("TimedLife")]
    internal TimedLifeData TimedLifeData;

    [INIEmbedObject("Render")]
    internal RenderData RenderData = RenderData.Default;

    [INIEmbedObject("ParticleSystem")]
    internal ParticleSystemData ParticleSystemData;


    internal void LoadFromINI(string id, string filepath)
    {
      ID = id;
      if (File.Exists(filepath))
      {
        INIFile f = new INIFile(filepath);
        var self = this;
        f.LoadByAttribute(ref self);
      }
    }

    internal void SaveToINI(string id)
    {
      ID = id;
      string filepath = Path.Combine(Globals.ParticleTypeINIDirectory, id + ".ini");

      if (!File.Exists(filepath))
        File.Create(filepath).Close();

      INIFile f = new INIFile(filepath);
      var self = this;
      f.UpdateByAttribute(ref self);
      f.WriteToFile(filepath);
    }

    internal void Init()
    {
      ParticleSystemData.Init(Engine, ref ParticleSystemData);
    }

    /// <summary>
    /// Initializes a projectile instance
    /// </summary>
    /// <param name="engine">The game engine</param>
    /// <param name="ainfo">The instance to initialize</param>
    public virtual void Initialize(Engine engine, ParticleInfo ainfo)
    {
    }

    /// <summary>
    /// Processes a projectile instance 
    /// </summary>
    /// <param name="engine">The game engine</param>
    /// <param name="ainfo">The instance to process</param>
    public virtual void ProcessState(Engine engine, ParticleInfo ainfo)
    {
      ParticleSystemData.Process();

      if (ainfo.IsDying)
      {
        ainfo.SetState_Dead();
      }
    }

    //public void Render()
    //{
    //  ParticleSystemData.Render();
    //}
  }
}
