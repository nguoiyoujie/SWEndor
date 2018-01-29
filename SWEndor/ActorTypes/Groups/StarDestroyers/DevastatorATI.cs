using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class DevastatorATI : StarDestroyerGroup
  {
    private static DevastatorATI _instance;
    public static DevastatorATI Instance()
    {
      if (_instance == null) { _instance = new DevastatorATI(); }
      return _instance;
    }

    private DevastatorATI() : base("Devastator Imperial-I Star Destroyer")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 950.0f;
      ImpactDamage = 60.0f;
      MaxSpeed = 65.0f;
      MinSpeed = 0.0f;
      MaxSpeedChangeRate = 5.0f;
      MaxTurnRate = 1.2f;

      Score_perStrength = 70;
      Score_DestroyBonus = 20000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"stardestroyer\star_destroyer.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      List<float[]> ttowers = new List<float[]>();

      // Sides
      ainfo.SetStateS("AddOn_0", "Star Destroyer Turbolaser Tower, 240, -40, 320, 0, 72, 0, true");
      ainfo.SetStateS("AddOn_1", "Star Destroyer Heavy Turbolaser Tower, 210, -40, 410, 0, 72, 0, true");
      ainfo.SetStateS("AddOn_2", "Star Destroyer Turbolaser Tower, 180, -40, 500, 0, 72, 0, true");
      ainfo.SetStateS("AddOn_3", "Star Destroyer Turbolaser Tower, 360, -40, -100, 0, 72, 0, true");

      ainfo.SetStateS("AddOn_4", "Star Destroyer Turbolaser Tower, -240, -40, 320, 0, -72, 0, true");
      ainfo.SetStateS("AddOn_5", "Star Destroyer Heavy Turbolaser Tower, -210, -40, 410, 0, -72, 0, true");
      ainfo.SetStateS("AddOn_6", "Star Destroyer Turbolaser Tower, -180, -40, 500, 0, -72, 0, true");
      ainfo.SetStateS("AddOn_7", "Star Destroyer Turbolaser Tower, -360, -40, -100, 0, -72, 0, true");

      // Front
      ainfo.SetStateS("AddOn_8", "Star Destroyer Turbolaser Tower, 0, -40, 1040, 0, 0, 0, true");

      // Top
      ainfo.SetStateS("AddOn_9", "Star Destroyer Heavy Turbolaser Tower, 100, 20, 320, 255, -90, 90, true");
      ainfo.SetStateS("AddOn_10", "Star Destroyer Heavy Turbolaser Tower, -100, 20, 320, 255, 90, 90, true");
      ainfo.SetStateS("AddOn_11", "Star Destroyer Turbolaser Tower, 75, 20, 420, 255, -90, 90, true");
      ainfo.SetStateS("AddOn_12", "Star Destroyer Turbolaser Tower, -75, 20, 420, 255, 90, 90, true");

      // Bottom
      ainfo.SetStateS("AddOn_13", "Star Destroyer Turbolaser Tower, -120, -105, 180, 75, -90, 90, true");
      ainfo.SetStateS("AddOn_14", "Star Destroyer Turbolaser Tower, 120, -105, 180, 75, 90, 90, true");

      ainfo.SetStateS("AddOn_15", "Star Destroyer Turbolaser Tower, 360, -100, -425, 75, 90, 90, true");
      ainfo.SetStateS("AddOn_16", "Star Destroyer Turbolaser Tower, -360, -100, -400, 75, -90, 90, true");

      //Shield Generators
      ainfo.SetStateS("AddOn_17", "Star Destroyer Shield Generator, -120, 360, -415, 0, 0, 0, true");
      ainfo.SetStateS("AddOn_18", "Star Destroyer Shield Generator, 120, 360, -415, 0, 0, 0, true");
      ainfo.SetStateS("AddOn_19", "Star Destroyer Lower Shield Generator, 0, -180, -250, 0, 0, 0, true");

      // Top 2
      ainfo.SetStateS("AddOn_20", "Star Destroyer Heavy Turbolaser Tower, 290, 16, -230, 255, -90, 90, true");
      ainfo.SetStateS("AddOn_21", "Star Destroyer Heavy Turbolaser Tower, 290, 16, -283, 255, -90, 90, true");
      ainfo.SetStateS("AddOn_22", "Star Destroyer Heavy Turbolaser Tower, 290, 16, -336, 255, -90, 90, true");
      ainfo.SetStateS("AddOn_23", "Star Destroyer Heavy Turbolaser Tower, 290, 16, -389, 255, -90, 90, true");
      ainfo.SetStateS("AddOn_24", "Star Destroyer Heavy Turbolaser Tower, -290, 16, -230, 255, 90, 90, true");
      ainfo.SetStateS("AddOn_25", "Star Destroyer Heavy Turbolaser Tower, -290, 16, -283, 255, 90, 90, true");
      ainfo.SetStateS("AddOn_26", "Star Destroyer Heavy Turbolaser Tower, -290, 16, -336, 255, 90, 90, true");
      ainfo.SetStateS("AddOn_27", "Star Destroyer Heavy Turbolaser Tower, -290, 16, -389, 255, 90, 90, true");

      ainfo.SetStateS("AddOn_28", "Star Destroyer Turbolaser Tower, 70, 35, 75, 255, -90, 90, true");
      ainfo.SetStateS("AddOn_29", "Star Destroyer Turbolaser Tower, -70, 35, 75, 255, 90, 90, true");
      ainfo.SetStateS("AddOn_30", "Star Destroyer Turbolaser Tower, 70, 35, 170, 255, -90, 90, true");
      ainfo.SetStateS("AddOn_31", "Star Destroyer Turbolaser Tower, -70, 35, 170, 255, 90, 90, true");

      // Camera System
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 300, -385));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 300, 2000));

      // Generate States
      ainfo.SetStateF("TIEspawnRemaining", 99);
      ainfo.SetStateF("TIEspawnCooldown", Game.Instance().GameTime + 15f);
      ainfo.SetStateF("TIEspawnMove", 0f);

      ainfo.EnableDeathExplosion = true;
      ainfo.DeathExplosionType = "ExplosionLg";
      ainfo.DeathExplosionSize = 2.5f;
      ainfo.ExplosionRate = 0.25f;
      ainfo.ExplosionSize = 1;
      ainfo.ExplosionType = "ExplosionSm";

      ainfo.Scale *= 1.7f;
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);

      if (ainfo.CreationState == CreationState.ACTIVE)
      {
        TV_3DVECTOR engineloc = ainfo.GetRelativePositionXYZ(0, 0, -400 - z_displacement);
        float dist = Engine.Instance().TVMathLibrary.GetDistanceVec3D(PlayerInfo.Instance().Position, engineloc);

        if (!ainfo.IsPlayer())
        {
          if (dist < 500)
          {
            if (PlayerInfo.Instance().enginelgvol < 1 - dist / 500.0f)
              PlayerInfo.Instance().enginelgvol = 1 - dist / 500.0f;
          }
        }

        if (ainfo.GetStateF("TIEspawnMove") < Game.Instance().GameTime)
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
            }
          }

          foreach (ActorInfo a in rm)
          {
            ainfo.RemoveChild(a);
          }
        }
        else
        {
          float time = ainfo.GetStateF("TIEspawnMove") - Game.Instance().GameTime;
          float spd = 15f;
          foreach (ActorInfo a in ainfo.GetAllChildren(1))
          {
            if (a.TypeInfo is FighterGroup || a.TypeInfo is TIEGroup)
            {
              a.Speed = ainfo.Speed;
              a.MoveRelative(0, -time * spd * Game.Instance().TimeSinceRender * ainfo.Scale.y, 0);
            }
          }
        }
      }
    }

    public override bool FireWeapon(ActorInfo ainfo, ActorInfo target, string weapon)
    {
      // Imperial Star Destroyer
      //Weapons: TIEspawn
      //Weapon0State: TIEspawn cooldown
      //Weapon1State: TIEspawn count remaining
      //Weapon2State: TIEspawn begin move
      //if (ainfo.Mesh == null)
      //  return false;

      if (weapon == "auto")
      {
        weapon = "TIEspawn";
      }

      if (weapon == "TIEspawn")
      {
        if (ainfo.GetStateF("TIEspawnCooldown") < Game.Instance().GameTime && ainfo.GetStateF("TIEspawnRemaining") > 0)
        {
          ainfo.SetStateF("TIEspawnCooldown", Game.Instance().GameTime + 5f);
          ainfo.SetStateF("TIEspawnRemaining", ainfo.GetStateF("TIEspawnRemaining")-1);

          List<TV_3DVECTOR> spawnloc = new List<TV_3DVECTOR>();
          //spawnloc.Add(new TV_3DVECTOR(0, -210, 250));
          spawnloc.Add(new TV_3DVECTOR(25, -80, 175));
          spawnloc.Add(new TV_3DVECTOR(25, -80, 225));
          spawnloc.Add(new TV_3DVECTOR(-25, -80, 175));
          spawnloc.Add(new TV_3DVECTOR(-25, -80, 225));

          foreach (TV_3DVECTOR sv in spawnloc)
          {
            ActorCreationInfo acinfo = new ActorCreationInfo(TIE_IN_ATI.Instance());
            TV_3DVECTOR clone = ainfo.GetRelativePositionXYZ(sv.x * ainfo.Scale.x, sv.y * ainfo.Scale.y, sv.z * ainfo.Scale.z - z_displacement);
            acinfo.Position = new TV_3DVECTOR(clone.x, clone.y, clone.z);
            acinfo.Rotation = new TV_3DVECTOR(ainfo.Rotation.x, ainfo.Rotation.y, ainfo.Rotation.z);
            acinfo.InitialState = ActorState.FREE;
            acinfo.InitialSpeed = 0;
            ActorInfo a = ActorInfo.Create(acinfo);
            a.AddParent(ainfo);
            a.Faction = ainfo.Faction;
            ActionManager.QueueFirst(a, new Actions.Lock());
          }
          ainfo.SetStateF("TIEspawnMove", Game.Instance().GameTime + 4f);
        }
        return true;
      }
      return false;
    }
  }
}

