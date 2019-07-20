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
      acinfo.CreationTime = actor.StateModel.CreationTime;
      acinfo.Faction = actor.Faction;

      //float scale = actor.CoordData.Scale;
      if (AttachToParent)
        acinfo.Position = Position;
      else
        acinfo.Position = actor.GetRelativePositionFUR(Position.x, Position.y, Position.z);

      //acinfo.InitialScale = scale;
      acinfo.Rotation = new TV_3DVECTOR(Rotation.x, Rotation.y, Rotation.z);

      ActorInfo a = actor.ActorFactory.Create(acinfo);
      actor.AddChild(a);
      
      a.Relation.UseParentCoords = AttachToParent;
    }
  }
}
