using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.Core;
using SWEndor.FileFormat.INI;
using SWEndor.Models;
using SWEndor.Player;
using SWEndor.Projectiles;
using System;
using System.IO;

namespace SWEndor.ProjectileTypes
{
  public partial class ProjectileTypeInfo : ITypeInfo<ProjectileInfo>
  {
    public static ProjectileTypeInfo Null = new ProjectileTypeInfo(Globals.Engine.ProjectileTypeFactory, "$NULL", "Null");

    public ProjectileTypeInfo(Factory owner, string id, string name)
    {
      ActorTypeFactory = owner;
      if (id?.Length > 0) { ID = id; }
      if (name?.Length > 0) { Name = name; }

      CombatData.Reset();
    }

    public readonly Factory ActorTypeFactory;
    public Engine Engine { get { return ActorTypeFactory.Engine; } }

    public PlayerInfo PlayerInfo { get { return Engine.PlayerInfo; } }

    // Basic Info
    public string ID;
    public string Name;

    // Data
    public ComponentMask Mask = ComponentMask.NONE;

    // Data (structs)
    public CombatData CombatData;
    public TimedLifeData TimedLifeData;
    public MoveLimitData MoveLimitData = MoveLimitData.Default;
    public RenderData RenderData = RenderData.Default;
    public MeshData MeshData = MeshData.Default;
    
    // AddOns

    // Explosionf
    public ExplodeData[] Explodes = new ExplodeData[0];

    // Sound
    public SoundSourceData[] InitialSoundSources = new SoundSourceData[0];
    public SoundSourceData[] SoundSources = new SoundSourceData[0];

    // derived
    public ProjectileTypes.Components.MoveBehavior MoveBehavior;

    public void LoadFromINI(string id)
    {
      ID = id;
      string filepath = Path.Combine(Globals.ProjectileTypeINIDirectory, id + ".ini");

      if (File.Exists(filepath))
      {
        INIFile f = new INIFile(filepath);
        Name = f.GetStringValue("General", "Name", Name);
        Mask = f.GetEnumValue("General", "Mask", Mask);

        CombatData.LoadFromINI(f, "CombatData");
        TimedLifeData.LoadFromINI(f, "TimedLifeData");
        MoveLimitData.LoadFromINI(f, "MoveLimitData");
        RenderData.LoadFromINI(f, "RenderData");
        MeshData.LoadFromINI(f, "MeshData");

        ExplodeData.LoadFromINI(f, "ExplodeData", "Explodes", out Explodes);
        SoundSourceData.LoadFromINI(f, "SoundSourceData", "InitialSoundSources", out InitialSoundSources);
        SoundSourceData.LoadFromINI(f, "SoundSourceData", "SoundSources", out SoundSources);
      }
    }

    public void SaveToINI(string id)
    {
      ID = id;
      string filepath = Path.Combine(Globals.ProjectileTypeINIDirectory, id + ".ini");

      if (!File.Exists(filepath))
        File.Create(filepath).Close(); INIFile f = new INIFile(filepath);

      f.SetStringValue("General", "Name", Name);
      f.SetEnumValue("General", "Mask", Mask);

      CombatData.SaveToINI(f, "CombatData");
      TimedLifeData.SaveToINI(f, "TimedLifeData");
      MoveLimitData.SaveToINI(f, "MoveLimitData");
      RenderData.SaveToINI(f, "RenderData");
      MeshData.SaveToINI(f, "MeshData");

      ExplodeData.SaveToINI(f, "ExplodeData", "Explodes", "EXP", Explodes);
      SoundSourceData.SaveToINI(f, "SoundSourceData", "InitialSoundSources", "ISN", InitialSoundSources);
      SoundSourceData.SaveToINI(f, "SoundSourceData", "SoundSources", "SND", SoundSources);
      f.SaveFile(filepath);
    }

    public void Init()
    {
      //cachedWeaponData.Load(this);
      MoveBehavior.Load(this);
    }

    public virtual void Initialize(Engine engine, ProjectileInfo ainfo)
    {
      // Sound
      foreach (SoundSourceData assi in InitialSoundSources)
        assi.Process(engine, ainfo);
    }

    public virtual void ProcessState(Engine engine, ProjectileInfo ainfo)
    {
      ainfo.TickExplosions();

      if (ainfo.IsDying)
        ainfo.SetState_Dead();

      // sound
      if (PlayerInfo.Actor != null
        && ainfo.Active)
      {
        foreach (SoundSourceData assi in SoundSources)
          assi.Process(engine, ainfo);
      }

      // projectile
      if (!ainfo.IsDyingOrDead)
        NearEnoughImpact(engine, ainfo, ainfo.Target);
    }

    public void NearEnoughImpact(Engine engine, ProjectileInfo proj, ActorInfo target)
    {
      // projectile
        float impdist = CombatData.ImpactCloseEnoughDistance;
      if (target != null)
      {
        if (target.TypeInfo.AIData.TargetType.Contains(TargetType.MUNITION))
          impdist += target.TypeInfo.CombatData.ImpactCloseEnoughDistance;

        // Anticipate
        float dist = DistanceModel.GetDistance(engine, proj, target);

        if (dist < impdist)
        {
          target.TypeInfo.ProcessHit(engine, target, proj, target.GetGlobalPosition());
          proj.TypeInfo.ProcessHit(engine, proj, target, target.GetGlobalPosition());

          ActorInfo o = proj.Owner;
          if (o != null)
          {
            o.OnHitEvent(target);
            target.OnHitEvent(o);
          }
        }
      }
    }

    public virtual void ProcessHit(Engine engine, ProjectileInfo owner, ActorInfo hitby, TV_3DVECTOR impact) { }

    private void AddScore(Engine engine, ScoreInfo score, ProjectileInfo proj, ActorInfo victim)
    {
      if (!victim.IsDyingOrDead)
      {
        score.AddHit(engine, victim, proj.TypeInfo.CombatData.ImpactDamage * victim.GetArmor(proj.TypeInfo.CombatData.DamageType));
      }

      if (victim.IsDyingOrDead)
      {
        score.AddKill(engine, victim);
      }
    }

    public virtual void Dying(Engine engine, ProjectileInfo ainfo)
    {
      if (ainfo == null)
        throw new ArgumentNullException("ainfo");

      ainfo.DyingTimerStart();
    }

    public virtual void Dead(Engine engine, ProjectileInfo ainfo)
    {
      if (ainfo == null)
        throw new ArgumentNullException("ainfo");

      // Explode
      ainfo.TickExplosions();
    }
  }
}
