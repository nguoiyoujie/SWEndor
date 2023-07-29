using MTV3D65;
using SWEndor.Game.Actors;
using SWEndor.Game.ActorTypes.Components;
using SWEndor.Game.Core;
using Primrose.FileFormat.INI;
using SWEndor.Game.Models;
using SWEndor.Game.Projectiles;
using System;
using System.IO;

namespace SWEndor.Game.ProjectileTypes
{
  /// <summary>
  /// Defines a type holding data to create projectile instances.
  /// </summary>
  public partial class ProjectileTypeInfo : ITypeInfo<ProjectileInfo>
  {
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
    [INIEmbedObject("Combat")]
    internal CombatData CombatData;

    [INIEmbedObject("TimedLife")]
    internal TimedLifeData TimedLifeData;

    [INIEmbedObject("MoveLimit")]
    internal MoveLimitData MoveLimitData = MoveLimitData.Default;

    [INIEmbedObject("Render")]
    internal RenderData RenderData = RenderData.Default;

    [INIEmbedObject("Mesh")]
    internal MeshData MeshData = MeshData.Default;

    [INIEmbedObject("Explode")]
    internal ExplodeSystemData ExplodeSystemData = ExplodeSystemData.Default;

    [INIEmbedObject("DamageSpecial")]
    internal DamageSpecialData DamageSpecialData = DamageSpecialData.Default;

    [INIEmbedObject("Sound")]
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

        MeshData.Load(Engine, ID);
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
      f.WriteToFile(filepath);
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
      {
        if (!NearEnoughImpact(engine, ainfo, ainfo.Target))
        {
          ulong oid = engine.GameScenarioManager.Scenario.State.Octree.GetId(ainfo.GetGlobalPosition(), CombatData.ImpactCloseEnoughDistance);
          //if (oid != null)
          {
            foreach (ActorInfo a in engine.GameScenarioManager.Scenario.State.Octree.Search(oid))
            {
              if (a.Active && ainfo.CanCollideWith(a))
              {
                if (NearEnoughImpact(engine, ainfo, a)) { break; }
              }
            }
          }
        }
      }
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

    private void NearEnoughImpact(Engine engine, ProjectileInfo proj)
    {
      foreach (ActorInfo a in engine.ActorFactory.Actors)
      {
        if (a.Active && proj.CanCollideWith(a))
        {
          if (NearEnoughImpact(engine, proj, a)) { break; }
        }
      }
    }

    private bool NearEnoughImpact(Engine engine, ProjectileInfo proj, ActorInfo target)
    {
      // projectile
      float impdist = CombatData.ImpactCloseEnoughDistance;
      if (target != null)
      {
        //if (target.TargetType.Intersects(TargetType.MUNITION))
        //  impdist += target.TypeInfo.CombatData.ImpactCloseEnoughDistance;

        // Anticipate
        float dist = DistanceModel.GetDistance(engine, proj, target);

        if (dist < impdist)
        {
          target.TypeInfo.ProcessHit(engine, target, proj, target.GetGlobalPosition());
          proj.TypeInfo.ProcessHit(engine, proj, target, target.GetGlobalPosition());
          return true;
        }
      }
      return false;
    }
  }
}
