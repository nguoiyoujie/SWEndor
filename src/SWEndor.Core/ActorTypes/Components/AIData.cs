using Primrose.FileFormat.INI;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Components
{
  internal struct AIData
  {
    // AI
#pragma warning disable 0649 // values are filled by the attribute
    [INIValue]
    public float Attack_AngularDelta;

    [INIValue]
    public float Attack_HighAccuracyAngularDelta;

    [INIValue]
    public float Move_CloseEnough;

    [INIValue]
    public bool AggressiveTracker;

    [INIValue]
    public bool AlwaysAccurateRotation;

    [INIValue]
    public float AIMinSpeed;

    [INIValue]
    public float AIMaxSpeed;

    [INIValue]
    public bool CanEvade;

    [INIValue]
    public bool CanRetaliate;

    [INIValue]
    public bool CanCheckCollisionAhead;
#pragma warning restore 0649

    // Targeting
    [INIValue]
    public TargetType TargetType;

    [INIValue]
    public int HuntWeight;

    public readonly static AIData Default =
      new AIData
      {
        Attack_AngularDelta = 5f,
        Attack_HighAccuracyAngularDelta = 1f,
        Move_CloseEnough = 500,
        TargetType = TargetType.NULL,
        HuntWeight = 1,
        AIMinSpeed = -1,
        AIMaxSpeed = -1
      };
  }
}

