using SWEndor.Actors;
using System.IO;

namespace SWEndor.ActorTypes
{
  public class Transport_Box2ATI : DebrisGroup
  {
    private static Transport_Box2ATI _instance;
    public static Transport_Box2ATI Instance()
    {
      if (_instance == null) { _instance = new Transport_Box2ATI(); }
      return _instance;
    }

    private Transport_Box2ATI() : base("Transport Box 2")
    {
      // Combat
      IsCombatObject = false;
      IsSelectable = false;
      IsDamage = false;
      CollisionEnabled = false;
      OnTimedLife = true;
      TimedLife = 12f;

      MaxSpeed = 500;
      MinSpeed = 100;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"transport\transport_box2.x");
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);

      ainfo.MovementInfo.ApplyZBalance = false;

      if (!ainfo.IsStateFDefined("RotateAngle"))
      {
        ainfo.SetStateF("RotateAngle", Engine.Instance().Random.Next(-450, 450));
      }
      float rotZ = ainfo.GetStateF("RotateAngle") * Game.Instance().TimeSinceRender;
      ainfo.Rotate(0, 0, rotZ);
      ainfo.MovementInfo.ResetTurn();
    }
  }
}

