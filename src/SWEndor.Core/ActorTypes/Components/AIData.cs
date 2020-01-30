using Primitives.FileFormat.INI;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Components
{
  internal struct AIData
  {
    // AI
    public float Attack_AngularDelta;
    public float Attack_HighAccuracyAngularDelta;
    public float Move_CloseEnough;

    public bool AggressiveTracker;
    public bool AlwaysAccurateRotation;

    public bool CanEvade;
    public bool CanRetaliate;
    public bool CanCheckCollisionAhead;

    // Targeting
    public TargetType TargetType;
    public int HuntWeight;

    public readonly static AIData Default = 
      new AIData
        {
          Attack_AngularDelta = 5f,
          Attack_HighAccuracyAngularDelta = 1f,
          Move_CloseEnough = 500,
          TargetType = TargetType.NULL,
          HuntWeight = 1
        };

    public void LoadFromINI(INIFile f, string sectionname)
    {
      Attack_AngularDelta = f.GetFloat(sectionname, "Attack_AngularDelta", Attack_AngularDelta);
      Attack_HighAccuracyAngularDelta = f.GetFloat(sectionname, "Attack_HighAccuracyAngularDelta", Attack_HighAccuracyAngularDelta);
      Move_CloseEnough = f.GetFloat(sectionname, "Move_CloseEnough", Move_CloseEnough);

      AggressiveTracker = f.GetBool(sectionname, "AggressiveTracker", AggressiveTracker);
      AlwaysAccurateRotation = f.GetBool(sectionname, "AlwaysAccurateRotation", AlwaysAccurateRotation);

      CanEvade = f.GetBool(sectionname, "CanEvade", CanEvade);
      CanRetaliate = f.GetBool(sectionname, "CanRetaliate", CanRetaliate);
      CanCheckCollisionAhead = f.GetBool(sectionname, "CanCheckCollisionAhead", CanCheckCollisionAhead);

      TargetType = f.GetEnum(sectionname, "TargetType", TargetType);
      HuntWeight = f.GetInt(sectionname, "HuntWeight", HuntWeight);

    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      f.SetFloat(sectionname, "Attack_AngularDelta", Attack_AngularDelta);
      f.SetFloat(sectionname, "Attack_HighAccuracyAngularDelta", Attack_HighAccuracyAngularDelta);
      f.SetFloat(sectionname, "Move_CloseEnough", Move_CloseEnough);

      f.SetBool(sectionname, "AggressiveTracker", AggressiveTracker);
      f.SetBool(sectionname, "AlwaysAccurateRotation", AlwaysAccurateRotation);

      f.SetBool(sectionname, "CanEvade", CanEvade);
      f.SetBool(sectionname, "CanRetaliate", CanRetaliate);
      f.SetBool(sectionname, "CanCheckCollisionAhead", CanCheckCollisionAhead);

      f.SetEnum(sectionname, "TargetType", TargetType);
      f.SetInt(sectionname, "HuntWeight", HuntWeight);
    }
  }
}

