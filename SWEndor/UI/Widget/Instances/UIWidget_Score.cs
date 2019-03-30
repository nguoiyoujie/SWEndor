using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Types;
using System.Collections.Generic;

namespace SWEndor.UI
{
  public class UIWidget_Score : UIWidget
  {
    // Score
    private TV_2DVECTOR score_position = new TV_2DVECTOR(Engine.Instance().ScreenWidth * 0.76f, 325);
    private float score_height = Engine.Instance().ScreenHeight - 325;
    private float score_width = Engine.Instance().ScreenWidth * 0.24f;

    public UIWidget_Score() : base("score") { }

    public override bool Visible
    {
      get
      {
        return (!Screen2D.Instance().ShowPage
            && PlayerInfo.Instance().Actor != null
            && PlayerInfo.Instance().Actor.ActorState != ActorState.DEAD
            && PlayerInfo.Instance().Actor.ActorState != ActorState.DYING
            && !(PlayerInfo.Instance().Actor.TypeInfo is InvisibleCameraATI)
            && Screen2D.Instance().ShowUI
            && Screen2D.Instance().ShowScore);
      }
    }

    public override void Draw()
    {
      Engine.Instance().TVScreen2DImmediate.Action_Begin2D();
      Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(score_position.x
                                    , score_position.y
                                    , score_position.x + score_width
                                    , score_position.y + score_height
                                    , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      Engine.Instance().TVScreen2DImmediate.Action_End2D();

      List<ScoreInfo> HighScorers = new List<ScoreInfo>(ScoreInfo.Scores.GetList());
      HighScorers.RemoveAll(x => x.Score == 0);
      HighScorers.Sort(new ScoreInfo.ScoreComparer());

      string hiscoretext = "LEADERS         | SCORE    | KILL | HIT";
      int count = (HighScorers.Count > 25) ? 25 : HighScorers.Count;
      int hi = HighScorers.Count - 1;
      ScoreInfo[] hilist = new ScoreInfo[HighScorers.Count];
      HighScorers.CopyTo(hilist, 0);
      while (count > 0 && hi >= 0)
      {
        hiscoretext += string.Format("\n{0,15}   {1,8:0}   {2,4:0}   {3,4:0}", (hilist[hi].Name.Length > 15) ? hilist[hi].Name.Remove(15) : hilist[hi].Name.PadRight(15), hilist[hi].Score, hilist[hi].Kills, hilist[hi].Hits);
        count--;
        hi--;
      }

      Engine.Instance().TVScreen2DText.TextureFont_DrawText(hiscoretext
      , score_position.x + 5
      , score_position.y + 5
      , new TV_COLOR(0.7f, 1f, 0.3f, 1).GetIntColor()
      , Font.GetFont("Text_10").ID
      );

    }
  }
}
