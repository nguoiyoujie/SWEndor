using MTV3D65;
using SWEndor.Game.Actors;
using SWEndor.Game.Actors.Models;

namespace SWEndor.Game.UI.Widgets
{
  public class SystemIndicatorWidget : Widget
  {
    private readonly TV_2DVECTOR top_left;
    private readonly float dx;
    private readonly float dx2;
    private readonly float dy;

    // this widget is run almost every frame, but the strings involved don't change that much.
    // cache values and strings


    public SystemIndicatorWidget(Screen2D owner) : base(owner, "systemstatus")
    {
      top_left = new TV_2DVECTOR(20, Engine.ScreenHeight * 0.66f - 60);
      dx = 150;
      dx2 = 60;
      dy = 12;
    }

    public override bool Visible
    {
      get
      {
        ActorInfo p = PlayerInfo.Actor;
        return (!Owner.ShowPage
          && p != null
          && !p.IsDyingOrDead
          && Owner.ShowUI);
      }
    }

    public override void Draw()
    {
      ActorInfo p = PlayerInfo.Actor;
      if (p == null)
        return;

      int icolor = pcolor.Value;
      int fntID = FontFactory.Get(Font.T10).ID;
      float y = top_left.y;
      float x2 = top_left.x + dx;

      TVScreen2DImmediate.Action_Begin2D();
      TVScreen2DImmediate.Draw_FilledBox(top_left.x - 2
                                       , y - 2
                                       , x2 + dx2 + 2
                                       , y + dy * (p.TypeInfo.SystemData.Parts.Length + 3) + 2
                                       , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      TVScreen2DImmediate.Action_End2D();

      TVScreen2DText.Action_BeginText();
      // Shields
      TVScreen2DText.TextureFont_DrawText("SHIELD"
                                              , top_left.x
                                              , y
                                              , icolor
                                              , fntID);


      TVScreen2DText.TextureFont_DrawText((p.MaxShd == 0) ? "----" : LookUpString.GetIntegerPercent(p.Shd_Perc)
                                              , x2
                                              , y
                                              , ((p.MaxShd == 0) ? new COLOR(1, 1, 1, 0.4f) : p.Shd_Color).Value
                                              , fntID);
      y += dy;

      // Hull
      TVScreen2DText.TextureFont_DrawText("HULL"
                                        , top_left.x
                                        , y
                                        , icolor
                                        , fntID);

      TVScreen2DText.TextureFont_DrawText((p.MaxHull == 0) ? "100%" : LookUpString.GetIntegerPercent(p.Hull_Perc)
                                              , x2
                                              , y
                                              , ((p.MaxHull == 0) ? new COLOR(0, 1, 0, 1) : p.Hull_Color).Value
                                              , fntID);
      y += dy;
      y += dy;

      foreach (SystemInstrument instrument in p.GetInstruments())
      {
        TVScreen2DText.TextureFont_DrawText(instrument.PartType.GetDisplayName()
                                                      , top_left.x
                                                      , y
                                                      , icolor
                                                      , fntID);

        ColorLocalKeys k = ColorLocalKeys.GAME_SYSTEMSTATE_NULL;
        string text = null;
        switch (instrument.Status)
        {
          case SystemState.DAMAGED:
            k = ColorLocalKeys.GAME_SYSTEMSTATE_DAMAGED;
            text = LookUpString.GetTimeDisplay(instrument.RecoveryCooldownTime - Engine.Game.GameTime);
            break;
          case SystemState.DISABLED:
            k = ColorLocalKeys.GAME_SYSTEMSTATE_DISABLED;
            text = "DISABLED";
            break;
          case SystemState.ACTIVE:
            k = (instrument.Endurance < instrument.MaxEndurance) ? ColorLocalKeys.GAME_SYSTEMSTATE_ACTIVE_DAMAGED : ColorLocalKeys.GAME_SYSTEMSTATE_ACTIVE;
            text = LookUpString.GetRatioDisplay(instrument.Endurance, instrument.MaxEndurance);
            break;
        }
        int scolor = ColorLocalization.Get(k).Value;
        if (text != null)
        {
          TVScreen2DText.TextureFont_DrawText(text
                                                , x2
                                                , y
                                                , scolor
                                                , fntID);
        }
        y += dy;
      }
      TVScreen2DText.Action_EndText();
    }
  }
}
