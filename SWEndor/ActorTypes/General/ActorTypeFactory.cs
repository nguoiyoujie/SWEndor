using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor
{

  public class ActorTypeFactory
  {
    private static ActorTypeFactory _instance;
    public static ActorTypeFactory Instance()
    {
      if (_instance == null) { _instance = new ActorTypeFactory(); }
      return _instance;
    }

    private ActorTypeFactory()
    {
    }

    private static Dictionary<string, ActorTypeInfo> list = new Dictionary<string, ActorTypeInfo>();

    public void Initialise()
    {
      Register(SceneRoomATI.Instance());
      Register(EndorATI.Instance());
      Register(YavinATI.Instance());
      Register(Yavin4ATI.Instance());
      Register(HothATI.Instance());
      Register(SunATI.Instance());
      Register(DeathStarATI.Instance());
      Register(DeathStar2ATI.Instance());
      Register(InvisibleCameraATI.Instance());
      Register(DeathCameraATI.Instance());
      Register(ImperialIStaticATI.Instance());
      Register(ExecutorStaticATI.Instance());

      // explosions
      Register(ExplosionATI.Instance());
      Register(Explosion2ATI.Instance());
      Register(ExplosionSmATI.Instance());
      Register(ExplosionLgATI.Instance());
      Register(ExplosionWaveATI.Instance());
      Register(ElectroATI.Instance());

      // fighters
      Register(XWingATI.Instance());
      Register(YWingATI.Instance());
      Register(AWingATI.Instance());
      Register(BWingATI.Instance());
      Register(FalconATI.Instance());
      Register(LandoFalconATI.Instance());
      Register(WedgeXWingATI.Instance());

      // TIEs
      Register(TIE_LN_ATI.Instance());
      Register(TIE_IN_ATI.Instance());
      Register(TIE_D_ATI.Instance());
      Register(TIE_X1_ATI.Instance());
      Register(TIE_sa_ATI.Instance());

      // ships
      Register(CorellianATI.Instance());
      Register(NebulonBATI.Instance());
      Register(NebulonB2ATI.Instance());
      Register(MC90ATI.Instance());
      Register(TransportATI.Instance());

      // Star Destroyers
      Register(ImperialIATI.Instance());
      Register(DevastatorATI.Instance());
      Register(AcclamatorATI.Instance());
      Register(ArquitensATI.Instance());
      Register(ExecutorATI.Instance());

      // surface
      Register(Surface001_00ATI.Instance());
      Register(Surface001_01ATI.Instance());
      Register(Surface002_00ATI.Instance());
      Register(Surface002_01ATI.Instance());
      Register(Surface002_02ATI.Instance());
      Register(Surface002_03ATI.Instance());
      Register(Surface002_04ATI.Instance());
      Register(Surface002_05ATI.Instance());
      Register(Surface002_06ATI.Instance());
      Register(Surface002_07ATI.Instance());
      Register(Surface002_08ATI.Instance());
      Register(Surface002_09ATI.Instance());
      Register(Surface002_10ATI.Instance());
      Register(Surface002_11ATI.Instance());
      Register(Surface002_12ATI.Instance());
      Register(Surface002_99ATI.Instance());
      Register(Surface003_00ATI.Instance());

      // towers
      Register(Tower00ATI.Instance());
      Register(Tower01ATI.Instance());
      Register(Tower02ATI.Instance());
      Register(Tower03ATI.Instance());
      Register(Tower00_RuinsATI.Instance());
      Register(Tower01_RuinsATI.Instance());
      Register(Tower02_RuinsATI.Instance());
      Register(Tower03_RuinsATI.Instance());
      Register(TowerGunATI.Instance());
      Register(TowerGunAdvATI.Instance());

      // lasers
      Register(RedLaserATI.Instance());
      Register(GreenLaserATI.Instance());
      Register(Green2LaserATI.Instance());
      Register(GreenAntiShipLaserATI.Instance());
      Register(GreenLaserAdvancedATI.Instance());
      Register(YellowLaserATI.Instance());
      Register(SmallIonLaserATI.Instance());
      Register(DeathStarLaserATI.Instance());

      // torps
      Register(MissileATI.Instance());

      // add ons
      Register(mc90TurbolaserATI.Instance());
      Register(CorellianTurboLaserATI.Instance());
      Register(TransportTurboLaserATI.Instance());
      Register(NebulonBTurboLaserATI.Instance());
      Register(ACTurboLaserATI.Instance());
      Register(ArqTurboLaserATI.Instance());
      Register(SDAntiShipTurboLaserATI.Instance());
      Register(SDTurboLaserATI.Instance());
      Register(SDShieldGeneratorATI.Instance());
      Register(SDLowerShieldGeneratorATI.Instance());
      Register(ExecutorTurboLaserATI.Instance());
      Register(ExecutorShieldGeneratorATI.Instance());
      Register(ExecutorBridgeATI.Instance());
      Register(InvisibleRebelTurboLaserATI.Instance());
      Register(DSLaserSourceATI.Instance());

      // debris
      Register(XWing_RD_LU_WingATI.Instance());
      Register(XWing_RU_LD_WingATI.Instance());
      Register(TIE_WingATI.Instance());
      Register(TIE_InterceptorWingATI.Instance());
      Register(YWing_WingATI.Instance());
      Register(BWing_Top_WingATI.Instance());
      Register(BWing_Bottom_WingATI.Instance());
      Register(BWing_WingATI.Instance());
      Register(Transport_Box1ATI.Instance());
      Register(Transport_Box2ATI.Instance());
      Register(Transport_Box3ATI.Instance());
      Register(Transport_Box4ATI.Instance());

    }

    public void Register(ActorTypeInfo atype)
    {
      using (new PerfElement("preload_register"))
      {
        using (new PerfElement("preload_register_" + atype.Name))
        {
          if (list.ContainsKey(atype.Name))
          {
            atype = list[atype.Name];
          }
          else
          {
            list.Add(atype.Name, atype);
          }
          atype.RegisterModel();
          Screen2D.Instance().LoadingTextLines.Add(atype.Name + " loaded!");
        }
      }
    }

    public ActorTypeInfo GetActorType(string name)
    {
      if (list.ContainsKey(name))
        return list[name];
      else
        throw new Exception("ActorTypeInfo '" + name + "' does not exist");
    }

    public void RemoveActorType(string name)
    {
      if (list.ContainsKey(name))
        list.Remove(name);
    }
  }
}
