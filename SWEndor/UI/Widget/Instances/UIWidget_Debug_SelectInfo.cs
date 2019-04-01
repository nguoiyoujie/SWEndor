using MTV3D65;
using SWEndor.Actors;

namespace SWEndor.UI
{
  public class UIWidget_Debug_SelectInfo : UIWidget
  {
    public UIWidget_Debug_SelectInfo() : base("debug_selectinfo") { }

    public override bool Visible
    {
      get
      {
        return false; // Settings.GameDebug;
      }
    }

    public override void Draw()
    {
      Engine.Instance().TVScreen2DText.Action_BeginText();

      TVCollisionResult tvcres = Engine.Instance().TVScene.MousePick((int)Engine.Instance().ScreenWidth / 2, (int)Engine.Instance().ScreenHeight / 2);
      if (tvcres.GetCollisionMesh() != null)
      {
        int n = 0;
        if (int.TryParse(tvcres.GetCollisionMesh().GetMeshName(), out n))
        {
          ActorInfo a = ActorInfo.Factory.GetActor(n);

          if (a != null) //&& a.TypeInfo.CollisionEnabled)
          {
            TV_3DVECTOR vec = tvcres.GetCollisionImpact() - a.GetPosition();
            TV_3DVECTOR vvec = new TV_3DVECTOR();
            TV_3DVECTOR rot = a.GetRotation();
            Engine.Instance().TVMathLibrary.TVVec3Rotate(ref vvec, vec, -rot.y, rot.x, rot.z);

            string text = string.Format("{0}({1:0.0},{2:0.0},{3:0.0})\nPos: {4:0.0},{5:0.0},{6:0.0}\nSpd: {7: 0.0}\n"//{8}  {9:0.0},{10:0.0},{11:0.0}\n"
              , a.Name
              , vvec.x
              , vvec.y
              , vvec.z
              , a.GetPosition().x
              , a.GetPosition().y
              , a.GetPosition().z
              , a.MovementInfo.Speed
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
            
            Engine.Instance().TVScreen2DText.TextureFont_DrawText(text
          , 900, 400, new TV_COLOR(0.8f, 0.8f, 0.2f, 1).GetIntColor(), Font.GetFont("Text_12").ID);
          }
        }
      }
      Engine.Instance().TVScreen2DText.Action_EndText();
    }
  }
}
