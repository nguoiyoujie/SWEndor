using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class MC90ATI : WarshipGroup
  {
    private static MC90ATI _instance;
    public static MC90ATI Instance()
    {
      if (_instance == null) { _instance = new MC90ATI(); }
      return _instance;
    }

    private MC90ATI() : base("Mon Calamari Capital Ship")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 3000.0f;
      ImpactDamage = 60.0f;
      MaxSpeed = 30.0f;
      MinSpeed = 0.0f;
      MaxSpeedChangeRate = 5.0f;
      MaxTurnRate = 3f;
      ZNormFrac = 0.006f;
      ZTilt = 5f;

      Score_perStrength = 20;
      Score_DestroyBonus = 20000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"mc90\mc90.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.CamDeathCircleRadius = 1500;
      ainfo.CamDeathCircleHeight = 250;
      ainfo.CamDeathCirclePeriod = 30;

      ainfo.SetStateS("AddOn_0", "MC90 Turbolaser Tower, 100, 75, 500, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_1", "MC90 Turbolaser Tower, -100, 75, 500, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_2", "MC90 Turbolaser Tower, 0, 135, 0, 0, -90, 0, true");
      //bottom
      ainfo.SetStateS("AddOn_3", "MC90 Turbolaser Tower, 0, -40, 700, 90, 0, 0, true");
      ainfo.SetStateS("AddOn_4", "MC90 Turbolaser Tower, 0, 40, 700, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_5", "MC90 Turbolaser Tower, 170, 40, 100, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_6", "MC90 Turbolaser Tower, -170, 40, 100, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_7", "MC90 Turbolaser Tower, 210, 40, -300, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_8", "MC90 Turbolaser Tower, -210, 40, -300, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_9", "MC90 Turbolaser Tower, 170, 40, -600, -90, 0, 0, true");
      ainfo.SetStateS("AddOn_10", "MC90 Turbolaser Tower, -170, 40, -600, -90, 0, 0, true");

      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 45, 660));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 45, 2000));

      // Generate States
      ainfo.SetStateF("WingspawnRemaining", 99);
      ainfo.SetStateF("WingspawnCooldown", Game.Instance().GameTime + 45f);
      ainfo.SetStateF("WingspawnMove", 0f);

      ainfo.EnableDeathExplosion = true;
      ainfo.DeathExplosionType = "ExplosionLg";
      ainfo.DeathExplosionSize = 3.5f;
      ainfo.ExplosionRate = 0.5f;
      ainfo.ExplosionSize = 1;
      ainfo.ExplosionType = "ExplosionSm";

      ainfo.SelfRegenRate = 0.2f;

      ainfo.Scale *= 1.1f;
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);

      if (ainfo.CreationState == CreationState.ACTIVE)
      {
        TV_3DVECTOR engineloc = ainfo.GetRelativePositionXYZ(0, 0, -750 - z_displacement);
        float dist = Engine.Instance().TVMathLibrary.GetDistanceVec3D(PlayerInfo.Instance().Position, engineloc);

        if (!ainfo.IsPlayer())
        {
          if (dist < 1500)
          {
            if (PlayerInfo.Instance().enginelgvol < 1 - dist / 1500.0f)
              PlayerInfo.Instance().enginelgvol = 1 - dist / 1500.0f;
          }
        }

        if (ainfo.GetStateF("WingspawnMove") < Game.Instance().GameTime)
        {
          List<ActorInfo> rm = new List<ActorInfo>();
          foreach (ActorInfo a in ainfo.GetAllChildren(1))
          {
            if (a.TypeInfo is FighterGroup || a.TypeInfo is TIEGroup)
            {
              a.ActorState = ActorState.NORMAL;
              ActionManager.Unlock(a);
              ActionManager.QueueLast(a, new Actions.Hunt());
              rm.Add(a);

              if (a.IsPlayer())
              {
                PlayerInfo.Instance().IsMovementControlsEnabled = true;
              }
            }
          }

          foreach (ActorInfo a in rm)
          {
            ainfo.RemoveChild(a);
          }
        }
        else
        {
          float time = ainfo.GetStateF("WingspawnMove") - Game.Instance().GameTime;
          foreach (ActorInfo a in ainfo.GetAllChildren(1))
          {
            if (a.TypeInfo is FighterGroup || a.TypeInfo is TIEGroup)
            {
              a.Speed = 75;
              a.MoveRelative(0, 0, -ainfo.Speed * Game.Instance().TimeSinceRender * ainfo.Scale.x);

              if (a.IsPlayer())
              {
                PlayerInfo.Instance().IsMovementControlsEnabled = false;
              }
            }
          }
        }
      }
    }


    public override bool FireWeapon(ActorInfo ainfo, ActorInfo target, string weapon)
    {
      // Mon Calamari
      //Weapons: Wingspawn
      //Weapon0State: Wingspawn cooldown
      //Weapon1State: Wingspawn count remaining
      //Weapon2State: Wingspawn begin move

      if (weapon == "auto")
      {
        weapon = "Wingspawn";
      }

      if (weapon == "Wingspawn")
      {
        if (!GameScenarioManager.Instance().GetGameStateB("PendingPlayerSpawn") && ainfo.GetStateF("WingspawnCooldown") < Game.Instance().GameTime && ainfo.GetStateF("WingspawnRemaining") > 0)
        {
          ainfo.SetStateF("WingspawnCooldown", Game.Instance().GameTime + 10f);
          ainfo.SetStateF("WingspawnRemaining",ainfo.GetStateF("WingspawnRemaining") - 1);

          List<TV_3DVECTOR> spawnloc = new List<TV_3DVECTOR>();
          spawnloc.Add(new TV_3DVECTOR(75, -30, -200));

          foreach (TV_3DVECTOR sv in spawnloc)
          {
            ActorTypeInfo[] types = new ActorTypeInfo[] { XWingATI.Instance(), YWingATI.Instance(), AWingATI.Instance(), BWingATI.Instance() };
            int n = Engine.Instance().Random.Next(0, types.Length);
            ActorCreationInfo acinfo = new ActorCreationInfo(types[n]);

            TV_3DVECTOR clone = ainfo.GetRelativePositionXYZ(sv.x * ainfo.Scale.x, sv.y * ainfo.Scale.y, sv.z * ainfo.Scale.z - z_displacement);
            acinfo.Position = new TV_3DVECTOR(clone.x, clone.y, clone.z);
            TV_3DVECTOR rot = ainfo.GetRotation();
            rot = new TV_3DVECTOR(rot.x, rot.y + 90, rot.z);
            acinfo.Rotation = rot;

            acinfo.InitialState = ActorState.FREE;
            acinfo.InitialSpeed = 75;
            acinfo.Faction = ainfo.Faction;
            ActorInfo a = ActorInfo.Create(acinfo);
            a.AddParent(ainfo);
            a.Faction = ainfo.Faction;
            ActionManager.QueueNext(a, new Actions.Lock());
          }
          ainfo.SetStateF("WingspawnMove", Game.Instance().GameTime + 3f);
        }
        return true;
      }
      else if (weapon == "Playerspawn")
      {
        if (ainfo.GetStateF("WingspawnCooldown") < Game.Instance().GameTime + 25f && ainfo.GetStateF("WingspawnRemaining") > 0)
        {
          ainfo.SetStateF("WingspawnCooldown", Game.Instance().GameTime + 10f);
        }

        List<TV_3DVECTOR> spawnloc = new List<TV_3DVECTOR>();
        spawnloc.Add(new TV_3DVECTOR(95, -30, -200));

        foreach (TV_3DVECTOR sv in spawnloc)
        {
          PlayerInfo.Instance().IsMovementControlsEnabled = false;

          ActorCreationInfo acinfo = new ActorCreationInfo(PlayerInfo.Instance().ActorType);

          TV_3DVECTOR clone = ainfo.GetRelativePositionXYZ(sv.x * ainfo.Scale.x, sv.y * ainfo.Scale.y, sv.z * ainfo.Scale.z - z_displacement);
          acinfo.Position = new TV_3DVECTOR(clone.x, clone.y, clone.z);
          TV_3DVECTOR rot = ainfo.GetRotation();
          rot = new TV_3DVECTOR(rot.x, rot.y + 90, rot.z);
          acinfo.Rotation = rot;

          acinfo.InitialState = ActorState.FREE;
          acinfo.InitialSpeed = 100;
          acinfo.Faction = ainfo.Faction;
          ActorInfo a = ActorInfo.Create(acinfo);
          a.AddParent(ainfo);
          ActionManager.QueueNext(a, new Actions.Lock());

          PlayerInfo.Instance().Actor = a;
        }
        ainfo.SetStateF("WingspawnMove", Game.Instance().GameTime + 3f);

        return true;
      }
      return false;
    }
  }
}

