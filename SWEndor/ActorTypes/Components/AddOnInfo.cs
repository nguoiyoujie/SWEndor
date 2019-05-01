using MTV3D65;
using SWEndor.Actors;

namespace SWEndor.ActorTypes.Components
{
  public struct AddOnInfo
  {
    public readonly string Type;
    private ActorTypeInfo _cache;
    public readonly TV_3DVECTOR Position;
    public readonly TV_3DVECTOR Rotation;
    public readonly bool AttachToParent;

    public AddOnInfo(string type, TV_3DVECTOR position, TV_3DVECTOR rotation, bool attachToParent)
    {
      Type = type;
      _cache = null;
      Position = position;
      Rotation = rotation;
      AttachToParent = attachToParent;
    }

    public void Create(Engine engine, ActorInfo actor)
    {
      // cache
      if (_cache == null)
        _cache = engine.ActorTypeFactory.Get(Type);

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

      ActorInfo a = ActorInfo.Create(actor.ActorFactory, acinfo);
      actor.AddChild(a.ID);
      
      a.AttachToParent = AttachToParent;
    }
  }
}
