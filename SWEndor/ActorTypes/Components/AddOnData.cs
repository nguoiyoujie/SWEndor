using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.FileFormat.INI;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Components
{
  public struct AddOnData
  {
    public readonly string Type;
    private ActorTypeInfo _cache;
    public readonly TV_3DVECTOR Position;
    public readonly TV_3DVECTOR Rotation;
    public readonly bool AttachToParent;

    public AddOnData(string type, TV_3DVECTOR position, TV_3DVECTOR rotation, bool attachToParent)
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
        acinfo.Position = Position;
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

    public void LoadFromINI(INIFile f, string sectionname)
    {
      string type = f.GetStringValue(sectionname, "Type", Type);
      TV_3DVECTOR pos = f.GetTV_3DVECTOR(sectionname, "Position", Position);
      TV_3DVECTOR rot = f.GetTV_3DVECTOR(sectionname, "Rotation", Rotation);
      bool attach = f.GetBoolValue(sectionname, "AttachToParent", AttachToParent);
      this = new AddOnData(type, pos, rot, attach);
    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      f.SetStringValue(sectionname, "Type", Type);
      f.SetTV_3DVECTOR(sectionname, "Position", Position);
      f.SetTV_3DVECTOR(sectionname, "Rotation", Rotation);
      f.SetBoolValue(sectionname, "AttachToParent", AttachToParent);
    }
  }
}
