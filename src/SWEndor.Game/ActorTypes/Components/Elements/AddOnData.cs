using MTV3D65;
using SWEndor.Game.Actors;
using SWEndor.Game.Core;
using Primrose.FileFormat.INI;
using SWEndor.Game.Models;
using SWEndor.Game.Primitives.Extensions;
using Primrose.Primitives.ValueTypes;

namespace SWEndor.Game.ActorTypes.Components
{
  internal struct AddOnData
  {
    private ActorTypeInfo _cache;

#pragma warning disable 0649 // values are filled by the attribute
    [INIValue]
    public readonly string Type;

    [INIValue]
    public readonly float? Scale;

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

      if (Scale.HasValue)
      {
        acinfo.InitialScale = Scale.Value;
      }

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
