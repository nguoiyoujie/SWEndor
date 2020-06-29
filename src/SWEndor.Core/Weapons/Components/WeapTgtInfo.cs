using SWEndor.ActorTypes;
using Primrose.FileFormat.INI;
using SWEndor.Models;
using SWEndor.ProjectileTypes;

namespace SWEndor.Weapons
{
  internal struct WeapTgtInfo
  {
    // Targeter
    [INIValue]
    public bool RequirePlayerTargetLock;

    [INIValue]
    public bool RequireAITargetLock; // TO-DO: Implement logic

    [INIValue]
    public float TargetLock_TimeRequired; // TO-DO: Implement logic

    [INIValue]
    public TargetAcqType PlayerTargetAcqType; // TO-DO: Implement logic

    [INIValue]
    public TargetAcqType AITargetAcqType; // TO-DO: Implement logic

    // AI Config
    [INIValue]
    public TargetType AIAttackTargets;

    [INIValue]
    public bool AIAttackNull;

    [INIValue]
    public float AngularRange;

    [INIValue]
    public float Range;

    public static WeapTgtInfo Default = new WeapTgtInfo
    {
      RequirePlayerTargetLock = false,
      RequireAITargetLock = false,
      TargetLock_TimeRequired = 0,

      PlayerTargetAcqType = TargetAcqType.ANY,
      AITargetAcqType = TargetAcqType.ENEMIES,

      // AI Config
      AIAttackTargets = TargetType.ANY,
      AIAttackNull = true,
      AngularRange = 10,
      Range = 4500
    };

    public void Init(ProjectileTypeInfo projt)
    {
      if (Range == 0)
        Range = projt.MoveLimitData.MaxSpeed * projt.TimedLifeData.TimedLife;
    }

    public void Init(ActorTypeInfo actt)
    {
      if (Range == 0)
        Range = actt.MoveLimitData.MaxSpeed * actt.TimedLifeData.TimedLife;
    }
  }
}
