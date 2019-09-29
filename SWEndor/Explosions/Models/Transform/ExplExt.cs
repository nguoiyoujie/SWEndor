using MTV3D65;
using SWEndor.Primitives;

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
        return Transform.GetWorldMatrix(this, Game.GameTime);
    }

    public TV_3DMATRIX GetPrevWorldMatrix()
    {
      using (ScopeCounterManager.Acquire(Scope))
        return Transform.GetPrevWorldMatrix(this, Game.GameTime);
    }

    public TV_3DVECTOR GetGlobalPosition()
    {
      using (ScopeCounterManager.Acquire(Scope))
        return Transform.GetGlobalPosition(this, Game.GameTime);
    }

    public TV_3DVECTOR GetPrevGlobalPosition()
    {
      using (ScopeCounterManager.Acquire(Scope))
        return Transform.GetPrevGlobalPosition(this, Game.GameTime);
    }

    public TV_3DVECTOR GetGlobalRotation()
    {
      using (ScopeCounterManager.Acquire(Scope))
        return Transform.GetGlobalRotation(this, Game.GameTime);
    }

    public TV_3DVECTOR GetGlobalDirection()
    {
      using (ScopeCounterManager.Acquire(Scope))
        return Transform.GetGlobalDirection(this, Game.GameTime);
    }

    public TV_3DVECTOR GetRelativePositionFUR(float front, float up, float right, bool uselocal = false)
    {
      using (ScopeCounterManager.Acquire(Scope))
        return Transform.GetRelativePositionFUR(this, Game.GameTime, front, up, right, uselocal);
    }

    public TV_3DVECTOR GetRelativePositionXYZ(float x, float y, float z, bool uselocal = false)
    {
      using (ScopeCounterManager.Acquire(Scope))
        return Transform.GetRelativePositionXYZ(this, Game.GameTime, x, y, z, uselocal);
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

