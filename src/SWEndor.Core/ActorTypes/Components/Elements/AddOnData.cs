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
    private const string sNone = "";

    [INIValue(sNone, "Type")]
    public readonly string Type;

    private ActorTypeInfo _cache;

    [INIValue(sNone, "Position")]
    public readonly float3 Position;

    [INIValue(sNone, "Rotation")]
    public readonly float3 Rotation;

    [INIValue(sNone, "AttachToParent")]
    public readonly bool AttachToParent;

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
