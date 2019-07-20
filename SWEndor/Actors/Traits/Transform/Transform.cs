using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.Primitives;
using SWEndor.Primitives.Traits;

namespace SWEndor.Actors.Traits
{
  public struct TransformData
  {
    public float Scale;
    public float Pitch;
    public float Yaw;
    public float Roll;
    public float X;
    public float Y;
    public float Z;

    public TV_3DVECTOR Position
    {
      get { return new TV_3DVECTOR(X, Y, Z); }
      set { X = value.x; Y = value.y; Z = value.z; }
    }

    public TV_3DVECTOR Rotation
    {
      get { return new TV_3DVECTOR(Pitch, Yaw, Roll); }
      set { Pitch = value.x; Yaw = value.y; Roll = value.z; }
    }

    public TV_3DVECTOR Direction
    {
      get { return Utilities.GetDirection(new TV_3DVECTOR(Pitch, Yaw, Roll)); }
      set
      {
        TV_3DVECTOR r = Utilities.GetRotation(value);
        Pitch = r.x; Yaw = r.y; Roll = r.z;
      }
    }
  }

  public class Transform : ITransform
  {
    private static TVMathLibrary mlib { get { return Globals.Engine.TrueVision.TVMathLibrary; } }

    TransformData currData;
    TransformData prevData;
    TimeCache<TV_3DMATRIX> currWMat;
    TimeCache<TV_3DMATRIX> prevWMat;

    public void Init(ActorTypeInfo type, ActorCreationInfo acinfo)
    {
      Position = acinfo.Position;
      Rotation = acinfo.Rotation;
      Scale = type.Scale * acinfo.InitialScale;
      currWMat = new TimeCache<TV_3DMATRIX>(0, () => { return new TV_3DMATRIX(); });
      prevWMat = new TimeCache<TV_3DMATRIX>(0, () => { return new TV_3DMATRIX(); });
    }

    public float Scale
    {
      get { return currData.Scale; }
      set { prevData.Scale = currData.Scale; currData.Scale = value; }
    }

    public TV_3DVECTOR Position
    {
      get { return currData.Position; }
      set { prevData.Position = currData.Position; currData.Position = value; }
    }

    public TV_3DVECTOR PrevPosition
    {
      get { return prevData.Position; }
    }

    public TV_3DVECTOR Direction
    {
      get { return currData.Direction; }
      set { prevData.Direction = currData.Direction; currData.Direction = value; }
    }

    public TV_3DVECTOR PrevDirection
    {
      get { return prevData.Direction; }
    }

    public TV_3DVECTOR Rotation
    {
      get { return currData.Rotation; }
      set { prevData.Rotation = currData.Rotation; currData.Rotation = value; }
    }

    public TV_3DVECTOR PrevRotation
    {
      get { return prevData.Rotation; }
    }

    public TV_3DMATRIX MatrixRotation { get { return GetMatR(ref currData); } }
    public TV_3DMATRIX PrevMatrixRotation { get { return GetMatR(ref prevData); } }
    private TV_3DMATRIX GetMatR(ref TransformData data)
    {
      TV_3DMATRIX matrix = new TV_3DMATRIX();
      mlib.TVMatrixRotationYawPitchRoll(ref matrix, data.Yaw, data.Pitch, data.Roll);
      return matrix;
    }

    public TV_3DMATRIX MatrixTranslation { get { return GetMatT(ref currData); } }
    public TV_3DMATRIX PrevMatrixTranslation { get { return GetMatT(ref prevData); } }
    private TV_3DMATRIX GetMatT(ref TransformData data)
    {
      TV_3DMATRIX matrix = new TV_3DMATRIX();
      mlib.TVMatrixTranslation(ref matrix, data.X, data.Y, data.Z);
      return matrix;
    }

    public TV_3DMATRIX MatrixScale { get { return GetMatS(ref currData); } }
    public TV_3DMATRIX PrevMatrixScale { get { return GetMatS(ref prevData); } }
    private TV_3DMATRIX GetMatS(ref TransformData data)
    {
      TV_3DMATRIX matrix = new TV_3DMATRIX();
      mlib.TVMatrixScaling(ref matrix, data.Scale, data.Scale, data.Scale);
      return matrix;
    }

    public TV_3DMATRIX GetWorldMatrix<A>(A self, float time) where A : ITraitOwner
    {
      return currWMat.Get(time, InnerGetWorldMatrix, self, time);
    }

    public TV_3DMATRIX GetPrevWorldMatrix<A>(A self, float time) where A : ITraitOwner
    {
      return prevWMat.Get(time, InnerGetPrevWorldMatrix, self, time);
    }

    public TV_3DMATRIX InnerGetWorldMatrix<A>(A self, float time) where A : ITraitOwner
    {
      Relation<A> rel;
      if (self.TryGetTrait(out rel) && rel.Parent != null && !rel.Parent.Disposed && rel.UseParentCoords)
        return GetMatS(ref currData) * GetMatR(ref currData) * GetMatT(ref currData) * rel.Parent.Trait<Transform>().GetWorldMatrix(rel.Parent, time);

      return GetMatS(ref currData) * GetMatR(ref currData) * GetMatT(ref currData);
    }

    public TV_3DMATRIX InnerGetPrevWorldMatrix<A>(A self, float time) where A : ITraitOwner
    {
      Relation<A> rel;
      if (self.TryGetTrait(out rel) && rel.Parent != null && !rel.Parent.Disposed && rel.UseParentCoords)
        return GetMatS(ref currData) * GetMatR(ref prevData) * GetMatT(ref prevData) * rel.Parent.Trait<Transform>().GetPrevWorldMatrix(rel.Parent, time);

      return GetMatS(ref prevData) * GetMatR(ref prevData) * GetMatT(ref prevData);
    }

