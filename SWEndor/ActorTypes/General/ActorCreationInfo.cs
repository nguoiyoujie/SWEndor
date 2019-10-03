using System;
using MTV3D65;
using SWEndor.Actors;
using SWEndor.Models;

namespace SWEndor.ActorTypes
{
  public struct ActorCreationInfo : ICreationInfo<ActorInfo, ActorTypeInfo>
  {
    public FactionInfo Faction;
    public ActorTypeInfo TypeInfo { get; }
    public string Name;
    public float CreationTime;
    public ActorState InitialState;
    public float InitialScale;
    public TV_3DVECTOR Position;
    public TV_3DVECTOR Rotation;

    public float InitialStrength;
    public float InitialSpeed;
    public bool FreeSpeed;

    public ActorCreationInfo(ActorTypeInfo at)
    {
      // Load defaults from actortype
      TypeInfo = at;
      Name = at.Name;
      InitialStrength = at.CombatData.MaxStrength;
      InitialSpeed = at.MoveLimitData.MaxSpeed;

      Faction = FactionInfo.Neutral;
      CreationTime = 0;
      InitialState = ActorState.NORMAL;
      InitialScale = 1;
      Position = new TV_3DVECTOR();
      Rotation = new TV_3DVECTOR();
      FreeSpeed = false;
    }
  }
}
