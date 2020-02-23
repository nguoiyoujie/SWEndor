using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.Core;
using Primitives.FileFormat.INI;
using SWEndor.Models;
using SWEndor.Projectiles;
using SWEndor.Projectiles.Components;
using System;
using System.IO;

namespace SWEndor.ProjectileTypes
{
  /// <summary>
  /// Defines a type holding data to create projectile instances.
  /// </summary>
  public partial class ProjectileTypeInfo : ITypeInfo<ProjectileInfo>
  {
    private const string sMesh = "Mesh";

    internal static readonly ProjectileTypeInfo Null = new ProjectileTypeInfo(Globals.Engine.ProjectileTypeFactory, "$NULL", "Null");

    internal ProjectileTypeInfo(Factory owner, string id, string name)
    {
      ActorTypeFactory = owner;
      if (id?.Length > 0) { ID = id; }
      if (name?.Length > 0) { Name = name; }

      CombatData.Reset();
    }

    internal readonly Factory ActorTypeFactory;

    /// <summary>The Engine that owns this object</summary>
    public Engine Engine { get { return ActorTypeFactory.Engine; } }

    /// <summary>The ID of this object</summary>
    public string ID;

    /// <summary>The given name of this object</summary>
    [INIValue("General", "Name")]
    public string Name;

    // Data
    [INIValue("General", "Mask")]
    public ComponentMask Mask { get; set; } = ComponentMask.NONE;

    // Data (structs)
    [INIEmbedObject]
    internal CombatData CombatData;

    [INIEmbedObject]
    internal TimedLifeData TimedLifeData;

    [INIEmbedObject]
    internal MoveLimitData MoveLimitData = MoveLimitData.Default;

    [INIEmbedObject]
    internal RenderData RenderData = RenderData.Default;

    //[INIEmbedObject]
    internal MeshData MeshData = MeshData.Default;

    [INIEmbedObject]
    internal ExplodeSystemData ExplodeSystemData = ExplodeSystemData.Default;

    [INIEmbedObject]
    internal DamageSpecialData DamageSpecialData;

    [INIEmbedObject]
    internal SoundData SoundData = SoundData.Default;

    // Derived (derived)
    internal Components.MoveBehavior MoveBehavior;

    internal void LoadFromINI(string id, string filepath)
    {
      ID = id;
      if (File.Exists(filepath))
      {
        INIFile f = new INIFile(filepath);
        var self = this;
        f.LoadByAttribute(ref self);

        MeshData.LoadFromINI(Engine, f, sMesh, ID);
      }
    }

    internal void SaveToINI(string id)
    {
      ID = id;
      string filepath = Path.Combine(Globals.ProjectileTypeINIDirectory, id + ".ini");

      if (!File.Exists(filepath))
        File.Create(filepath).Close();

      INIFile f = new INIFile(filepath);
      var self = this;
      f.UpdateByAttribute(ref self);

      MeshData.SaveToINI(f, sMesh);

      f.SaveFile(filepath);
    }

    internal void Init()
    {
      MoveBehavior.Load(this);
    }

    /// <summary>
    /// Initializes a projectile instance
    /// </summary>
    /// <param name="engine">The game engine</param>
    /// <param name="ainfo">The instance to initialize</param>
    public virtual void Initialize(Engine engine, ProjectileInfo ainfo)
    {
      SoundData.ProcessInitial(engine, ainfo);

      if (RenderData.RemapLaserColor)
        if (ainfo.Owner != null && ainfo.Owner.Faction.LaserColor.Value != 0)
          ainfo.SetColor(ainfo.Owner.Faction.LaserColor.Value);
    }

    /// <summary>
    /// Processes a projectile instance 
    /// </summary>
    /// <param name="engine">The game engine</param>
    /// <param name="ainfo">The instance to process</param>
    public virtual void ProcessState(Engine engine, ProjectileInfo ainfo)
    {
      ainfo.TickExplosions();

      if (ainfo.IsDying)
        ainfo.SetState_Dead();

      // sound
      if (engine.PlayerInfo.Actor != null && ainfo.Active)
        SoundData.Process(engine, ainfo);

      // projectile
      if (!ainfo.IsDyingOrDead)
        NearEnoughImpact(engine, ainfo, ainfo.Target);
    }

    /// <summary>
    /// Processes a projectile instance when a hit is registered
    /// </summary>
    /// <param name="engine">The game engine</param>
    /// <param name="owner">The instance to process</param>
    /// <param name="hitby">The actor instance that hit it</param>
    /// <param name="impact">The impact location</param>
    public virtual void ProcessHit(Engine engine, ProjectileInfo owner, ActorInfo hitby, TV_3DVECTOR impact)
    {
      DamageSpecialData.ProcessHit(engine, hitby);
    }


    /// <summary>
    /// Processes a projectile instance when it enters the Dying state
    /// </summary>
    /// <param name="engine">The game engine</param>
    /// <param name="ainfo">The instance to process</param>
    public virtual void Dying(Engine engine, ProjectileInfo ainfo)
    {
#if DEBUG
      if (engine == null) throw new ArgumentNullException("engine");
      if (ainfo == null) throw new ArgumentNullException("ainfo");
#endif

      ainfo.DyingTimerStart();
    }

    /// <summary>
    /// Processes a projectile instance when it enters the Dead state
    /// </summary>
    /// <param name="engine">The game engine</param>
    /// <param name="ainfo">The instance to process</param>
    public virtual void Dead(Engine engine, ProjectileInfo ainfo)
    {
#if DEBUG
      if (engine == null) throw new ArgumentNullException("engine");
      if (ainfo == null) throw new ArgumentNullException("ainfo");
#endif

      // Explode
      ainfo.TickExplosions();
    }

    private void NearEnoughImpact(Engine engine, ProjectileInfo proj, ActorInfo target)
    {
      // projectile
      float impdist = CombatData.ImpactCloseEnoughDistance;
      if (target != null)
      {
        if (target.TypeInfo.AIData.TargetType.Intersects(TargetType.MUNITION))
          impdist += target.TypeInfo.CombatData.ImpactCloseEnoughDistance;

        // Anticipate
        float dist = DistanceModel.GetDistance(engine, proj, target);

        if (dist < impdist)
        {
          target.TypeInfo.ProcessHit(engine, target, proj, target.GetGlobalPosition());
          proj.TypeInfo.ProcessHit(engine, proj, target, target.GetGlobalPosition());

          target.OnHitEvent();
        }
      }
    }

    private void AddScore(Engine engine, ScoreInfo score, ProjectileInfo proj, ActorInfo victim)
    {
      if (!victim.IsDyingOrDead)
        score.AddHit(engine, victim, proj.TypeInfo.CombatData.ImpactDamage * victim.GetArmor(proj.TypeInfo.CombatData.DamageType));

      if (victim.IsDyingOrDead)
        score.AddKill(engine, victim);
    }

  }
}
