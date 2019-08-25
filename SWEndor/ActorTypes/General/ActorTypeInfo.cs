using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.Player;
using SWEndor.Primitives;
using SWEndor.Scenarios;
using SWEndor.Weapons;
using System;
using System.Collections.Generic;

namespace SWEndor.ActorTypes
{
  public partial class ActorTypeInfo : IDisposable
  {
    public ActorTypeInfo(Factory owner, string name = "")
    {
      ActorTypeFactory = owner;
      if (name.Length > 0) { Name = name; }

      RegenData.Reset();
      ExplodeData.Reset();
      CombatData.Reset();
      TimedLifeData.Reset();
    }

    public readonly Factory ActorTypeFactory;
    public Engine Engine { get { return ActorTypeFactory.Engine; } }

    public Game Game { get { return Engine.Game; } }
    public GameScenarioManager GameScenarioManager { get { return Engine.GameScenarioManager; } }
    public TrueVision TrueVision { get { return Engine.TrueVision; } }
    public ActorDataSet ActorDataSet { get { return Engine.ActorDataSet; } }
    public ActorInfo.Factory ActorFactory { get { return Engine.ActorFactory; } }
    public LandInfo LandInfo { get { return Engine.LandInfo; } }
    public AtmosphereInfo AtmosphereInfo { get { return Engine.AtmosphereInfo; } }
    public PlayerInfo PlayerInfo { get { return Engine.PlayerInfo; } }
    public PlayerCameraInfo PlayerCameraInfo { get { return Engine.PlayerCameraInfo; } }
    public Screen2D Screen2D { get { return Engine.Screen2D; } }

    // Basic Info
    public string Name;

    // Data
    public ComponentMask Mask = ComponentMask.NONE;
    public RegenData RegenData = new RegenData();
    public ExplodeData ExplodeData = new ExplodeData();
    public CombatData CombatData = new CombatData();
    public TimedLifeData TimedLifeData = new TimedLifeData();
    public ArmorInfo Armor = new ArmorInfo();
    public DamageType DamageType = DamageType.COLLISION;

    // Mesh Data
    public float Scale = 1;

    //Sys Data
    public float MaxStrength = 1.0f;

    //Combat Data
    public float ImpactDamage = 1.0f;

    //Move Data
    public float MaxSpeed = 0.0f;
    public float MinSpeed = 0.0f;
    public float MaxSpeedChangeRate = 0.0f;
    public float MaxTurnRate = 0.0f;
    public float MaxSecondOrderTurnRateFrac = 0.2f;
    public float XLimit = 75.0f;
    public float ZTilt = 0.0f;
    public float ZNormFrac = 0.025f;


    public bool EnableDistanceCull = true;
    public float CullDistance = 20000.0f;

    // AI
    public float Attack_AngularDelta = 5f;
    public float Attack_HighAccuracyAngularDelta = 1f;
    public float Move_CloseEnough = 500;

    public bool AggressiveTracker = false;
    public bool AlwaysAccurateRotation = false;

    public int Score_perStrength = 0;
    public int Score_DestroyBonus = 0;

    // AI
    public bool CanEvade = false;
    public bool CanRetaliate = false;
    public bool CanCheckCollisionAhead = false;

    // Targeting
    public TargetType TargetType = TargetType.NULL;
    public int HuntWeight = 1;

    // Meshs
    public string SourceMeshPath = "";
    public string SourceFarMeshPath = "";
    public TVMesh SourceMesh = null;
    public TVMesh SourceFarMesh = null;

    public TV_3DVECTOR max_dimensions = new TV_3DVECTOR();
    public TV_3DVECTOR min_dimensions = new TV_3DVECTOR();
    public TV_3DVECTOR size { get { return max_dimensions - min_dimensions; } }


    // Render
    public float RadarSize = 0;
    public RadarType RadarType = RadarType.NULL;
    public bool AlwaysShowInRadar = false;

    // Performance Savings
    public bool IsLaser = false;

    // Misc
    public bool IsExplosion = false;

    // AddOns
    public AddOnInfo[] AddOns = new AddOnInfo[0];

    // Debris
    public DebrisSpawnerInfo[] Debris = new DebrisSpawnerInfo[0];

    public ActorCameraInfo[] Cameras = new ActorCameraInfo[0];
    public DeathCameraInfo DeathCamera = new DeathCameraInfo(350, 25, 15);

    // Sound
    public SoundSourceInfo[] InitialSoundSources = new SoundSourceInfo[0];
    public SoundSourceInfo[] SoundSources = new SoundSourceInfo[0];


