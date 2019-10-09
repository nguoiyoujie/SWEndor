/*
namespace SWEndor.ProjectileTypes
{
  public partial class ProjectileTypeInfo : ITypeInfo<ActorInfo>
  {
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
    //public RegenData RegenData;
    public TimedLifeData TimedLifeData;
    public ArmorData ArmorData;
    public MoveLimitData MoveLimitData = MoveLimitData.Default;
    public RenderData RenderData = RenderData.Default;
    public AIData AIData = AIData.Default;
    public MeshData MeshData = MeshData.Default;
    //public DyingMoveData DyingMoveData;
    //public ScoreData ScoreData;
    
    // AddOns
    //public AddOnData[] AddOns = new AddOnData[0];

    // Explosionf
    public ExplodeData[] Explodes = new ExplodeData[0];

    // Weapons
    public string[] Loadouts = new string[0];
    public bool TrackerDummyWeapon = false;

    // Sound
    public SoundSourceData[] InitialSoundSources = new SoundSourceData[0];
    public SoundSourceData[] SoundSources = new SoundSourceData[0];

    // derived
    public MoveBehavior MoveBehavior;
    internal UnfixedWeaponData cachedWeaponData;

    public void LoadFromINI(string id)
    {
      ID = id;
      string filepath = Path.Combine(Globals.ActorTypeINIDirectory, id + ".ini");

      if (File.Exists(filepath))
      {
        INIFile f = new INIFile(filepath);
        Name = f.GetStringValue("General", "Name", Name);
        Mask = f.GetEnumValue("General", "Mask", Mask);

        Loadouts = f.GetStringList("General", "Loadouts", Loadouts);
        TrackerDummyWeapon = f.GetBoolValue("General", "TrackerDummyWeapon", TrackerDummyWeapon);

        CombatData.LoadFromINI(f, "CombatData");
        TimedLifeData.LoadFromINI(f, "TimedLifeData");
        ArmorData.LoadFromINI(f, "ArmorData");
        MoveLimitData.LoadFromINI(f, "MoveLimitData");
        RenderData.LoadFromINI(f, "RenderData");
        AIData.LoadFromINI(f, "AIData");
        MeshData.LoadFromINI(f, "MeshData");

        ExplodeData.LoadFromINI(f, "ExplodeData", "Explodes", out Explodes);
        SoundSourceData.LoadFromINI(f, "SoundSourceData", "InitialSoundSources", out InitialSoundSources);
        SoundSourceData.LoadFromINI(f, "SoundSourceData", "SoundSources", out SoundSources);
      }
    }

    public void SaveToINI(string id)
    {
      ID = id;
      string filepath = Path.Combine(Globals.ExplosionTypeINIDirectory, id + ".ini");

      if (!File.Exists(filepath))
        File.Create(filepath).Close(); INIFile f = new INIFile(filepath);

      f.SetStringValue("General", "Name", Name);
      f.SetEnumValue("General", "Mask", Mask);

      f.SetStringList("General", "Loadouts", Loadouts);
      f.SetBoolValue("General", "TrackerDummyWeapon", TrackerDummyWeapon);

      CombatData.SaveToINI(f, "CombatData");
      TimedLifeData.SaveToINI(f, "TimedLifeData");
      ArmorData.SaveToINI(f, "ArmorData");
      MoveLimitData.SaveToINI(f, "MoveLimitData");
      RenderData.SaveToINI(f, "RenderData");
      AIData.SaveToINI(f, "AIData");
      MeshData.SaveToINI(f, "MeshData");

      ExplodeData.SaveToINI(f, "ExplodeData", "Explodes", "EXP", Explodes);
      SoundSourceData.SaveToINI(f, "SoundSourceData", "InitialSoundSources", "ISN", InitialSoundSources);
      SoundSourceData.SaveToINI(f, "SoundSourceData", "SoundSources", "SND", SoundSources);
      f.SaveFile(filepath);
    }

    public void Init()
    {
      //cachedWeaponData.Load(this);
      //MoveBehavior.Load(this);
    }

    public virtual void Initialize(Engine engine, ActorInfo ainfo)
    {
      // AI
      ainfo.CanEvade = AIData.CanEvade;
      ainfo.CanRetaliate = AIData.CanRetaliate;

      // Sound
      foreach (SoundSourceData assi in InitialSoundSources)
        assi.Process(engine, ainfo);
    }

    public virtual void ProcessState(Engine engine, ActorInfo ainfo)
    {
      // weapons
      foreach (WeaponInfo w in ainfo.WeaponDefinitions.Weapons)
        w.Reload(engine);

      // regeneration
      ainfo.Regenerate(engine.Game.TimeSinceRender);

      ainfo.TickExplosions();

      if (ainfo.IsDying)
        ainfo.SetState_Dead();

      // sound
      if (PlayerInfo.Actor != null
        && ainfo.Active 
        && !ainfo.IsScenePlayer)
      {
        foreach (SoundSourceData assi in SoundSources)
          assi.Process(engine, ainfo);
      }
    }

    public virtual void ProcessHit(Engine engine, ActorInfo owner, ActorInfo hitby, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      if (owner == null || hitby == null)
        return;

      if (hitby.TypeInfo.CombatData.ImpactDamage == 0)
        return;

      if (owner.IsDying
        && owner.TypeInfo.CombatData.HitWhileDyingLeadsToDeath)
        owner.SetState_Dead();

      if (!owner.IsDyingOrDead)
      {
        float p_hp = owner.HP;
        owner.InflictDamage(hitby, hitby.TypeInfo.CombatData.ImpactDamage, DamageType.NORMAL, impact);
        float hp = owner.HP;

        if (owner.IsPlayer)
          if (hp < (int)p_hp)
            PlayerInfo.FlashHit(PlayerInfo.StrengthColor);

        // scoring
        ActorInfo attacker = hitby.TopParent;
        if (attacker.IsScenePlayer)
        {
          if (!attacker.Faction.IsAlliedWith(owner.Faction))
            AddScore(engine, PlayerInfo.Score, hitby, owner);
          else
            Engine.Screen2D.MessageText(string.Format("{0}: {1}, watch your fire!", owner.Name, PlayerInfo.Name)
                                            , 5
                                            , owner.Faction.Color
                                            , -1);
        }


        if (owner.IsScenePlayer)
        {
          PlayerInfo.Score.AddDamage(engine, attacker, hitby.TypeInfo.CombatData.ImpactDamage * owner.GetArmor(DamageType.NORMAL));

          if (owner.IsDyingOrDead)
            PlayerInfo.Score.AddDeath(engine, attacker);
        }

        if (attacker != null && !attacker.Faction.IsAlliedWith(owner.Faction))
        {
          // Fighter AI
          if ((owner.TypeInfo.AIData.TargetType.Has(TargetType.FIGHTER)))
          {
            if (owner.CanRetaliate && (owner.CurrentAction == null || owner.CurrentAction.CanInterrupt))
            {
              if (!owner.Squad.IsNull && owner.Squad.Mission == null)
              {
                if (!attacker.Squad.IsNull)
                {
                  foreach (ActorInfo a in owner.Squad.Members)
                  {
                    if (a.CanRetaliate && (a.CurrentAction == null || a.CurrentAction.CanInterrupt))
                    {
                      ActorInfo b = attacker.Squad.MembersCopy.Random(engine);
                      if (b != null)
                      {
                        a.ClearQueue();
                        a.QueueLast(AttackActor.GetOrCreate(b.ID));
                      }
                    }
                  }
                }
                else
                {
                  foreach (ActorInfo a in owner.Squad.Members)
                  {
                    if (a.CanRetaliate && (a.CurrentAction == null || a.CurrentAction.CanInterrupt))
                    {
                      a.ClearQueue();
                      a.QueueLast(AttackActor.GetOrCreate(attacker.ID));
                    }
                  }
                }
              }
              else
              {
                owner.ClearQueue();
                owner.QueueLast(AttackActor.GetOrCreate(attacker.ID));
              }
            }
            else if (owner.CanEvade && !(owner.CurrentAction is Evade))
            {
              owner.QueueFirst(new Evade());
            }

            if (!owner.Squad.IsNull)
            {
              if (owner.Squad.Leader == owner)
                owner.Squad.AddThreat(attacker, true);
              else
                owner.Squad.AddThreat(attacker);
            }
          }
        }
      }

      hitby.Position = new TV_3DVECTOR(impact.x, impact.y, impact.z);
      hitby.SetState_Dead(); // projectiles die on impact
    }

    private void AddScore(Engine engine, ScoreInfo score, ActorInfo proj, ActorInfo victim)
    {
      if (!victim.IsDyingOrDead)
      {
        score.AddHit(engine, victim, proj.TypeInfo.CombatData.ImpactDamage * victim.GetArmor(DamageType.NORMAL));
      }

      if (victim.IsDyingOrDead)
      {
        score.AddKill(engine, victim);
      }
    }

    public virtual bool FireWeapon(Engine engine, ActorInfo owner, ActorInfo target, WeaponShotInfo sweapon)
    {
      if (owner == null)
        return false;

      // AI Determination
      if (EqualityComparer<WeaponShotInfo>.Default.Equals(sweapon, WeaponShotInfo.Automatic))
      {
        foreach (WeaponShotInfo ws in owner.WeaponDefinitions.AIWeapons)
          if (FireWeapon(engine, owner, target, ws))
            return true;
      }
      else
      {
        return sweapon.Fire(engine, owner, target);
      }
      
      return false;
    }

    public virtual void Dying(Engine engine, ActorInfo ainfo)
    {
      if (ainfo == null)
        throw new ArgumentNullException("ainfo");

      ainfo.DyingTimerStart();
    }

    public virtual void Dead(Engine engine, ActorInfo ainfo)
    {
      if (ainfo == null)
        throw new ArgumentNullException("ainfo");

      // Explode
      ainfo.TickExplosions();
    }
  }
}
*/