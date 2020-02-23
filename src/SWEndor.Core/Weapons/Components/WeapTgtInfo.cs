using SWEndor.ActorTypes;
using Primitives.FileFormat.INI;
using SWEndor.Models;
using SWEndor.ProjectileTypes;

namespace SWEndor.Weapons
{
  internal struct WeapTgtInfo
  {
    private const string sNone = "";

    // Targeter
    [INIValue(sNone, "RequirePlayerTargetLock")]
    public bool RequirePlayerTargetLock;

    [INIValue(sNone, "RequireAITargetLock")]
    public bool RequireAITargetLock; // TO-DO: Implement logic

    [INIValue(sNone, "TargetLock_TimeRequired")]
    public float TargetLock_TimeRequired; // TO-DO: Implement logic

    [INIValue(sNone, "PlayerTargetAcqType")]
    public TargetAcqType PlayerTargetAcqType; // TO-DO: Implement logic

    [INIValue(sNone, "AITargetAcqType")]
    public TargetAcqType AITargetAcqType; // TO-DO: Implement logic

    // AI Config
    [INIValue(sNone, "AIAttackTargets")]
    public TargetType AIAttackTargets;

    [INIValue(sNone, "AIAttackNull")]
    public bool AIAttackNull;

    [INIValue(sNone, "AngularRange")]
    public float AngularRange;

    [INIValue(sNone, "Range")]
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
