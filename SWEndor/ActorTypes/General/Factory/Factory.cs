using SWEndor.ActorTypes.Instances;
using SWEndor.Core;
using SWEndor.Primitives.Extensions;
using SWEndor.Primitives.Factories;
using System;
using System.IO;

namespace SWEndor.ActorTypes
{
  public partial class ActorTypeInfo
  {
    public class Factory : Registry<ActorTypeInfo>
    {
      public readonly Engine Engine;
      internal Factory(Engine engine)
      { Engine = engine; }

      public void RegisterBase()
      {
        Register(new Hyperspace(this));

        Register(new EndorATI(this));
        Register(new YavinATI(this));
        Register(new Yavin4ATI(this));
        Register(new HothATI(this));
        Register(new DeathStarATI(this));
        Register(new DeathStar2ATI(this));
        Register(new ImperialIStaticATI(this));
        Register(new ExecutorStaticATI(this));

        // debris (load before their parents)
        Register(new XWing_RD_LU_WingATI(this));
        Register(new XWing_RU_LD_WingATI(this));
        Register(new TIE_WingATI(this));
        Register(new TIE_InterceptorWingATI(this));
        Register(new YWing_WingATI(this));
        Register(new BWing_Top_WingATI(this));
        Register(new BWing_Bottom_WingATI(this));
        Register(new BWing_WingATI(this));
        Register(new Transport_Box1ATI(this));
        Register(new Transport_Box2ATI(this));
        Register(new Transport_Box3ATI(this));
        Register(new Transport_Box4ATI(this));

        // fighters
        Register(new Z95ATI(this));
        Register(new XWingATI(this));
        Register(new YWingATI(this));
        Register(new AWingATI(this));
        Register(new BWingATI(this));
        Register(new FalconATI(this));
        Register(new LandoFalconATI(this));
        Register(new WedgeXWingATI(this));

        // TIEs
        Register(new TIE_LN_ATI(this));
        Register(new TIE_IN_ATI(this));
        Register(new TIE_D_ATI(this));
        Register(new TIE_A_ATI(this));
        Register(new TIE_X1_ATI(this));
        Register(new TIE_sa_ATI(this));
        Register(new AssaultGunboatATI(this));
        Register(new MissileBoatATI(this));

        // ships
        Register(new CorellianATI(this));
        Register(new NebulonBATI(this));
        Register(new NebulonB2ATI(this));
        Register(new MCLATI(this));
        Register(new MC90ATI(this));
        Register(new MC80BATI(this));
        Register(new TransportATI(this));

        // Star Destroyers
        Register(new ImperialIATI(this));
        Register(new DevastatorATI(this));
        Register(new InterdictorATI(this));
        Register(new VictoryIATI(this));
        Register(new AcclamatorATI(this));
        Register(new ArquitensATI(this));
        Register(new ExecutorATI(this));

        // Platforms
        Register(new XQ1ATI(this));

        // surface
        Register(new Surface001_00ATI(this));
        Register(new Surface001_01ATI(this));
        Register(new Surface002_00ATI(this));
        Register(new Surface002_01ATI(this));
        Register(new Surface002_02ATI(this));
        Register(new Surface002_03ATI(this));
        Register(new Surface002_04ATI(this));
        Register(new Surface002_05ATI(this));
        Register(new Surface002_06ATI(this));
        Register(new Surface002_07ATI(this));
        Register(new Surface002_08ATI(this));
        Register(new Surface002_09ATI(this));
        Register(new Surface002_10ATI(this));
        Register(new Surface002_11ATI(this));
        Register(new Surface002_12ATI(this));
        Register(new Surface002_99ATI(this));
        Register(new Surface003_00ATI(this));

        // towers
        Register(new Tower00ATI(this));
        Register(new Tower01ATI(this));
        Register(new Tower02ATI(this));
        Register(new Tower03ATI(this));
        Register(new Tower04ATI(this));
        Register(new Tower00_RuinsATI(this));
        Register(new Tower01_RuinsATI(this));
        Register(new Tower02_RuinsATI(this));
        Register(new Tower03_RuinsATI(this));
        Register(new TowerGunATI(this));
        Register(new TowerGunAdvATI(this));
        Register(new TowerGunSuperATI(this));
        Register(new SurfaceVentATI(this));

        // lasers
        Register(new RedLaserATI(this));
        Register(new GreenLaserATI(this));
        Register(new Green2LaserATI(this));
        Register(new GreenAntiShipLaserATI(this));
        Register(new GreenLaserAdvancedATI(this));
        Register(new YellowLaserATI(this));
        Register(new Yellow2LaserATI(this));
        Register(new SmallIonLaserATI(this));
        Register(new BigIonLaserATI(this));
        Register(new DeathStarLaserATI(this));

        // torps
        Register(new MissileATI(this));
        Register(new TorpedoATI(this));

        // add ons
        Register(new mc90TurbolaserATI(this));
        Register(new CorellianTurboLaserATI(this));
        Register(new TransportTurboLaserATI(this));
        Register(new NebulonBTurboLaserATI(this));
        Register(new NebulonBMissilePodATI(this));
        Register(new ACTurboLaserATI(this));
        Register(new XQTurboLaserATI(this));
        Register(new ArqTurboLaserATI(this));
        Register(new SDAntiShipTurboLaserATI(this));
        Register(new SDTurboLaserATI(this));
        Register(new SDMissilePodATI(this));
        Register(new ISDShieldGeneratorATI(this));
        Register(new SDShieldGeneratorATI(this));
        Register(new SDLowerShieldGeneratorATI(this));
        Register(new ExecutorTurboLaserATI(this));
        Register(new ExecutorShieldGeneratorATI(this));
        Register(new ExecutorBridgeATI(this));
        Register(new InvisibleRebelTurboLaserATI(this));
        Register(new DSLaserSourceATI(this));
        Register(new HangarBayATI(this));
        Register(new PlayerSpawnerATI(this));

        // asteroids
        Register(new Asteroid01ATI(this));
        Register(new Asteroid02ATI(this));
        Register(new Asteroid03ATI(this));
        Register(new Asteroid04ATI(this));
        Register(new Asteroid05ATI(this));
        Register(new Asteroid06ATI(this));
        Register(new Asteroid07ATI(this));
        Register(new Asteroid08ATI(this));
      }

      public void Register(ActorTypeInfo atype)
      {
        if (Contains(atype.ID))
        {
          atype = GetX(atype.ID);
        }
        else
        {
          Add(atype.ID, atype);
        }
        //atype.SaveToINI(atype.ID);
        Engine.Screen2D.LoadingTextLines.Add(string.Format("{0} loaded!", atype.Name));
      }

      public void Load()
      {
        foreach (string fp in Directory.GetFiles(Globals.ActorTypeINIDirectory, "*.ini", SearchOption.AllDirectories))
        {
          string f = Path.GetFileNameWithoutExtension(fp);
          if (Contains(f))
            throw new InvalidOperationException(TextLocalization.Get(TextLocalKeys.ACTORTYPE_INITWICE_ERROR).F(f));
          ActorTypeInfo t = new ActorTypeInfo(this, f, f);
          t.LoadFromINI(f);
        }
      }

      public void Initialise()
      {
        foreach (ActorTypeInfo atype in list.Values)
          atype.Init();
      }

      public new ActorTypeInfo Get(string id)
      {
        ActorTypeInfo ret = base.Get(id);
        if (ret == null)
          throw new Exception(TextLocalization.Get(TextLocalKeys.ACTORTYPE_INVALID_ERROR).F(id));

        return ret;
      }
    }
  }
}
