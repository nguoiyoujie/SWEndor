using MTV3D65;

namespace SWEndor.Actors
{
  public class AddOnInfo
  {
    public string Type;
    private ActorTypeInfo _cache;
    public TV_3DVECTOR Position;
    public TV_3DVECTOR Rotation;
    public bool AttachToParent;

    public AddOnInfo(string type, TV_3DVECTOR position, TV_3DVECTOR rotation, bool attachToParent)
    {
      Type = type;
      Position = position;
      Rotation = rotation;
      AttachToParent = attachToParent;
    }

    public void Create(ActorInfo actor)
    {
      // cache
      if (_cache == null)
        _cache = ActorTypeFactory.Instance().GetActorType(Type);

      ActorCreationInfo acinfo = new ActorCreationInfo(_cache);
      acinfo.InitialState = ActorState.NORMAL;
      acinfo.Faction = actor.Faction;
      if (AttachToParent)
        acinfo.Position = new TV_3DVECTOR(Position.x * actor.Scale.x, Position.y * actor.Scale.y, Position.z * actor.Scale.z);
      else
        acinfo.Position = actor.GetRelativePositionFUR(Position.x * actor.Scale.x, Position.y * actor.Scale.y, Position.z * actor.Scale.z);

      acinfo.InitialScale = actor.Scale;
      acinfo.Rotation = new TV_3DVECTOR(Rotation.x, Rotation.y, Rotation.z);
      acinfo.CreationTime = actor.CreationTime;

      ActorInfo a = ActorInfo.Create(acinfo);
      a.AddParent(actor);

      if (AttachToParent)
        a.AttachToMesh = actor.ID;
    }
  }
}