    public TV_3DVECTOR GetGlobalPosition<A>(A self, float time) where A : ITraitOwner
    {
      Relation<A> rel;
      if (self.TryGetTrait(out rel) && rel.Parent != null && !rel.Parent.Disposed && rel.UseParentCoords)
      {
        TV_3DQUATERNION qR = new TV_3DQUATERNION();
        TV_3DVECTOR vT = new TV_3DVECTOR();
        TV_3DVECTOR vS = new TV_3DVECTOR();
        mlib.TVMatrixDecompose(ref vS, ref qR, ref vT, GetWorldMatrix(self, time));
        return vT;
      }
      else
        return currData.Position;
    }

    public TV_3DVECTOR GetPrevGlobalPosition<A>(A self, float time) where A : ITraitOwner
    {
      Relation<A> rel;
      if (self.TryGetTrait(out rel) && rel.Parent != null && !rel.Parent.Disposed && rel.UseParentCoords)
      {
        TV_3DQUATERNION qR = new TV_3DQUATERNION();
        TV_3DVECTOR vT = new TV_3DVECTOR();
        TV_3DVECTOR vS = new TV_3DVECTOR();
        mlib.TVMatrixDecompose(ref vS, ref qR, ref vT, GetPrevWorldMatrix(self, time));
        return vT;
      }
      else
        return prevData.Position;
    }

    public TV_3DVECTOR GetGlobalRotation<A>(A self)
       where A : ITraitOwner
    {
      Relation<A> rel;
      if (self.TryGetTrait(out rel) && rel.Parent != null && !rel.Parent.Disposed && rel.UseParentCoords)
      {
        TV_3DVECTOR dir = rel.Parent.Trait<ITransform>().Direction;
        TV_3DVECTOR rdir = new TV_3DVECTOR();
        mlib.TVVec3TransformCoord(ref rdir, dir, GetMatR(ref currData));
        return Utilities.GetRotation(rdir);
      }
      else
        return currData.Rotation;
    }

    public TV_3DVECTOR GetPrevGlobalRotation<A>(A self)
      where A : ITraitOwner
    {
      Relation<A> rel;
      if (self.TryGetTrait(out rel) && rel.Parent != null && !rel.Parent.Disposed && rel.UseParentCoords)
      {
        TV_3DVECTOR dir = rel.Parent.Trait<ITransform>().PrevDirection;
        TV_3DVECTOR rdir = new TV_3DVECTOR();
        mlib.TVVec3TransformCoord(ref rdir, dir, GetMatR(ref prevData));
        return Utilities.GetRotation(rdir);
      }
      else
        return currData.Rotation;
    }

    // ?
    public TV_3DVECTOR GetGlobalDirection<A>(A self)
       where A : ITraitOwner
    {
      TV_3DVECTOR ret = Direction;

      Relation<A> rel = self.Trait<Relation<A>>();
      if (rel != null && rel.Parent != null && !rel.Parent.Disposed && rel.UseParentCoords)
          ret += rel.Parent.Trait<Transform>().GetGlobalDirection(rel.Parent);

      TV_3DVECTOR dir = new TV_3DVECTOR();
      mlib.TVVec3Normalize(ref dir, ret);
      return dir;
    }

    public TV_3DVECTOR GetPrevGlobalDirection<A>(A self)
      where A : ITraitOwner
    {
      TV_3DVECTOR ret = PrevDirection;

      Relation<A> rel = self.Trait<Relation<A>>();
      if (rel != null && rel.Parent != null && !rel.Parent.Disposed && rel.UseParentCoords)
        ret += rel.Parent.Trait<Transform>().GetPrevGlobalDirection(rel.Parent);

      TV_3DVECTOR dir = new TV_3DVECTOR();
      mlib.TVVec3Normalize(ref dir, ret);
      return dir;
    }

    public TV_3DVECTOR GetRelativePositionFUR<A>(A self, float time, float forward, float up = 0, float right = 0, bool local = false)
       where A : ITraitOwner
    {
      TV_3DVECTOR pos = local ? Position : GetGlobalPosition(self, time);
      TV_3DVECTOR rot = local ? Rotation : GetGlobalRotation(self);
      TV_3DVECTOR ret = new TV_3DVECTOR();

      mlib.TVVec3Rotate(ref ret, new TV_3DVECTOR(right, up, forward), rot.y, rot.x, rot.z);
      ret += pos;
      return ret;
    }

    public TV_3DVECTOR GetRelativePositionXYZ<A>(A self, float time, float x, float y, float z, bool local = false) 
      where A : ITraitOwner
    {
      TV_3DVECTOR pos = local ? Position : GetGlobalPosition(self, time);
      TV_3DVECTOR rot = local ? Rotation : GetGlobalRotation(self);
      TV_3DVECTOR ret = new TV_3DVECTOR();

      mlib.TVVec3Rotate(ref ret, new TV_3DVECTOR(x, y, z), rot.y, rot.x, rot.z);
      ret += pos;
      return ret;
    }

    public ITransform LookAt(TV_3DVECTOR target, bool preserveZrotation = false)
    {
      float zrot = currData.Roll;
      TV_3DVECTOR dir = target - Position;
      Direction = dir;

      if (preserveZrotation)
        currData.Roll = zrot;

      return this;
    }
  }
}
