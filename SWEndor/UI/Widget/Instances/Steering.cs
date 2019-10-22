using MTV3D65;
using SWEndor.Actors;

namespace SWEndor.UI.Widgets
{
  public class Steering: Widget
  {
    public Steering(Screen2D owner) : base(owner, "steering") { }

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
      if (p == null || !p.Active)
        return;

      int pcolor = p.Faction.Color.Value;
      float xfrac = p.MoveData.YTurnAngle / p.MoveData.MaxTurnRate / 2;
      float yfrac = p.MoveData.XTurnAngle / p.MoveData.MaxTurnRate / 2;
      float zfrac = p.MoveData.ZRoll / p.MoveData.MaxTurnRate / 2;
      float space = 40;
      float width = 3;

      TVScreen2DImmediate.Action_Begin2D();

      TVScreen2DImmediate.Draw_Box(Engine.ScreenWidth / 2 - space
                                             , Engine.ScreenHeight / 2 
                                             , Engine.ScreenWidth / 2 - space + width
                                             , Engine.ScreenHeight / 2 + 32 * (yfrac + zfrac)
                                             , pcolor);

      TVScreen2DImmediate.Draw_Box(Engine.ScreenWidth / 2 + space
                                       , Engine.ScreenHeight / 2
                                       , Engine.ScreenWidth / 2 + space - width
                                       , Engine.ScreenHeight / 2 + 32 * (yfrac - zfrac)
                                       , pcolor);

      TVScreen2DImmediate.Draw_Box(Engine.ScreenWidth / 2
                                       , Engine.ScreenHeight / 2 - space
                                       , Engine.ScreenWidth / 2 + 32 * (xfrac - zfrac)
                                       , Engine.ScreenHeight / 2 - space + width
                                       , pcolor);

      TVScreen2DImmediate.Draw_Box(Engine.ScreenWidth / 2
                                 , Engine.ScreenHeight / 2 + space
                                 , Engine.ScreenWidth / 2 + 32 * (xfrac + zfrac)
                                 , Engine.ScreenHeight / 2 + space - width
                                 , pcolor);

      TVScreen2DImmediate.Action_End2D();
    }
  }
}
