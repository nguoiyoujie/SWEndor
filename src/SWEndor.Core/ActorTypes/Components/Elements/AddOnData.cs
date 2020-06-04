using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;
using Primitives.FileFormat.INI;
using SWEndor.Models;
using SWEndor.Primitives.Extensions;
using Primrose.Primitives.ValueTypes;

namespace SWEndor.ActorTypes.Components
{
  internal struct AddOnData
  {
    private ActorTypeInfo _cache;

#pragma warning disable 0649 // values are filled by the attribute
    [INIValue]
    public readonly string Type;

    [INIValue]
    public readonly float3 Position;

    [INIValue]
    public readonly float3 Rotation;

    [INIValue]
    public readonly bool AttachToParent;
#pragma warning restore 0649

    public void Create(Engine engine, ActorInfo actor)
    {
      // cache
      if (_cache == null)
        _cache = engine.ActorTypeFactory.Get(Type);

      ActorCreationInfo acinfo = new ActorCreationInfo(_cache);
      acinfo.InitialState = ActorState.NORMAL;
      acinfo.Faction = actor.Faction;

      if (AttachToParent)
        acinfo.Position = Position.ToVec3();
      else
        acinfo.Position = actor.GetRelativePositionFUR(Position.x, Position.y, Position.z);

      acinfo.Rotation = new TV_3DVECTOR(Rotation.x, Rotation.y, Rotation.z);
      acinfo.CreationTime = actor.CreationTime;

      ActorInfo a = actor.ActorFactory.Create(acinfo);
      actor.AddChild(a);

      if (AttachToParent)
      {
        a.UseParentCoords = AttachToParent;
        a.JoinSquad(actor);
      }
    }
  }
}
