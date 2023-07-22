using MTV3D65;
using SWEndor.Game.ActorTypes.Components;
using SWEndor.Game.Core;

namespace SWEndor.Game.Player
{
  internal class DeathCameraLook : ICameraLook
  {
    private TargetPosition LookFrom;
    private DeathCameraData DeathCamera;

    public DeathCameraLook() { }

    public TV_3DVECTOR GetPosition(Engine engine) { return LookFrom.GetGlobalPosition(engine); }

    public void SetPosition_Actor(int actorID, DeathCameraData data)
    {
      LookFrom.Position = new TV_3DVECTOR();
      LookFrom.PositionRelative = new TV_3DVECTOR();
      LookFrom.TargetActorID = actorID;
      DeathCamera = data;
    }

    public void SetPosition_Point(TV_3DVECTOR position, DeathCameraData data)
    {
      LookFrom.Position = position;
      LookFrom.PositionRelative = new TV_3DVECTOR();
      LookFrom.TargetActorID = -1;
      DeathCamera = data;
    }

    public void Update(Engine engine, TVCamera cam, float rotz)
    {
      TV_3DVECTOR pos = LookFrom.GetGlobalPosition(engine);

      float angle = (engine.Game.GameTime % DeathCamera.Period) * (2 * Globals.PI / DeathCamera.Period);
      cam.SetPosition(pos.x + DeathCamera.Radius * LookUp.Cos(angle)
                    , pos.y + DeathCamera.Height
                    , pos.z + DeathCamera.Radius * LookUp.Sin(angle));

      cam.SetLookAt(pos.x, pos.y, pos.z);
    }
  }
}
