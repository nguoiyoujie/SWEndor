using MTV3D65;
using SWEndor.Game.ActorTypes;
using SWEndor.Game.ExplosionTypes;
using Primrose.Primitives;
using SWEndor.Game.ProjectileTypes;
using SWEndor.Game.Primitives.Extensions;
using SWEndor.Game.Core;
using Primrose.Primitives.ValueTypes;

namespace SWEndor.Game.Models
{
  public struct TransformModel<T, U> 
    where T :
    IEngineObject, 
    IScoped, 
    IParent<U>
    where U :
    IScoped,
    IActorDisposable,
    ITransformable
  {
    TVMathLibrary math; //{ get { return Globals.Engine.TrueVision.TVMathLibrary; } }

    TransformData currData;
    TransformData prevData;
    float currTime;
    float prevTime;
    TV_3DMATRIX currMat;
    TV_3DMATRIX prevMat;

    public void Init(Engine engine, float3 scale, ActorCreationInfo acinfo)
    {
      Position = acinfo.Position;
      Rotation = acinfo.Rotation;
      PrevPosition = Position;
      PrevRotation = Rotation;
      ZSqueeze = 1;
      Scale = scale * acinfo.InitialScale;
      math = engine.TrueVision.TVMathLibrary;
    }

    public void Init(Engine engine, float3 scale, ExplosionCreationInfo acinfo)
    {
      Position = acinfo.Position;
      Rotation = acinfo.Rotation;
      PrevPosition = Position;
      PrevRotation = Rotation;
      ZSqueeze = 1;
      Scale = scale * acinfo.InitialScale;
      math = engine.TrueVision.TVMathLibrary;
    }

    public void Init(Engine engine, float3 scale, ProjectileCreationInfo acinfo)
    {
      Position = acinfo.Position;
      Rotation = acinfo.Rotation;
      PrevPosition = Position;
      PrevRotation = Rotation;
      ZSqueeze = 1;
      Scale = scale * acinfo.InitialScale;
      math = engine.TrueVision.TVMathLibrary;
    }

    public void Init(Engine engine, float3 scale, ParticleCreationInfo acinfo)
    {
      Position = acinfo.Position;
      Rotation = acinfo.Rotation;
      PrevPosition = Position;
      PrevRotation = Rotation;
      ZSqueeze = 1;
      Scale = scale * acinfo.InitialScale;
      math = engine.TrueVision.TVMathLibrary;
    }

    public void Reset()
    {
      currData = new TransformData();
      prevData = new TransformData();
      currTime = 0;
      prevTime = 0;
      ZSqueeze = 1;
      currMat = default(TV_3DMATRIX);
      prevMat = default(TV_3DMATRIX);
    }

    public float3 Scale
    {
      get { return currData.Scale; }
      set { prevData.Scale = currData.Scale; currData.Scale = value; }
    }

    public float ZSqueeze; // special for projectiles

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
      get { return currData.Rotation.ConvertRotToDir(math); }
      set { prevData.Rotation = currData.Rotation; currData.Rotation = value.ConvertDirToRot(math); }
    }

    public TV_3DVECTOR PrevDirection
    {
      get { return prevData.Rotation.ConvertRotToDir(math); }
      private set { prevData.Rotation = value.ConvertDirToRot(math); }
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
      math.TVMatrixRotationYawPitchRoll(ref matrix, data.Yaw, data.Pitch, data.Roll);
      return matrix;
    }

    public TV_3DMATRIX MatrixTranslation { get { return GetMatT(ref currData); } }
    public TV_3DMATRIX PrevMatrixTranslation { get { return GetMatT(ref prevData); } }
    private TV_3DMATRIX GetMatT(ref TransformData data)
    {
      TV_3DMATRIX matrix = new TV_3DMATRIX();
      math.TVMatrixTranslation(ref matrix, data.X, data.Y, data.Z);
      return matrix;
    }

    public TV_3DMATRIX MatrixScale { get { return GetMatS(ref currData); } }
    public TV_3DMATRIX PrevMatrixScale { get { return GetMatS(ref prevData); } }
    private TV_3DMATRIX GetMatS(ref TransformData data)
    {
      TV_3DMATRIX matrix = new TV_3DMATRIX();
      math.TVMatrixScaling(ref matrix, data.Scale.x, data.Scale.y, data.Scale.z * ZSqueeze);
      return matrix;
    }

    public TV_3DMATRIX GetWorldMatrix(T self, float time)
    {
      if (time > currTime)
      {
        currMat = InnerGetWorldMatrix(self, time);
        currTime = time;
      }
      return currMat;
    }

    public TV_3DMATRIX GetPrevWorldMatrix(T self, float time)
    {
      if (time > prevTime)
      {
        prevMat = InnerGetPrevWorldMatrix(self, time);
        prevTime = time;
      }
      return prevMat;
    }

