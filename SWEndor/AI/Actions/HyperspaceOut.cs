using MTV3D65;
using SWEndor.Actors;

namespace SWEndor.AI.Actions
{
  public class HyperspaceOut : ActionInfo
  {
    public HyperspaceOut() : base("HyperspaceOut")
    {
      CanInterrupt = false;
    }

    // parameters
    private TV_3DVECTOR Origin_Position = new TV_3DVECTOR();
    private static float Incre_Speed = 15000; //2500;
    private static float FarEnoughDistance = 250000;
    private bool hyperspace = false;

    public override void Process(Engine engine, ActorInfo actor) { }

    public void ApplyMove(ActorInfo owner)
    {
      if (!hyperspace)
      {
        hyperspace = true;
        Origin_Position = owner.GetGlobalPosition();
      }

      owner.MoveData.Speed += Incre_Speed * owner.Game.TimeSinceRender;

      float dist = owner.TrueVision.TVMathLibrary.GetDistanceVec3D(owner.GetGlobalPosition(), Origin_Position);
      if (dist >= FarEnoughDistance)
        Complete = true;
    }
  }
}
