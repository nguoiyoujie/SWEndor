using MTV3D65;
using SWEndor.ActorTypes;

namespace SWEndor.Actors
{
  public partial class ActorInfo
  {
    public struct TransformModel
    {
      private static TVMathLibrary mlib { get { return Globals.Engine.TrueVision.TVMathLibrary; } }

      TransformData currData;
      TransformData prevData;
      float currTime;
      float prevTime;
      TV_3DMATRIX currMat;
      TV_3DMATRIX prevMat;

      public void Init(ActorTypeInfo type, ActorCreationInfo acinfo)
      {
        Position = acinfo.Position;
        Rotation = acinfo.Rotation;
        Scale = type.Scale * acinfo.InitialScale;
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

      public TV_3DMATRIX GetWorldMatrix(ActorInfo self, float time)
      {
        if (time > currTime)
        {
          currMat = InnerGetWorldMatrix(self, time);
          currTime = time;
        }
        return currMat;
      }

      public TV_3DMATRIX GetPrevWorldMatrix(ActorInfo self, float time)
      {
        if (time > prevTime)
        {
          prevMat = InnerGetPrevWorldMatrix(self, time);
          prevTime = time;
        }
        return prevMat;
      }

      public TV_3DMATRIX InnerGetWorldMatrix(ActorInfo self, float time)
      {
        if (self.Relation.Parent != null && !self.Relation.Parent.Disposed && self.Relation.UseParentCoords)
        {
          TV_3DMATRIX m = new TV_3DMATRIX();
          mlib.TVMatrixMultiply(ref m, GetMat(ref currData), self.Relation.Parent.Transform.GetWorldMatrix(self.Relation.Parent, time));
          return m;
        }
        return GetMat(ref currData);
      }

      public TV_3DMATRIX InnerGetPrevWorldMatrix(ActorInfo self, float time)
      {
        if (self.Relation.Parent != null && !self.Relation.Parent.Disposed && self.Relation.UseParentCoords)
        {
          TV_3DMATRIX m = new TV_3DMATRIX();
          mlib.TVMatrixMultiply(ref m, GetMat(ref prevData), self.Relation.Parent.Transform.GetPrevWorldMatrix(self.Relation.Parent, time));
          return m;
        }

        return GetMat(ref prevData);
      }

      private TV_3DMATRIX GetMat(ref TransformData data)
      {
        TV_3DMATRIX mSR = new TV_3DMATRIX();
        TV_3DMATRIX mSRT = new TV_3DMATRIX();
        mlib.TVMatrixMultiply(ref mSR, GetMatS(ref currData), GetMatR(ref currData));
        mlib.TVMatrixMultiply(ref mSRT, mSR, GetMatT(ref currData));
        return mSRT;
      }

      public TV_3DVECTOR GetGlobalPosition(ActorInfo self, float time)
      {
        if (self.Relation.Parent != null && !self.Relation.Parent.Disposed && self.Relation.UseParentCoords)
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

      public TV_3DVECTOR GetPrevGlobalPosition(ActorInfo self, float time)
      {
        if (self.Relation.Parent != null && !self.Relation.Parent.Disposed && self.Relation.UseParentCoords)
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

      public TV_3DVECTOR GetGlobalRotation(ActorInfo self, float time) // broken
      {
        if (self.Relation.Parent != null && !self.Relation.Parent.Disposed && self.Relation.UseParentCoords)
        {
          TV_3DVECTOR dir = new TV_3DVECTOR();
          mlib.TVVec3TransformNormal(ref dir, new TV_3DVECTOR(0, 0, 100), GetWorldMatrix(self, time));
          return Utilities.GetRotation(dir);
        }
        else
          return currData.Rotation;
      }

      public TV_3DVECTOR GetPrevGlobalRotation(ActorInfo self, float time) // broken
      {
        if (self.Relation.Parent != null && !self.Relation.Parent.Disposed && self.Relation.UseParentCoords)
        {
          TV_3DVECTOR dir = new TV_3DVECTOR();
          mlib.TVVec3TransformNormal(ref dir, new TV_3DVECTOR(0, 0, 100), GetPrevWorldMatrix(self, time));
          return Utilities.GetRotation(dir);
        }
        else
          return currData.Rotation;
      }

      // ?
      public TV_3DVECTOR GetGlobalDirection(ActorInfo self)
      {
        TV_3DVECTOR ret = Direction;
        if (self.Relation.Parent != null && !self.Relation.Parent.Disposed && self.Relation.UseParentCoords)
          ret += self.Relation.Parent.Transform.GetGlobalDirection(self.Relation.Parent);

        TV_3DVECTOR dir = new TV_3DVECTOR();
        mlib.TVVec3Normalize(ref dir, ret);
        return dir;
      }

      public TV_3DVECTOR GetPrevGlobalDirection(ActorInfo self)
      {
        TV_3DVECTOR ret = PrevDirection;
        if (self.Relation.Parent != null && !self.Relation.Parent.Disposed && self.Relation.UseParentCoords)
          ret += self.Relation.Parent.Transform.GetPrevGlobalDirection(self.Relation.Parent);

        TV_3DVECTOR dir = new TV_3DVECTOR();
        mlib.TVVec3Normalize(ref dir, ret);
        return dir;
      }

      public TV_3DVECTOR GetRelativePositionFUR(ActorInfo self, float time, float forward, float up = 0, float right = 0, bool local = false)
      {
        TV_3DVECTOR pos = local ? Position : GetGlobalPosition(self, time);
        TV_3DVECTOR rot = local ? Rotation : GetGlobalRotation(self, time);
        TV_3DVECTOR ret = new TV_3DVECTOR();

        mlib.TVVec3Rotate(ref ret, new TV_3DVECTOR(right, up, forward), rot.y, rot.x, rot.z);
        ret += pos;
        return ret;
      }

      public TV_3DVECTOR GetRelativePositionXYZ(ActorInfo self, float time, float x, float y, float z, bool local = false)
      {
        TV_3DVECTOR pos = local ? Position : GetGlobalPosition(self, time);
        TV_3DVECTOR rot = local ? Rotation : GetGlobalRotation(self, time);
        TV_3DVECTOR ret = new TV_3DVECTOR();

        mlib.TVVec3Rotate(ref ret, new TV_3DVECTOR(x, y, z), rot.y, rot.x, rot.z);
        ret += pos;
        return ret;
      }

      public void LookAt(TV_3DVECTOR target, bool preserveZrotation = false)
      {
        float zrot = currData.Roll;
        TV_3DVECTOR dir = target - Position;
        Direction = dir;

        if (preserveZrotation)
          currData.Roll = zrot;
      }
    }
  }
}

