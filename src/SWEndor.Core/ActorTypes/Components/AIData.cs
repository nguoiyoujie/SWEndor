using Primitives.FileFormat.INI;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Components
{
  internal struct AIData
  {
    private const string sAI = "AI";

    // AI
    [INIValue(sAI, "Attack_AngularDelta")]
    public float Attack_AngularDelta;

    [INIValue(sAI, "Attack_HighAccuracyAngularDelta")]
    public float Attack_HighAccuracyAngularDelta;

    [INIValue(sAI, "Move_CloseEnough")]
    public float Move_CloseEnough;

    [INIValue(sAI, "AggressiveTracker")]
    public bool AggressiveTracker;

    [INIValue(sAI, "AlwaysAccurateRotation")]
    public bool AlwaysAccurateRotation;

    [INIValue(sAI, "CanEvade")]
    public bool CanEvade;

    [INIValue(sAI, "CanRetaliate")]
    public bool CanRetaliate;

    [INIValue(sAI, "CanCheckCollisionAhead")]
    public bool CanCheckCollisionAhead;

    // Targeting
    [INIValue(sAI, "TargetType")]
    public TargetType TargetType;

    [INIValue(sAI, "HuntWeight")]
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
  }
}

