using MTV3D65;
using SWEndor.Actors;
using SWEndor.Primitives;

namespace SWEndor.UI.Widgets
{
  public class Debug_SelectInfo : Widget
  {
    public Debug_SelectInfo(Screen2D owner) : base(owner, "debug_selectinfo") { }

    public override bool Visible
    {
      get
      {
        return false; // Settings.GameDebug;
      }
    }

    public override void Draw()
    {
      TVScreen2DText.Action_BeginText();

      TVCollisionResult tvcres;
      using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
        tvcres = Engine.TrueVision.TVScene.MousePick((int)Engine.ScreenWidth / 2, (int)Engine.ScreenHeight / 2);

      TVMesh mesh = tvcres.GetCollisionMesh();
      if (mesh != null)
      {
        int n = ActorInfo.MeshModel.GetID(mesh.GetIndex());
        if (n != -1)
        {
          ActorInfo a = Engine.ActorFactory.Get(n);

          if (a != null) //&& a.TypeInfo.CollisionEnabled)
          {
            TV_3DVECTOR vec = tvcres.GetCollisionImpact() - a.GetGlobalPosition();
            TV_3DVECTOR vvec = new TV_3DVECTOR();
            TV_3DVECTOR rot = a.GetGlobalRotation();
            Engine.TrueVision.TVMathLibrary.TVVec3Rotate(ref vvec, vec, -rot.y, rot.x, rot.z);

            string text = string.Format("{0}({1:0.0},{2:0.0},{3:0.0})\nPos: {4:0.0},{5:0.0},{6:0.0}\nSpd: {7: 0.0}\n"//{8}  {9:0.0},{10:0.0},{11:0.0}\n"
              , a.Name
              , vvec.x
              , vvec.y
              , vvec.z
              , a.GetGlobalPosition().x
              , a.GetGlobalPosition().y
              , a.GetGlobalPosition().z
              , a.MoveData.Speed
              //, a.AI.Master.CurrAI
              //, a.AI.Master.Move_TargetPosition.x
              //, a.AI.Master.Move_TargetPosition.y
              //, a.AI.Master.Move_TargetPosition.z
              );

            /*foreach (AIElement ai in a.AI.Orders)
            {
              text += string.Format("\n{0}  ({1:0.0},{2:0.0},{3:0.0})"
                                    , ai.AIType
                                    , ai.TargetPosition.x
                                    , ai.TargetPosition.y
                                    , ai.TargetPosition.z
                                    );
            }*/
            
            TVScreen2DText.TextureFont_DrawText(text
          , 900, 400, new TV_COLOR(0.8f, 0.8f, 0.2f, 1).GetIntColor(), FontFactory.Get(Font.T12).ID);
          }
        }
      }
      TVScreen2DText.Action_EndText();
    }
  }
}
