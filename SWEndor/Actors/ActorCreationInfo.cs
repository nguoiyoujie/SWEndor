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
    public CreationState CreationState;
    public ActorState InitialState;
    public TV_3DVECTOR InitialScale;
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
      CreationState = CreationState.PLANNED;
      CreationTime = 0;
      InitialState = ActorState.NORMAL;
      InitialScale = new TV_3DVECTOR(1, 1, 1);
      Position = new TV_3DVECTOR();
      Rotation = new TV_3DVECTOR();
    }
  }
}
