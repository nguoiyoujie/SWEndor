using MTV3D65;
using Primrose.Primitives;
using Primrose.Primitives.ValueTypes;

namespace SWEndor.Game.Explosions
{
  public partial class ExplosionInfo
  {
    /// <summary>The local position of the object</summary>
    public TV_3DVECTOR Position { get { return Transform.Position; } set { Transform.Position = value; } }

    /// <summary>The previous local position of the object</summary>
    public TV_3DVECTOR PrevPosition { get { return Transform.PrevPosition; } }

    /// <summary>The local rotation of the object</summary>
    public TV_3DVECTOR Rotation { get { return Transform.Rotation; } set { Transform.Rotation = value; } }

    /// <summary>The previous local rotation of the object</summary>
    public TV_3DVECTOR PrevRotation { get { return Transform.PrevRotation; } }

    /// <summary>The local direction of the object</summary>
    public TV_3DVECTOR Direction { get { return Transform.Direction; } set { Transform.Direction = value; } }

    /// <summary>The scale of the object</summary>
    public float3 Scale { get { return Transform.Scale; } set { Transform.Scale = value; } }

    /// <summary>Gets the world transformation matrix of the object</summary>
    public TV_3DMATRIX GetWorldMatrix()
    {
      using (ScopeCounters.Acquire(Scope))
        return Transform.GetWorldMatrix(this, Engine.Game.GameTime);
    }

    /// <summary>Gets the previous world transformation matrix of the object</summary>
    public TV_3DMATRIX GetPrevWorldMatrix()
    {
      using (ScopeCounters.Acquire(Scope))
        return Transform.GetPrevWorldMatrix(this, Engine.Game.GameTime);
    }

    /// <summary>Extracts the world position from the object's transformation matrix</summary>
    public TV_3DVECTOR GetGlobalPosition()
    {
      using (ScopeCounters.Acquire(Scope))
        return Transform.GetGlobalPosition(this, Engine.Game.GameTime);
    }

    /// <summary>Extracts the previous world position from the object's transformation matrix</summary>
    public TV_3DVECTOR GetPrevGlobalPosition()
    {
      using (ScopeCounters.Acquire(Scope))
        return Transform.GetPrevGlobalPosition(this, Engine.Game.GameTime);
    }

    /// <summary>Extracts the world rotation from the object's transformation matrix</summary>
    public TV_3DVECTOR GetGlobalRotation()
    {
      using (ScopeCounters.Acquire(Scope))
        return Transform.GetGlobalRotation(this, Engine.Game.GameTime);
    }

    /// <summary>Extracts the world direction from the object's transformation matrix</summary>
    public TV_3DVECTOR GetGlobalDirection()
    {
      using (ScopeCounters.Acquire(Scope))
        return Transform.GetGlobalDirection(this, Engine.Game.GameTime);
    }

    /// <summary>Gets the relative offset from an object</summary>
    public TV_3DVECTOR GetRelativePositionFUR(float front, float up, float right, bool uselocal = false)
    {
      using (ScopeCounters.Acquire(Scope))
        return Transform.GetRelativePositionFUR(this, Engine.Game.GameTime, front, up, right, uselocal);
    }

    /// <summary>Gets the relative offset from an object</summary>
    public TV_3DVECTOR GetRelativePositionXYZ(float x, float y, float z, bool uselocal = false)
    {
      using (ScopeCounters.Acquire(Scope))
        return Transform.GetRelativePositionXYZ(this, Engine.Game.GameTime, x, y, z, uselocal);
    }

    /// <summary>Moves the object by the relative offset</summary>
    public void MoveRelative(float forward, float up = 0, float right = 0)
    {
      TV_3DVECTOR vec = GetRelativePositionFUR(forward, up, right, true);
      Transform.Position = new TV_3DVECTOR(vec.x, vec.y, vec.z);
    }

    /// <summary>Moves the object by world coordinate offset</summary>
    public void MoveAbsolute(float x, float y, float z)
    {
      TV_3DVECTOR vec = Transform.Position + new TV_3DVECTOR(x, y, z);
      Transform.Position = new TV_3DVECTOR(vec.x, vec.y, vec.z);
    }

    /// <summary>Rotates the object to look at an object, eliminating its Z-rotation in the process</summary>
    public void LookAt(TV_3DVECTOR point) { Transform.LookAt(point); }
  }
}