    public virtual void RegisterModel()
    {
      if (SourceMesh == null)
      {
        SourceMesh = TrueVision.TVGlobals.GetMesh(Name);
        if (SourceMesh == null)
        {
          SourceMesh = TrueVision.TVScene.CreateMeshBuilder(Name);
          //SourceMesh.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_BUMPMAPPING_TANGENTSPACE, 8);

          if (SourceMeshPath.Length > 0)
            SourceMesh.LoadXFile(SourceMeshPath, true);
          SourceMesh.Enable(false);
          SourceMesh.WeldVertices();
          SourceMesh.ComputeBoundings();
        }
      }

      if (SourceFarMesh == null)
      {
        SourceFarMesh = TrueVision.TVGlobals.GetMesh(Name + "_far");
        if (SourceFarMesh == null)
        {
          if (SourceFarMeshPath.Length > 0)
          {
            SourceFarMesh = TrueVision.TVScene.CreateMeshBuilder(Name + "_far");
            SourceFarMesh.LoadXFile(SourceFarMeshPath, true);
            SourceFarMesh.Enable(false);
            SourceFarMesh.WeldVertices();
            SourceFarMesh.ComputeBoundings();
          }
          else
            SourceFarMesh = SourceMesh.Duplicate();
        }
      }
    }

    public virtual TVMesh GenerateMesh()
    {
      if (SourceMesh == null)
        throw new NotImplementedException("Attempted to generate empty model");

      SourceMesh.Enable(false);
      SourceMesh.SetCollisionEnable(false);

      TVMesh ret = SourceMesh.Duplicate();
      SourceMesh.GetBoundingBox(ref min_dimensions, ref max_dimensions);
      return ret;
    }

    public virtual TVMesh GenerateFarMesh()
    {
      if (SourceFarMesh == null)
        SourceFarMesh = SourceMesh.Duplicate();

      if (SourceFarMesh == null)
        throw new NotImplementedException("Attempted to generate empty model");

      SourceFarMesh.Enable(false);
      SourceFarMesh.SetCollisionEnable(false);

      TVMesh ret = SourceFarMesh.Duplicate();
      return ret;
    }

    public int LoadAlphaTexture(string name, string texpath, string alphatexpath = null)
    {
      int tex = TrueVision.TVGlobals.GetTex(name);
      if (tex == 0)
      {
        int texS = TrueVision.TVTextureFactory.LoadTexture(texpath);
        int texA = TrueVision.TVTextureFactory.LoadTexture(alphatexpath ?? texpath); //LoadAlphaTexture
        tex = TrueVision.TVTextureFactory.AddAlphaChannel(texS, texA, name);
      }
      return tex;
    }

    public int LoadTexture(string name, string texpath)
    {
      int tex = TrueVision.TVGlobals.GetTex(name);
      if (tex == 0)
        tex = TrueVision.TVTextureFactory.LoadTexture(texpath, name);
      return tex;
    }

    public virtual void Initialize(ActorInfo ainfo)
    {
      // AI
      ainfo.CanEvade = CanEvade;
      ainfo.CanRetaliate = CanRetaliate;

      if (!(GameScenarioManager.Scenario is GSMainMenu))
        foreach (SoundSourceInfo assi in InitialSoundSources)
          assi.Process(ainfo);
    }

    public void GenerateAddOns(ActorInfo ainfo)
    {
      foreach (AddOnInfo addon in AddOns)
        addon.Create(Engine, ainfo);
    }

    public virtual void ProcessState(ActorInfo ainfo)
    {
      // weapons
      foreach (WeaponInfo w in ainfo.WeaponSystemInfo.Weapons.Values)
        w.Reload(Engine);

      // regeneration
      RegenerationSystem.Process(Engine, ainfo, Game.TimeSinceRender);

      if (ainfo.IsDying)
      {
        ExplosionSystem.ProcessDying(Engine, ainfo);
        ainfo.DyingMoveComponent?.Update(ainfo, ref ainfo.MoveData, Game.TimeSinceRender);
      }

      // sound
      if (PlayerInfo.Actor != null
        && ainfo.Active 
        && !ainfo.IsScenePlayer
        && !(GameScenarioManager.Scenario is GSMainMenu))
      {
        foreach (SoundSourceInfo assi in SoundSources)
          assi.Process(ainfo);
      }
    }

