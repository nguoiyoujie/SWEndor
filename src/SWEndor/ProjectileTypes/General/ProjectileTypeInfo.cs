using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.Core;
using SWEndor.FileFormat.INI;
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
    public string Name;

    // Data
    public ComponentMask Mask { get; set; } = ComponentMask.NONE;

    // Data (structs)
    internal CombatData CombatData;
    internal TimedLifeData TimedLifeData;
    internal MoveLimitData MoveLimitData = MoveLimitData.Default;
    internal RenderData RenderData = RenderData.Default;
    internal MeshData MeshData = MeshData.Default;
    internal DamageSpecialData DamageSpecialData;

    // Data (struct arrays)
    internal ExplodeData[] Explodes = new ExplodeData[0];
    internal SoundSourceData[] InitialSoundSources = new SoundSourceData[0];
    internal SoundSourceData[] SoundSources = new SoundSourceData[0];

    // Derived (derived)
    internal Components.MoveBehavior MoveBehavior;

    internal void LoadFromINI(string id)
    {
      ID = id;
      string filepath = Path.Combine(Globals.ProjectileTypeINIDirectory, id + ".ini");

      if (File.Exists(filepath))
      {
        INIFile f = new INIFile(filepath);
        Name = f.GetString("General", "Name", Name);
        Mask = f.GetEnumValue("General", "Mask", Mask);

        CombatData.LoadFromINI(f, "CombatData");
        TimedLifeData.LoadFromINI(f, "TimedLifeData");
        MoveLimitData.LoadFromINI(f, "MoveLimitData");
        RenderData.LoadFromINI(f, "RenderData");
        MeshData.LoadFromINI(Engine, f, "MeshData");
        DamageSpecialData.LoadFromINI(f, "DamageSpecialData");

        ExplodeData.LoadFromINI(f, "ExplodeData", "Explodes", out Explodes);
        SoundSourceData.LoadFromINI(f, "SoundSourceData", "InitialSoundSources", out InitialSoundSources);
        SoundSourceData.LoadFromINI(f, "SoundSourceData", "SoundSources", out SoundSources);
      }
    }

    internal void SaveToINI(string id)
    {
      ID = id;
      string filepath = Path.Combine(Globals.ProjectileTypeINIDirectory, id + ".ini");

      if (!File.Exists(filepath))
        File.Create(filepath).Close();

      INIFile f = new INIFile(filepath);

      f.SetString("General", "Name", Name);
      f.SetEnum("General", "Mask", Mask);

      CombatData.SaveToINI(f, "CombatData");
      TimedLifeData.SaveToINI(f, "TimedLifeData");
      MoveLimitData.SaveToINI(f, "MoveLimitData");
      RenderData.SaveToINI(f, "RenderData");
      MeshData.SaveToINI(f, "MeshData");
      DamageSpecialData.SaveToINI(f, "DamageSpecialData");

      ExplodeData.SaveToINI(f, "ExplodeData", "Explodes", "EXP", Explodes);
      SoundSourceData.SaveToINI(f, "SoundSourceData", "InitialSoundSources", "ISN", InitialSoundSources);
      SoundSourceData.SaveToINI(f, "SoundSourceData", "SoundSources", "SND", SoundSources);
      f.SaveFile(filepath);
    }

    internal void Init()
    {
      //cachedWeaponData.Load(this);
      MoveBehavior.Load(this);
    }

    /// <summary>
    /// Initializes a projectile instance
    /// </summary>
    /// <param name="engine">The game engine</param>
    /// <param name="ainfo">The instance to initialize</param>
    public virtual void Initialize(Engine engine, ProjectileInfo ainfo)
    {
      // Sound
      foreach (SoundSourceData assi in InitialSoundSources)
        assi.Process(engine, ainfo);
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
        foreach (SoundSourceData assi in SoundSources)
          assi.Process(engine, ainfo);

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
