using SWEndor.ActorTypes;
using SWEndor.FileFormat.INI;
using SWEndor.Models;
using SWEndor.ProjectileTypes;

namespace SWEndor.Weapons
{
  internal struct WeapTgtInfo
  {
    // Targeter
    public bool RequirePlayerTargetLock;
    public bool RequireAITargetLock; // TO-DO: Implement logic
    public float TargetLock_TimeRequired; // TO-DO: Implement logic

    public TargetAcqType PlayerTargetAcqType; // TO-DO: Implement logic
    public TargetAcqType AITargetAcqType; // TO-DO: Implement logic

    // AI Config
    public TargetType AIAttackTargets;
    public bool AIAttackNull;
    public float AngularRange;
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


    public void LoadFromINI(INIFile f, string sectionname)
    {
      this = Default;

      RequirePlayerTargetLock = f.GetBool(sectionname, "RequirePlayerTargetLock", RequirePlayerTargetLock);
      RequireAITargetLock = f.GetBool(sectionname, "RequireAITargetLock", RequireAITargetLock);
      TargetLock_TimeRequired = f.GetFloat(sectionname, "TargetLock_TimeRequired", TargetLock_TimeRequired);
      PlayerTargetAcqType = f.GetEnumValue(sectionname, "PlayerTargetAcqType", PlayerTargetAcqType);
      AITargetAcqType = f.GetEnumValue(sectionname, "AITargetAcqType", AITargetAcqType);
      AIAttackTargets = f.GetEnumValue(sectionname, "AIAttackTargets", AIAttackTargets);
      AIAttackNull = f.GetBool(sectionname, "AIAttackNull", AIAttackNull);
      AngularRange = f.GetFloat(sectionname, "AngularRange", AngularRange);
      Range = f.GetFloat(sectionname, "Range", Range);
    }
  }
}
