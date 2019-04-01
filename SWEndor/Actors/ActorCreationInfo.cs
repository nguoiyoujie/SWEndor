using MTV3D65;
using SWEndor.Actors;

namespace SWEndor.ActorTypes
{
  public class ActorCreationInfo
  {
    public float CreationTime;

    public FactionInfo Faction = FactionInfo.Neutral;
    public ActorTypeInfo ActorTypeInfo;
    public string Name;
    public CreationState CreationState = CreationState.PLANNED;
    public ActorState InitialState = ActorState.NORMAL;
    public TV_3DVECTOR InitialScale = new TV_3DVECTOR(1, 1, 1);
    public TV_3DVECTOR Position = new TV_3DVECTOR();
    public TV_3DVECTOR Rotation = new TV_3DVECTOR();

    public float InitialStrength;
    public float InitialSpeed;

    private ActorCreationInfo()
    { }

    public ActorCreationInfo(ActorTypeInfo at)
    {
      // Load defaults from actortype
      ActorTypeInfo = at;
      Name = at.Name;
      InitialStrength = at.MaxStrength;
      InitialSpeed = at.MaxSpeed;
    }
  }
}
