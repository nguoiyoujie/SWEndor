using MTV3D65;
using Primrose.Primitives;
using SWEndor.Game.Actors;
using SWEndor.Game.Core;
using SWEndor.Game.Models;
using System;


namespace SWEndor.Game.Player
{
  public class TargetPicker
  {
    private struct TargetScore
    {
      public float Score;
      public int ID;

      public TargetScore(float score, int id)
      {
        Score = score;
        ID = id;
      }
    }

    public static bool PickTargetFighter(Engine engine, bool pick_allies, ref int targetID)
    {
      ActorInfo p = engine.PlayerInfo.Actor;
      TargetScore t = new TargetScore(9999 , targetID);

      if (p != null || p.Active)
        engine.ActorFactory.DoEach(PickTargetFighterInner, pick_allies, ref t);

      targetID = t.ID;
      return t.Score < 9999;
    }

    public static bool PickTargetLargeShip(Engine engine, ref int targetID)
    {
      ActorInfo p = engine.PlayerInfo.Actor;
      bool ret = false;

      if (p != null || p.Active)
      {
        TVCollisionResult tvcres;
        using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
          tvcres = engine.TrueVision.TVScene.MousePick(engine.InputManager.MOUSE_X, engine.InputManager.MOUSE_Y);
        TVMesh mesh = tvcres.GetCollisionMesh();
        if (mesh != null)
        {
          if (engine.ActorMeshTable.TryGet(mesh.GetIndex(), out int n))
          {
            ActorInfo a = engine.ActorFactory.Get(n);

            if (a != null)
            {
              targetID = a.ID;
              ret = true;
            }
          }
        }
      }
      return ret;
    }

    private static void PickTargetFighterInner(Engine e, ActorInfo a, bool b, ref TargetScore s)
    {
      ActorInfo p0 = e.PlayerInfo.Actor;
      if (a != null
        && !a.IsPlayer
        && a.Active
        && !a.IsDyingOrDead
        && a.Mask.Has(ComponentMask.CAN_BETARGETED)
        && (b || !p0.IsAlliedWith(a))
        && e.PlayerCameraInfo.Camera.IsPointVisible(a.GetGlobalPosition())
        )
      {
        float dist = DistanceModel.GetDistance(e, p0, a, 7501);
        if (dist < 7500)
        {
          float x = 0;
          float y = 0;
          float limit = 0.015f * dist;
          if (limit < 50)
            limit = 50;
          e.TrueVision.TVScreen2DImmediate.Math_3DPointTo2D(a.GetGlobalPosition(), ref x, ref y);

          x -= e.ScreenWidth / 2;
          y -= e.ScreenHeight / 2;

          x = Math.Abs(x);
          y = Math.Abs(y);

          if (x < limit && y < limit && x + y < s.Score)
          {
            s.Score = x + y;
            s.ID = a.ID;
          }
        }
      }
    }



  }
}
