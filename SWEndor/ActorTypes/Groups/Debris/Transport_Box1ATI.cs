using System.IO;

namespace SWEndor.Actors.Types
{
  public class Transport_Box1ATI : DebrisGroup
  {
    private static Transport_Box1ATI _instance;
    public static Transport_Box1ATI Instance()
    {
      if (_instance == null) { _instance = new Transport_Box1ATI(); }
      return _instance;
    }

    private Transport_Box1ATI() : base("Transport Box 1")
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

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"transport\transport_box1.x");
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