    private TV_3DMATRIX InnerGetWorldMatrix(T a, float time)
    {
      using (ScopeCounters.Acquire(a.Scope))
      {
        U p = a.ParentForCoords;
        if (!p?.Disposed ?? false)
        {
          using (ScopeCounters.Acquire(p.Scope))
          {
            TV_3DMATRIX m = new TV_3DMATRIX();
            math.TVMatrixMultiply(ref m, GetMat(ref currData), p.GetWorldMatrix());
            return m;
          }
        }
      }
      return GetMat(ref currData);
    }

    private TV_3DMATRIX InnerGetPrevWorldMatrix(T a, float time)
    {
      using (ScopeCounters.Acquire(a.Scope))
      {
        U p = a.ParentForCoords;
        if (!p?.Disposed ?? false)
        {
          using (ScopeCounters.Acquire(p.Scope))
          {
            TV_3DMATRIX m = new TV_3DMATRIX();
            math.TVMatrixMultiply(ref m, GetMat(ref prevData), p.GetPrevWorldMatrix());
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
      math.TVMatrixMultiply(ref mSR, GetMatS(ref currData), GetMatR(ref currData));
      math.TVMatrixMultiply(ref mSRT, mSR, GetMatT(ref currData));
      return mSRT;
    }

    public TV_3DVECTOR GetGlobalPosition(T self, float time)
    {
      if (!self.ParentForCoords?.Disposed ?? false)
      {
        TV_3DQUATERNION qR = new TV_3DQUATERNION();
        TV_3DVECTOR vT = new TV_3DVECTOR();
        TV_3DVECTOR vS = new TV_3DVECTOR();
        math.TVMatrixDecompose(ref vS, ref qR, ref vT, GetWorldMatrix(self, time));
        return vT;
      }
      else
        return currData.Position;
    }

    public TV_3DVECTOR GetPrevGlobalPosition(T self, float time)
    {
      if (!self.ParentForCoords?.Disposed ?? false)
      {
        TV_3DQUATERNION qR = new TV_3DQUATERNION();
        TV_3DVECTOR vT = new TV_3DVECTOR();
        TV_3DVECTOR vS = new TV_3DVECTOR();
        math.TVMatrixDecompose(ref vS, ref qR, ref vT, GetPrevWorldMatrix(self, time));
        return vT;
      }
      else
        return prevData.Position;
    }

    public TV_3DVECTOR GetGlobalRotation(T self, float time) // broken
    {
      if (!self.ParentForCoords?.Disposed ?? false)
      {
        TV_3DVECTOR dir = new TV_3DVECTOR();
        math.TVVec3TransformNormal(ref dir, new TV_3DVECTOR(0, 0, 100), GetWorldMatrix(self, time));
        return dir.ConvertDirToRot(self.Engine.TrueVision.TVMathLibrary);
      }
      else
        return currData.Rotation;
    }

    public TV_3DVECTOR GetPrevGlobalRotation(T self, float time) // broken
    {
      if (!self.ParentForCoords?.Disposed ?? false)
      {
        TV_3DVECTOR dir = new TV_3DVECTOR();
        math.TVVec3TransformNormal(ref dir, new TV_3DVECTOR(0, 0, 100), GetPrevWorldMatrix(self, time));
        return dir.ConvertDirToRot(self.Engine.TrueVision.TVMathLibrary);
      }
      else
        return currData.Rotation;
    }

    // ?
    public TV_3DVECTOR GetGlobalDirection(T self, float time)
    {
      TV_3DVECTOR dir = GetGlobalRotation(self, time).ConvertRotToDir(math);
      return dir;
    }

    public TV_3DVECTOR GetPrevGlobalDirection(T self, float time)
    {
      TV_3DVECTOR dir = GetPrevGlobalRotation(self, time).ConvertRotToDir(math);
      return dir;
    }

    public TV_3DVECTOR GetRelativePositionFUR(T self, float time, float forward, float up = 0, float right = 0, bool local = false)
    {
      TV_3DVECTOR pos = local ? Position : GetGlobalPosition(self, time);
      TV_3DVECTOR rot = local ? Rotation : GetGlobalRotation(self, time);
      TV_3DVECTOR ret = new TV_3DVECTOR();

      math.TVVec3Rotate(ref ret, new TV_3DVECTOR(right, up, forward), rot.y, rot.x, rot.z);
      ret += pos;
      return ret;
    }

    public TV_3DVECTOR GetRelativePositionXYZ(T self, float time, float x, float y, float z, bool local = false)
    {
      TV_3DVECTOR pos = local ? Position : GetGlobalPosition(self, time);
      TV_3DVECTOR rot = local ? Rotation : GetGlobalRotation(self, time);
      TV_3DVECTOR ret = new TV_3DVECTOR();

      math.TVVec3Rotate(ref ret, new TV_3DVECTOR(x, y, z), rot.y, rot.x, rot.z);
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
