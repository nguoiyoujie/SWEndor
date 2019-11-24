using MTV3D65;
using SWEndor.Core;

namespace SWEndor.Player
{
  public class FreeLook : ICameraLook
  {
    private TargetPosition LookFrom;

    public FreeLook() { LookFrom.TargetActorID = -1; }

    public TV_3DVECTOR Position
    {
      get { return LookFrom.Position; }
      set { LookFrom.Position = value; }
    }

    public TV_3DVECTOR Rotation
    {
      get { return LookFrom.PositionRelative; }
      set { LookFrom.PositionRelative = value; }
    }

    public TV_3DVECTOR GetPosition(Engine engine) { return LookFrom.GetGlobalPosition(engine); }

    public void Move_Forward(Engine engine, float displacement)
    {
      Position = Utilities.GetRelativePositionFUR(engine, Position, Rotation, displacement, 0, 0);
    }

    public void Move_Backward(Engine engine, float displacement)
    {
      Position = Utilities.GetRelativePositionFUR(engine, Position, Rotation, -displacement, 0, 0);
    }

    public void Move_Leftward(Engine engine, float displacement)
    {
      Position = Utilities.GetRelativePositionFUR(engine, Position, Rotation, 0, 0, -displacement);
    }

    public void Move_Rightward(Engine engine, float displacement)
    {
      Position = Utilities.GetRelativePositionFUR(engine, Position, Rotation, 0, 0, displacement);
    }

    public void Move_Upward(Engine engine, float displacement)
    {
      Position = Utilities.GetRelativePositionFUR(engine, Position, Rotation, 0, displacement, 0);
    }

    public void Move_Downward(Engine engine, float displacement)
    {
      Position = Utilities.GetRelativePositionFUR(engine, Position, Rotation, 0, -displacement, 0);
    }

    public void Update(Engine engine, TVCamera cam, float rotz)
    {
      TV_3DVECTOR pos = LookFrom.Position;
      TV_3DVECTOR tgt = Utilities.GetRelativePositionFUR(engine, Position, Rotation, 100, 0, 0);

      cam.SetPosition(pos.x, pos.y, pos.z);
      cam.SetLookAt(tgt.x, tgt.y, tgt.z);
    }
  }
}