    public virtual void ProcessHit(ActorInfo owner, ActorInfo hitby, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      if (owner == null || hitby == null)
        return;

      if (owner.IsDying 
        && ActorDataSet.CombatData[owner.dataID].HitWhileDyingLeadsToDeath)
        owner.SetState_Dead();

      if (owner.IsDyingOrDead
        || hitby.TypeInfo.ImpactDamage == 0)
        return;

      if (Engine.MaskDataSet[hitby].Has(ComponentMask.IS_DAMAGE))
      {
        float str0 = owner.HP;
        owner.InflictDamage(hitby, hitby.TypeInfo.ImpactDamage, DamageType.NORMAL, impact);
        float str1 = owner.HP;

        if (owner.IsPlayer)
        {
          if (str1 < (int)str0)
            PlayerInfo.FlashHit(PlayerInfo.StrengthColor);
        }

        // scoring
        ActorInfo attacker = hitby.TopParent;
        if (attacker.IsScenePlayer)
        {
          if (!attacker.Faction.IsAlliedWith(owner.Faction))
            AddScore(PlayerInfo.Score, hitby, owner);
          else
            Screen2D.MessageText(string.Format("{0}: {1}, watch your fire!", owner.Name, PlayerInfo.Name)
                                            , 5
                                            , owner.Faction.Color
                                            , -1);
        }

        if (owner.IsScenePlayer)
        {
          PlayerInfo.Score.AddDamage(Engine, attacker, hitby.TypeInfo.ImpactDamage * owner.GetArmor(DamageType.NORMAL));

          if (owner.IsDyingOrDead)
            PlayerInfo.Score.AddDeath(Engine, attacker);
        }

        if (attacker != null && !attacker.Faction.IsAlliedWith(owner.Faction))
        {
          // Fighter AI
          if ((owner.TypeInfo is Groups.Fighter))
          {
            if (owner.CanRetaliate && (owner.CurrentAction == null || owner.CurrentAction.CanInterrupt))
            {
              if (owner.Squad != null && owner.Squad.Missions.Count == 0)
              {
                if (attacker.Squad != null)
                {
                  foreach (ActorInfo a in owner.Squad.Members)
                  {
                    ActorInfo b = new List<ActorInfo>(attacker.Squad.Members).Random(Engine);
                    a.ClearQueue();
                    a.QueueLast(new AttackActor(b.ID));
                  }
                }
                else
                {
                  foreach (ActorInfo a in owner.Squad.Members)
                  {
                    a.ClearQueue();
                    a.QueueLast(new AttackActor(attacker.ID));
                  }
                }
              }
              else
              {
                owner.ClearQueue();
                owner.QueueLast(new AttackActor(attacker.ID));
              }
            }
            else if (owner.CanEvade && !(owner.CurrentAction is Evade))
            {
              owner.QueueFirst(new Evade());
            }

            if (owner.Squad != null)
            {
              if (owner.Squad.Leader == owner)
                owner.Squad.AddThreat(attacker, true);
              else
                owner.Squad.AddThreat(attacker);
            }
          }
        }

        hitby.Position = new TV_3DVECTOR(impact.x, impact.y, impact.z);
        hitby.SetState_Dead(); // projectiles die on impact
      }
      else if (Engine.MaskDataSet[owner].Has(ComponentMask.IS_DAMAGE))
      {
      }
      else
      {
        // Collision
        owner.InflictDamage(hitby, hitby.TypeInfo.ImpactDamage, DamageType.COLLISION, impact);
        if (owner.HP > 0
          && Engine.MaskDataSet[owner].Has(ComponentMask.CAN_MOVE)
          && owner.TypeInfo.TargetType.HasFlag(TargetType.FIGHTER))
        {
          float repel = -owner.MoveData.Speed * 0.25f;
          owner.MoveRelative(repel, 0, 0);
        }

        ActorInfo attacker = hitby.TopParent;
        if (attacker.IsScenePlayer)
        {
          if (!attacker.Faction.IsAlliedWith(owner.Faction))
            AddScore(PlayerInfo.Score, attacker, owner);
          else
            Screen2D.MessageText(string.Format("{0}: {1}, watch it!", owner.Name, PlayerInfo.Name)
                                            , 5
                                            , owner.Faction.Color
                                            , -1);
        }

        // Fighter Collision
        if ((owner.TypeInfo is Groups.Fighter && owner.IsDyingOrDead))
        {
          owner.SetState_Dead();
          if (owner.IsScenePlayer)
            PlayerInfo.Score.AddDeath(Engine, attacker);
        }
      }

      hitby.OnHitEvent(owner.ID);
    }

