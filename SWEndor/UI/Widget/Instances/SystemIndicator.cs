using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Models;
using SWEndor.Primitives.Extensions;

namespace SWEndor.UI.Widgets
{
  public class SystemIndicator : Widget
  {
    private TV_2DVECTOR top_left;
    private float dx;
    private float dy;

    public SystemIndicator(Screen2D owner) : base(owner, "systemstatus")
    {
      top_left = new TV_2DVECTOR(10, 500);
      dx = 150;
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

      int pcolor = p.Faction.Color;
      int fntID = FontFactory.Get(Font.T10).ID;
      float y = top_left.y;
      float x2 = top_left.x + dx;

      TVScreen2DImmediate.Action_Begin2D();
      TVScreen2DImmediate.Draw_FilledBox(top_left.x - 2
                                       , y - 2
                                       , x2 + 62
                                       , y + dy * (p.TypeInfo.SystemData.Parts.Length + 2) + 2
                                       , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      TVScreen2DImmediate.Action_End2D();

      TVScreen2DText.Action_BeginText();
      // Shields
      TVScreen2DText.TextureFont_DrawText("SHIELD"
                                              , top_left.x
                                              , y
                                              , pcolor
                                              , fntID);

      TVScreen2DText.TextureFont_DrawText((p.MaxShd == 0) ? "----" : "{0:0}%".F(p.Shd_Perc)
                                              , x2
                                              , y
                                              , ((p.MaxShd == 0) ? new TV_COLOR(1, 1, 1, 0.4f).GetIntColor() : p.Shd_Color)
                                              , fntID);
      y += dy;

      // Hull
      TVScreen2DText.TextureFont_DrawText("HULL"
                                        , top_left.x
                                        , y
                                        , pcolor
                                        , fntID);

      TVScreen2DText.TextureFont_DrawText((p.MaxHull == 0) ? "100%" : "{0:0}%".F(p.Hull_Perc)
                                              , x2
                                              , y
                                              , ((p.MaxHull == 0) ? new TV_COLOR(0, 1, 0, 1).GetIntColor() : p.Hull_Color)
                                              , fntID);
      y += dy;

      foreach (SystemPart part in p.TypeInfo.SystemData.Parts)
      {
        TVScreen2DText.TextureFont_DrawText(part.ToString().Replace('_', ' ')
                                                      , top_left.x
                                                      , y
                                                      , pcolor
                                                      , fntID);

        SystemState s = p.GetStatus(part);
        int scolor = s == SystemState.ACTIVE ? new TV_COLOR(0.3f, 1f, 0.3f, 1).GetIntColor() :
                       s == SystemState.DISABLED ? new TV_COLOR(0.2f, 0.2f, 0.6f, 1).GetIntColor() :
                       s == SystemState.DESTROYED ? new TV_COLOR(0.7f, 0.2f, 0.2f, 1).GetIntColor() :
                       new TV_COLOR(0.4f, 0.4f, 0.4f, 1).GetIntColor();

        TVScreen2DText.TextureFont_DrawText(s.ToString()
                                              , x2
                                              , y
                                              , scolor
                                              , fntID);

        y += dy;
      }
      TVScreen2DText.Action_EndText();
    }
  }
}
