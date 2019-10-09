using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class XQ1ATI : Groups.Warship
  {
    internal XQ1ATI(Factory owner) : base(owner, "XQ1", "XQ1-Platform")
    {
      CombatData.MaxStrength = 1250.0f;
      CombatData.ImpactDamage = 60.0f;
      MoveLimitData.MaxSpeed = 0;
      MoveLimitData.MinSpeed = 0;

      RenderData.CullDistance = 40000;
      ScoreData = new ScoreData(10, 10000);

      Cameras = new LookData[] { new LookData(new TV_3DVECTOR(0, 45, 660), new TV_3DVECTOR(0, 45, 2000)) };
      DeathCamera = new DeathCameraData(1500, 250, 30);

      MeshData = new MeshData(Name, @"platform\xq1.x", 1.5f);
      RegenData = new RegenData(false, 0.5f, 0, 0.25f, 0);

      AddOns = new AddOnData[]
      {
        // roof
        new AddOnData("XQLSR", new TV_3DVECTOR(0, 35, 115), new TV_3DVECTOR(-90, 0, 0), true),
        new AddOnData("XQLSR", new TV_3DVECTOR(89, 35, -38), new TV_3DVECTOR(-90, 0, 120), true),
        new AddOnData("XQLSR", new TV_3DVECTOR(-89, 35, -38), new TV_3DVECTOR(-90, 0, -120), true),

        // platform
        new AddOnData("XQLSR", new TV_3DVECTOR(245, 2, -289), new TV_3DVECTOR(-90, 0, 120), true),
        new AddOnData("XQLSR", new TV_3DVECTOR(-245, 2, -289), new TV_3DVECTOR(-90, 0, 120), true),
        new AddOnData("XQLSR", new TV_3DVECTOR(384, 2, -48), new TV_3DVECTOR(-90, 0, -120), true),
        new AddOnData("XQLSR", new TV_3DVECTOR(-384, 2, -48), new TV_3DVECTOR(-90, 0, -120), true),
        new AddOnData("XQLSR", new TV_3DVECTOR(138, 2, 375), new TV_3DVECTOR(-90, 0, 0), true),
        new AddOnData("XQLSR", new TV_3DVECTOR(-138, 2, 375), new TV_3DVECTOR(-90, 0, 0), true)

      };
    }
  }
}

