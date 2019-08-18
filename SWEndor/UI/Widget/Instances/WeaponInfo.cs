using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Traits;
using SWEndor.ActorTypes;

namespace SWEndor.UI.Widgets
{
  public class WidgetWeaponInfo : Widget
  {
    // Left Info
    private float leftinfo_left = 20;
    private float leftinfo_weapontop = 50;
    private float leftinfo_weaponwidth = 95;
    private float leftinfo_weaponheight = 40;

    public WidgetWeaponInfo(Screen2D owner) : base(owner, "weapon") { }

    public override bool Visible
    {
      get
      {
        using (var v = ActorFactory.Get(PlayerInfo.ActorID))
        {
          if (v == null)
            return false;

          ActorInfo p = v.Value;
          return (!Owner.ShowPage
          && !p.StateModel.IsDyingOrDead
          && Owner.ShowUI);
        }
      }
    }

    public override void Draw()
    {
      using (var v = ActorFactory.Get(PlayerInfo.ActorID))
      {
        if (v == null)
          return;

        ActorInfo p = v.Value;
        if (!p.Active)
          return;

        Engine.TrueVision.TVScreen2DImmediate.Action_Begin2D();
        Engine.TrueVision.TVScreen2DImmediate.Draw_FilledBox(leftinfo_left - 5
                                      , leftinfo_weapontop - 5
                                      , leftinfo_left + leftinfo_weaponwidth * 2 + 5
                                      , leftinfo_weapontop + leftinfo_weaponheight + 5
                                      , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());

        Engine.TrueVision.TVScreen2DImmediate.Draw_Box(leftinfo_left - 5
                              , leftinfo_weapontop - 5
                              , leftinfo_left + leftinfo_weaponwidth
                              , leftinfo_weapontop + leftinfo_weaponheight + 5
                              , pcolor.GetIntColor());

        Engine.TrueVision.TVScreen2DImmediate.Draw_Box(leftinfo_left + leftinfo_weaponwidth
                              , leftinfo_weapontop - 5
                              , leftinfo_left + leftinfo_weaponwidth * 2 + 5
                              , leftinfo_weapontop + leftinfo_weaponheight + 5
                              , pcolor.GetIntColor());
        Engine.TrueVision.TVScreen2DImmediate.Action_End2D();


        Engine.TrueVision.TVScreen2DText.Action_BeginText();

        string pweap = PlayerInfo.PrimaryWeapon?.ToUpper() ?? "NONE";
        string sweap = PlayerInfo.SecondaryWeapon?.ToUpper() ?? "NONE";

        Engine.TrueVision.TVScreen2DText.TextureFont_DrawText(pweap
        , leftinfo_left
        , leftinfo_weapontop + 20
        , pcolor.GetIntColor()
        , FontFactory.Get(Font.T16).ID
        );

        Engine.TrueVision.TVScreen2DText.TextureFont_DrawText(sweap
        , leftinfo_left + leftinfo_weaponwidth + 5
        , leftinfo_weapontop + 20
        , pcolor.GetIntColor()
        , FontFactory.Get(Font.T16).ID
        );
        Engine.TrueVision.TVScreen2DText.Action_EndText();
      }
    }
  }
}
