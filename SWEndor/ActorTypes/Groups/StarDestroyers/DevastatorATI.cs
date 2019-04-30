using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.ActorTypes.Components;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class DevastatorATI : Groups.StarDestroyer
  {
    internal DevastatorATI(Factory owner) : base(owner, "Devastator Imperial-I Star Destroyer")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 1075.0f;
      ImpactDamage = 60.0f;
      MaxSpeed = 70.0f;
      MinSpeed = 0.0f;
      MaxSpeedChangeRate = 5.0f;
      MaxTurnRate = 1.2f;

      CullDistance = 40000;

      Score_perStrength = 70;
      Score_DestroyBonus = 20000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"stardestroyer\star_destroyer.x");
      SourceFarMeshPath = Path.Combine(Globals.ModelPath, @"stardestroyer\star_destroyer_far.x");

      SoundSources = new SoundSourceInfo[] { new SoundSourceInfo("engine_big", 1500f, new TV_3DVECTOR(0, -100, -400), true) };
      AddOns = new AddOnInfo[]
      {
        // Front
        new AddOnInfo("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(0, -40, 1040), new TV_3DVECTOR(0, 0, 0), true)

        // Sides
        , new AddOnInfo("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(360, -40, -100), new TV_3DVECTOR(0, 72, 0), true)
        , new AddOnInfo("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(240, -40, 320), new TV_3DVECTOR(0, 72, 0), true)
        , new AddOnInfo("Star Destroyer Heavy Turbolaser Tower", new TV_3DVECTOR(210, -40, 410), new TV_3DVECTOR(0, 72, 0), true)
        , new AddOnInfo("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(180, -40, 500), new TV_3DVECTOR(0, 72, 0), true)

        , new AddOnInfo("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-360, -40, -100), new TV_3DVECTOR(0, -72, 0), true)
        , new AddOnInfo("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-240, -40, 320), new TV_3DVECTOR(0, -72, 0), true)
        , new AddOnInfo("Star Destroyer Heavy Turbolaser Tower", new TV_3DVECTOR(-210, -40, 410), new TV_3DVECTOR(0, -72, 0), true)
        , new AddOnInfo("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-180, -40, 500), new TV_3DVECTOR(0, -72, 0), true)
        
        // Top
        , new AddOnInfo("Star Destroyer Heavy Turbolaser Tower", new TV_3DVECTOR(100, 20, 320), new TV_3DVECTOR(-90, 0, -15), true)
        , new AddOnInfo("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(72, 20, 420), new TV_3DVECTOR(-90, 0, -15), true)

        , new AddOnInfo("Star Destroyer Heavy Turbolaser Tower", new TV_3DVECTOR(-100, 20, 320), new TV_3DVECTOR(-90, 0, 15), true)
        , new AddOnInfo("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-72, 20, 420), new TV_3DVECTOR(-90, 0, 15), true)

        // Top Middle
        , new AddOnInfo("Star Destroyer Heavy Turbolaser Tower", new TV_3DVECTOR(72, 35, 75), new TV_3DVECTOR(-90, 0, -15), true)
        , new AddOnInfo("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(70, 35, 170), new TV_3DVECTOR(-90, 0, -15), true)

        , new AddOnInfo("Star Destroyer Heavy Turbolaser Tower", new TV_3DVECTOR(-72, 35, 75), new TV_3DVECTOR(-90, 0, 15), true)
        , new AddOnInfo("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-70, 35, 170), new TV_3DVECTOR(-90, 0, 15), true)

        // Top Rear
        , new AddOnInfo("Star Destroyer Heavy Turbolaser Tower", new TV_3DVECTOR(290, 16, -230), new TV_3DVECTOR(-90, 0, -15), true)
        , new AddOnInfo("Star Destroyer Heavy Turbolaser Tower", new TV_3DVECTOR(290, 18, -283), new TV_3DVECTOR(-90, 0, -15), true)
        , new AddOnInfo("Star Destroyer Missile Pod", new TV_3DVECTOR(290, 20, -336), new TV_3DVECTOR(-90, 0, -15), true)
        , new AddOnInfo("Star Destroyer Missile Pod", new TV_3DVECTOR(290, 22, -389), new TV_3DVECTOR(-90, 0, -15), true)

        , new AddOnInfo("Star Destroyer Heavy Turbolaser Tower", new TV_3DVECTOR(-290, 16, -230), new TV_3DVECTOR(-90, 0, 15), true)
        , new AddOnInfo("Star Destroyer Heavy Turbolaser Tower", new TV_3DVECTOR(-290, 18, -283), new TV_3DVECTOR(-90, 0, 15), true)
        , new AddOnInfo("Star Destroyer Missile Pod", new TV_3DVECTOR(-290, 20, -336), new TV_3DVECTOR(-90, 0, 15), true)
        , new AddOnInfo("Star Destroyer Missile Pod", new TV_3DVECTOR(-290, 22, -389), new TV_3DVECTOR(-90, 0, 15), true)

        // Bottom
        , new AddOnInfo("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-120, -105, 180), new TV_3DVECTOR(90, 0, -25), true)
        , new AddOnInfo("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(-360, -100, -400), new TV_3DVECTOR(90, 0, -25), true)

        , new AddOnInfo("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(120, -105, 180), new TV_3DVECTOR(90, 0, 25), true)
        , new AddOnInfo("Star Destroyer Turbolaser Tower", new TV_3DVECTOR(360, -100, -425), new TV_3DVECTOR(90, 0, 25), true)

        // Hangar Bay
        , new AddOnInfo("Hangar Bay", new TV_3DVECTOR(0, -80, 205), new TV_3DVECTOR(0, 0, 0), true)

        //Shield Generators
        , new AddOnInfo("Imperial Star Destroyer Shield Generator", new TV_3DVECTOR(-120, 360, -415), new TV_3DVECTOR(0, 0, 0), true)
        , new AddOnInfo("Imperial Star Destroyer Shield Generator", new TV_3DVECTOR(120, 360, -415), new TV_3DVECTOR(0, 0, 0), true)
        //, new AddOnInfo("Star Destroyer Lower Shield Generator", new TV_3DVECTOR(0, -180, -250), new TV_3DVECTOR(0, 0, 0), true)
      };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      List<float[]> ttowers = new List<float[]>();

      ainfo.CameraSystemInfo.CamLocations = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 300, -385) };
      ainfo.CameraSystemInfo.CamTargets = new TV_3DVECTOR[] { new TV_3DVECTOR(0, 300, 2000) };

      ainfo.ExplosionInfo.DeathExplosionSize = 2.5f;
      ainfo.ExplosionInfo.ExplosionRate = 0.25f;

      ainfo.DyingMoveComponent = new DyingSink(0.005f, 5f, 0.8f);

      ainfo.Scale *= 1.7f;

      ainfo.SpawnerInfo = new SDSpawnerII(ainfo);
    }
  }
}