    private void AddScore(ScoreInfo score, ActorInfo proj, ActorInfo victim)
    {
      if (!victim.IsDyingOrDead)
      {
        score.AddHit(Engine, victim, proj.TypeInfo.ImpactDamage * victim.GetArmor(DamageType.NORMAL));
      }

      if (victim.IsDyingOrDead)
      {
        score.AddKill(Engine, victim);
      }
    }

    public void InterpretWeapon(ActorInfo owner, string sweapon, out WeaponInfo weapon, out int burst)
    {
      string s = "none";
      weapon = null;
      burst = 1;

      if (owner == null)
        return;

      int seperatorpos = sweapon.IndexOf(':');
      if (seperatorpos > -1)
      {
        s = sweapon.Substring(seperatorpos + 1).Trim();
        int.TryParse(sweapon.Substring(0, seperatorpos), out burst);
      }
      else
      {
        s = sweapon.Trim();
      }

      if (owner.WeaponSystemInfo.Weapons.ContainsKey(s))
      {
        weapon = owner.WeaponSystemInfo.Weapons[s];
      }
    }

    public virtual bool FireWeapon(ActorInfo owner, ActorInfo target, string sweapon)
    {
      if (owner == null)
        return false;

      WeaponInfo weapon = null;
      int burst = 0;

      // AI Determination
      if (sweapon == "auto")
      {
        foreach (string ws in owner.WeaponSystemInfo.AIWeapons)
        {
          if (FireWeapon(owner, target, ws))
            return true;
        }
      }
      else
      {
        InterpretWeapon(owner, sweapon, out weapon, out burst);

        if (weapon != null)
          return weapon.Fire(Engine, owner, target, burst);
      }
      
      return false;
    }

    public virtual void FireAggregation(ActorInfo owner, ActorInfo target, ActorTypeInfo weapontype)
    {
      float accuracy = 1;

      float d = ActorDistanceInfo.GetDistance(owner, target) / weapontype.MaxSpeed;
      TV_3DVECTOR angle = Utilities.GetRotation(target.GetGlobalPosition() - owner.GetGlobalPosition()) - owner.GetGlobalRotation();
      angle.x -= (int)((angle.x + 180) / 360) * 360;
      angle.y -= (int)((angle.y + 180) / 360) * 360;

      accuracy /= (d + 1);
      accuracy /= (Math.Abs(angle.x) + 1);
      accuracy /= (Math.Abs(angle.y) + 1);

      if (Engine.Random.NextDouble() < accuracy)
        target.InflictDamage(owner, weapontype.ImpactDamage, DamageType.NORMAL, target.GetGlobalPosition());
    }

    public void Dispose()
    {
    }

    public virtual void Dying(ActorInfo ainfo)
    {
      if (ainfo == null)
        return;

      ainfo.DyingMoveComponent?.Initialize(ainfo, ref ainfo.MoveData);

      if (ainfo.IsPlayer)
      {
        PlayerInfo.ActorID = -1;
        PlayerCameraInfo.Look.SetPosition_Actor(ainfo.ID);
        PlayerCameraInfo.Look.SetModeDeathCircle(DeathCamera);

        ainfo.TickEvents += GameScenarioManager.Scenario.ProcessPlayerDying;
        ainfo.DestroyedEvents += GameScenarioManager.Scenario.ProcessPlayerKilled;
      }
    }

    public virtual void Dead(ActorInfo ainfo)
    {
      if (ainfo == null)
        return;

      // Explode
      ExplosionSystem.OnDeath(Engine, ainfo);

      // Debris
      if (!ainfo.IsAggregateMode && !Game.IsLowFPS())
        foreach (DebrisSpawnerInfo ds in Debris)
          ds.Process(ainfo);

      if (ainfo.IsPlayer)
      {
        PlayerInfo.ActorID = -1;
        PlayerCameraInfo.Look.SetPosition_Actor(ainfo.ID);
        PlayerCameraInfo.Look.SetModeDeathCircle(DeathCamera);
        ainfo.DestroyedEvents += GameScenarioManager.Scenario.ProcessPlayerKilled;
      }
      else
      {
        if (ainfo.UseParentCoords && ainfo.TopParent.IsPlayer)
          Screen2D.MessageSystemsText("WARNING: Subsystem [{0}] lost.".F(ainfo.Name), 3, new TV_COLOR(1, 0.2f, 0.2f, 1));
      }
    }
  }
}
