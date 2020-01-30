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
    private const string sCombat = "Combat";
    private const string sTimedLife = "TimedLife";
    private const string sMoveLimit = "MoveLimit";
    private const string sRender = "Render";
    private const string sMesh = "Mesh";
    private const string sExplode = "Explode";
    private const string sDamageSpecial = "DamageSpecial";
    private const string sSound = "Sound";

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
    internal ExplodeSystemData ExplodeSystemData = ExplodeSystemData.Default;
    internal DamageSpecialData DamageSpecialData;
    internal SoundData SoundData = SoundData.Default;

    // Derived (derived)
    internal Components.MoveBehavior MoveBehavior;

    internal void LoadFromINI(string id, string filepath)
    {
      ID = id;
      if (File.Exists(filepath))
      {
        INIFile f = new INIFile(filepath);
        Name = f.GetString("General", "Name", Name);
        Mask = f.GetEnum("General", "Mask", Mask);

        CombatData.LoadFromINI(f, sCombat);
        TimedLifeData.LoadFromINI(f, sTimedLife);
        MoveLimitData.LoadFromINI(f, sMoveLimit);
        RenderData.LoadFromINI(f, sRender);
        MeshData.LoadFromINI(Engine, f, sMesh, ID);
        ExplodeSystemData.LoadFromINI(f, sExplode);
        DamageSpecialData.LoadFromINI(f, sDamageSpecial);
        SoundData.LoadFromINI(f, sSound);
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

      CombatData.SaveToINI(f, sCombat);
      TimedLifeData.SaveToINI(f, sTimedLife);
      MoveLimitData.SaveToINI(f, sMoveLimit);
      RenderData.SaveToINI(f, sRender);
      MeshData.SaveToINI(f, sMesh);
      ExplodeSystemData.SaveToINI(f, sExplode);
      DamageSpecialData.SaveToINI(f, sDamageSpecial);
      SoundData.SaveToINI(f, sSound);

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
