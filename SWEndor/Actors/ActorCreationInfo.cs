using MTV3D65;
using SWEndor.Actors;

namespace SWEndor.ActorTypes
{
  public struct ActorCreationInfo
  {
    public FactionInfo Faction;
    public ActorTypeInfo ActorTypeInfo;
    public string Name;
    public float CreationTime;
    public ActorState InitialState;
    public float InitialScale;
    public TV_3DVECTOR Position;
    public TV_3DVECTOR Rotation;

    public float InitialStrength;
    public float InitialSpeed;

    public ActorCreationInfo(ActorTypeInfo at)
    {
      // Load defaults from actortype
      ActorTypeInfo = at;
      Name = at.Name;
      InitialStrength = at.MaxStrength;
      InitialSpeed = at.MaxSpeed;

      Faction = FactionInfo.Neutral;
      CreationTime = 0;
      InitialState = ActorState.NORMAL;
      InitialScale = 1;
      Position = new TV_3DVECTOR();
      Rotation = new TV_3DVECTOR();
    }
  }
}
