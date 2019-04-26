using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;

namespace SWEndor.UI.Widgets
{
  public class Score : Widget
  {
    // Score
    private TV_2DVECTOR score_position = new TV_2DVECTOR(Globals.Engine.ScreenWidth * 0.76f, 325);
    private float score_height = Globals.Engine.ScreenHeight - 325;
    private float score_width = Globals.Engine.ScreenWidth * 0.24f;

    public Score(Screen2D owner) : base(owner, "score") { }

    public override bool Visible
    {
      get
      {
        return (!Owner.ShowPage
            && this.GetEngine().PlayerInfo.Actor != null
            && this.GetEngine().PlayerInfo.Actor.ActorState != ActorState.DEAD
            && this.GetEngine().PlayerInfo.Actor.ActorState != ActorState.DYING
            && !(Owner.Engine.PlayerInfo.Actor.TypeInfo is InvisibleCameraATI)
            && Owner.ShowUI
            && Owner.ShowScore);
      }
    }

    public override void Draw()
    {
      TVScreen2DImmediate.Action_Begin2D();
      TVScreen2DImmediate.Draw_FilledBox(score_position.x
                                    , score_position.y
                                    , score_position.x + score_width
                                    , score_position.y + score_height
                                    , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      TVScreen2DImmediate.Action_End2D();

      /*
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

      Globals.Engine.TrueVision.TVScreen2DText.TextureFont_DrawText(hiscoretext
      , score_position.x + 5
      , score_position.y + 5
      , new TV_COLOR(0.7f, 1f, 0.3f, 1).GetIntColor()
      , Font.Factory.Get("Text_10").ID
      );
      */
    }
  }
}
