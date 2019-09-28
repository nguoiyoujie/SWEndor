﻿using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.ExplosionTypes;
using SWEndor.Primitives;

namespace SWEndor.Explosions.Models
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

    public void Init(ExplosionTypeInfo type, ExplosionCreationInfo acinfo)
    {
      Position = acinfo.Position;
      Rotation = acinfo.Rotation;
      PrevPosition = Position;
      PrevRotation = Rotation;
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
      private set { prevData.Position = value; }
    }

    public TV_3DVECTOR Direction
    {
      get { return currData.Direction; }
      set { prevData.Direction = currData.Direction; currData.Direction = value; }
    }

    public TV_3DVECTOR PrevDirection
    {
      get { return prevData.Direction; }
      private set { prevData.Direction = value; }
    }

    public TV_3DVECTOR Rotation
    {
      get { return currData.Rotation; }
      set { prevData.Rotation = currData.Rotation; currData.Rotation = value; }
    }

    public TV_3DVECTOR PrevRotation
    {
      get { return prevData.Rotation; }
      private set { prevData.Rotation = value; }
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

    public TV_3DMATRIX GetWorldMatrix(ActorInfo.Factory<ActorInfo> f, ExplosionInfo self, float time)
    {
      if (time > currTime)
      {
        currMat = InnerGetWorldMatrix(f, self, time);
        currTime = time;
      }
      return currMat;
    }

    public TV_3DMATRIX GetPrevWorldMatrix(ActorInfo.Factory<ActorInfo> f, ExplosionInfo self, float time)
    {
      if (time > prevTime)
      {
        prevMat = InnerGetPrevWorldMatrix(f, self, time);
        prevTime = time;
      }
      return prevMat;
    }

    private TV_3DMATRIX InnerGetWorldMatrix(ActorInfo.Factory<ActorInfo> f, ExplosionInfo a, float time)
    {
      using (ScopeCounterManager.Acquire(a.Scope))
      {
        ActorInfo p = f.Get(a.AttachedActorID);
        if (!p?.Disposed ?? false)
        {
          using (ScopeCounterManager.Acquire(p.Scope))
          {
            TV_3DMATRIX m = new TV_3DMATRIX();
            mlib.TVMatrixMultiply(ref m, GetMat(ref currData), p.GetWorldMatrix());
            return m;
          }
        }
      }
      return GetMat(ref currData);
    }

    private TV_3DMATRIX InnerGetPrevWorldMatrix(ActorInfo.Factory<ActorInfo> f, ExplosionInfo a, float time)
    {
      using (ScopeCounterManager.Acquire(a.Scope))
      {
        ActorInfo p = f.Get(a.AttachedActorID);
        if (!p?.Disposed ?? false)
        {
          using (ScopeCounterManager.Acquire(p.Scope))
          {
            TV_3DMATRIX m = new TV_3DMATRIX();
            mlib.TVMatrixMultiply(ref m, GetMat(ref prevData), p.GetPrevWorldMatrix());
            return m;
          }
        }
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

    public TV_3DVECTOR GetGlobalPosition(ActorInfo.Factory<ActorInfo> f, ExplosionInfo self, float time)
    {
      ActorInfo p = f.Get(self.AttachedActorID);
      if (!p?.Disposed ?? false)
      {
        TV_3DQUATERNION qR = new TV_3DQUATERNION();
        TV_3DVECTOR vT = new TV_3DVECTOR();
        TV_3DVECTOR vS = new TV_3DVECTOR();
        mlib.TVMatrixDecompose(ref vS, ref qR, ref vT, GetWorldMatrix(f, self, time));
        return vT;
      }
      else
        return currData.Position;
    }

    public TV_3DVECTOR GetPrevGlobalPosition(ActorInfo.Factory<ActorInfo> f, ExplosionInfo self, float time)
    {
      ActorInfo p = f.Get(self.AttachedActorID);
      if (!p?.Disposed ?? false)
      {
        TV_3DQUATERNION qR = new TV_3DQUATERNION();
        TV_3DVECTOR vT = new TV_3DVECTOR();
        TV_3DVECTOR vS = new TV_3DVECTOR();
        mlib.TVMatrixDecompose(ref vS, ref qR, ref vT, GetPrevWorldMatrix(f, self, time));
        return vT;
      }
      else
        return prevData.Position;
    }

    public TV_3DVECTOR GetGlobalRotation(ActorInfo.Factory<ActorInfo> f, ExplosionInfo self, float time) // broken
    {
      ActorInfo p = f.Get(self.AttachedActorID);
      if (!p?.Disposed ?? false)
      {
        TV_3DVECTOR dir = new TV_3DVECTOR();
        mlib.TVVec3TransformNormal(ref dir, new TV_3DVECTOR(0, 0, 100), GetWorldMatrix(f, self, time));
        return Utilities.GetRotation(dir);
      }
      else
        return currData.Rotation;
    }

    public TV_3DVECTOR GetPrevGlobalRotation(ActorInfo.Factory<ActorInfo> f, ExplosionInfo self, float time) // broken
    {
      ActorInfo p = f.Get(self.AttachedActorID);
      if (!p?.Disposed ?? false)
      {
        TV_3DVECTOR dir = new TV_3DVECTOR();
        mlib.TVVec3TransformNormal(ref dir, new TV_3DVECTOR(0, 0, 100), GetPrevWorldMatrix(f, self, time));
        return Utilities.GetRotation(dir);
      }
      else
        return currData.Rotation;
    }

    // ?
    public TV_3DVECTOR GetGlobalDirection(ActorInfo.Factory<ActorInfo> f, ExplosionInfo self, float time)
    {
      TV_3DVECTOR dir = Utilities.GetDirection(GetGlobalRotation(f, self, time));
      return dir;
    }

    public TV_3DVECTOR GetPrevGlobalDirection(ActorInfo.Factory<ActorInfo> f, ExplosionInfo self, float time)
    {
      TV_3DVECTOR dir = Utilities.GetDirection(GetPrevGlobalRotation(f, self, time));
      return dir;
    }

    public TV_3DVECTOR GetRelativePositionFUR(ActorInfo.Factory<ActorInfo> f, ExplosionInfo self, float time, float forward, float up = 0, float right = 0, bool local = false)
    {
      TV_3DVECTOR pos = local ? Position : GetGlobalPosition(f, self, time);
      TV_3DVECTOR rot = local ? Rotation : GetGlobalRotation(f, self, time);
      TV_3DVECTOR ret = new TV_3DVECTOR();

      mlib.TVVec3Rotate(ref ret, new TV_3DVECTOR(right, up, forward), rot.y, rot.x, rot.z);
      ret += pos;
      return ret;
    }

    public TV_3DVECTOR GetRelativePositionXYZ(ActorInfo.Factory<ActorInfo> f, ExplosionInfo self, float time, float x, float y, float z, bool local = false)
    {
      TV_3DVECTOR pos = local ? Position : GetGlobalPosition(f, self, time);
      TV_3DVECTOR rot = local ? Rotation : GetGlobalRotation(f, self, time);
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

namespace SWEndor.Explosions
{
  public partial class ExplosionInfo
  {
    public TV_3DVECTOR Position { get { return Transform.Position; } set { Transform.Position = value; } }
    public TV_3DVECTOR PrevPosition { get { return Transform.PrevPosition; } }
    public TV_3DVECTOR Rotation { get { return Transform.Rotation; } set { Transform.Rotation = value; } }
    public TV_3DVECTOR PrevRotation { get { return Transform.PrevRotation; } }
    public TV_3DVECTOR Direction { get { return Transform.Direction; } set { Transform.Direction = value; } }
    public float Scale { get { return Transform.Scale; } set { Transform.Scale = value; } }

    public TV_3DMATRIX GetWorldMatrix()
    {
      using (ScopeCounterManager.Acquire(Scope))
        return Transform.GetWorldMatrix(Engine.ActorFactory, this, Game.GameTime);
    }

    public TV_3DMATRIX GetPrevWorldMatrix()
    {
      using (ScopeCounterManager.Acquire(Scope))
        return Transform.GetPrevWorldMatrix(Engine.ActorFactory, this, Game.GameTime);
    }

    public TV_3DVECTOR GetGlobalPosition()
    {
      using (ScopeCounterManager.Acquire(Scope))
        return Transform.GetGlobalPosition(Engine.ActorFactory, this, Game.GameTime);
    }

    public TV_3DVECTOR GetPrevGlobalPosition()
    {
      using (ScopeCounterManager.Acquire(Scope))
        return Transform.GetPrevGlobalPosition(Engine.ActorFactory, this, Game.GameTime);
    }

    public TV_3DVECTOR GetGlobalRotation()
    {
      using (ScopeCounterManager.Acquire(Scope))
        return Transform.GetGlobalRotation(Engine.ActorFactory, this, Game.GameTime);
    }

    public TV_3DVECTOR GetGlobalDirection()
    {
      using (ScopeCounterManager.Acquire(Scope))
        return Transform.GetGlobalDirection(Engine.ActorFactory, this, Game.GameTime);
    }

    public TV_3DVECTOR GetRelativePositionFUR(float front, float up, float right, bool uselocal = false)
    {
      using (ScopeCounterManager.Acquire(Scope))
        return Transform.GetRelativePositionFUR(Engine.ActorFactory, this, Game.GameTime, front, up, right, uselocal);
    }

    public TV_3DVECTOR GetRelativePositionXYZ(float x, float y, float z, bool uselocal = false)
    {
      using (ScopeCounterManager.Acquire(Scope))
        return Transform.GetRelativePositionXYZ(Engine.ActorFactory, this, Game.GameTime, x, y, z, uselocal);
    }

    public void MoveRelative(float forward, float up = 0, float right = 0)
    {
      TV_3DVECTOR vec = GetRelativePositionFUR(forward, up, right, true);
      Transform.Position = new TV_3DVECTOR(vec.x, vec.y, vec.z);
    }

    public void MoveAbsolute(float x, float y, float z)
    {
      TV_3DVECTOR vec = Transform.Position + new TV_3DVECTOR(x, y, z);
      Transform.Position = new TV_3DVECTOR(vec.x, vec.y, vec.z);
    }

    public void LookAt(TV_3DVECTOR point) { Transform.LookAt(point); }
  }
}
