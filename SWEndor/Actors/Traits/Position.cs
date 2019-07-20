/*
using MTV3D65;
using SWEndor.Primitives.Traits;

namespace SWEndor.Actors.Traits
{
public class Position
{
  private TV_3DVECTOR l_pos;
  private TV_3DVECTOR l_rot;

  public TV_3DVECTOR GetPosition<A>(A self, bool local = true)
    where A : ITraitOwner
  {
    if (local)
      return l_pos;

    Relation rel = self.Trait<Relation>();
    if (rel != null)
    {
      if (rel.Parent != null && rel.Attached)
      {
        return (rel.Parent.Trait<Position>().GetRelative(rel.Parent, l_pos, local));
      }
    }

    return l_pos;
  }

  public TV_3DVECTOR GetRotation<A>(A self, bool local = true)
    where A : ITraitOwner
  {
    if (local)
      return l_rot;

    return l_rot;
  }

  public void SetPosition<A>(A self, TV_3DVECTOR value, bool local = true)
    where A : ITraitOwner
  {
    if (local)
    {
      l_pos = value;
      return;
    }
    l_pos = value;
  }

  public void SetRotation<A>(A self, TV_3DVECTOR value, bool local = true)
    where A : ITraitOwner
  {
    if (local)
    {
      l_rot = value;
      return;
    }
    l_rot = value;
  }

  public Position Move<A>(A self, TV_3DVECTOR fur, bool local = true)
     where A : ITraitOwner
  {
    SetPosition(self, GetRelative(self, fur, local), local);
    return this;
  }

  public Position MoveAbsolute<A>(A self, TV_3DVECTOR xyz, bool local = true)
    where A : ITraitOwner
  {
    SetPosition(self, GetPosition(self, local) + xyz, local);
    return this;
  }

  TV_3DVECTOR FURtoXYZ(TV_3DVECTOR fur)
  {
    return new TV_3DVECTOR(fur.z, fur.y, fur.x);
  }

  public TV_3DVECTOR GetRelative<A>(A self, TV_3DVECTOR xyz, bool local = true)
    where A : ITraitOwner
  {
    TV_3DVECTOR pos = GetPosition(self, local);
    TV_3DVECTOR rot = GetRotation(self, local);
    TV_3DVECTOR ret = new TV_3DVECTOR();

    Globals.Engine.TrueVision.TVMathLibrary.TVVec3Rotate(ref ret, xyz, rot.y, rot.x, rot.z);
    ret += pos;
    return ret;
  }
}
}
*/
