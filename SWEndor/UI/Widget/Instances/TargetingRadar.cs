using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.Explosions;
using SWEndor.Models;
using SWEndor.Primitives;
using SWEndor.Projectiles;
using System;
using System.Collections.Generic;

namespace SWEndor.UI.Widgets
{
  public class TargetingRadar : Widget
  {
    private TV_2DVECTOR radar_center;
    private float radar_radius;
    private float radar_range;
    private float radar_blinkfreq;
    private float radar_bigshiprenderunit;
    private TV_2DVECTOR targetingradar_center;
    private float targetingradar_radius;

    public TargetingRadar(Screen2D owner) : base(owner, "targetingradar")
    {
      radar_center = new TV_2DVECTOR(Engine.ScreenWidth * 0.34f, Engine.ScreenHeight * 0.24f) + new TV_2DVECTOR(Engine.ScreenWidth / 2, Engine.ScreenHeight / 2);
      radar_radius = Engine.ScreenHeight * 0.16f; //100;
      radar_range = 4000;
      radar_blinkfreq = 2.5f;
      radar_bigshiprenderunit = 50;
      targetingradar_center = new TV_2DVECTOR(Engine.ScreenWidth / 2, Engine.ScreenHeight * 0.28f);
      targetingradar_radius = Engine.ScreenHeight * 0.12f; //100;
    }

  public override bool Visible
    {
      get
      {
        ActorInfo p = PlayerInfo.Actor;
        return (!Owner.ShowPage
          && p != null
          && !p.IsDyingOrDead
          && Owner.ShowUI
          && Owner.ShowRadar
          && Owner.OverrideTargetingRadar);
      }
    }

    public override void Draw()
    {
      ActorInfo p = PlayerInfo.Actor;
      if (p == null || !p.Active)
        return;

      float posX = targetingradar_center.x;
      float posY = targetingradar_center.y;
      float left = posX - targetingradar_radius;
      float right = posX + targetingradar_radius;
      float top = posY - targetingradar_radius;
      float bottom = posY + targetingradar_radius;
      float timefactor = Engine.Game.GameTime % 1;
      float divisor = 1.75f;
      while (timefactor + 1 / divisor < 1)
      {
        timefactor += 1 / divisor;
      }

      TVScreen2DImmediate.Action_Begin2D();
      TVScreen2DImmediate.Draw_FilledBox(left, top, right, bottom, new TV_COLOR(0, 0, 0, 0.25f).GetIntColor());
      TVScreen2DImmediate.Draw_Box(left, top, right, bottom, pcolor.GetIntColor());
      TVScreen2DImmediate.Draw_Box(posX - targetingradar_radius * 0.1f, posY - targetingradar_radius * 0.2f, posX + targetingradar_radius * 0.1f, posY + targetingradar_radius * 0.2f, pcolor.GetIntColor());
      TVScreen2DImmediate.Draw_Line(left, top, posX - targetingradar_radius * 0.1f, posY - targetingradar_radius * 0.2f, pcolor.GetIntColor());
      TVScreen2DImmediate.Draw_Line(left, bottom, posX - targetingradar_radius * 0.1f, posY + targetingradar_radius * 0.2f, pcolor.GetIntColor());
      TVScreen2DImmediate.Draw_Line(right, top, posX + targetingradar_radius * 0.1f, posY - targetingradar_radius * 0.2f, pcolor.GetIntColor());
      TVScreen2DImmediate.Draw_Line(right, bottom, posX + targetingradar_radius * 0.1f, posY + targetingradar_radius * 0.2f, pcolor.GetIntColor());

      while (timefactor / divisor > 0.1f)
      {
        float x = 0.1f + timefactor * 0.9f;
        float y = 0.2f + timefactor * 0.8f;
        TVScreen2DImmediate.Draw_Box(posX - targetingradar_radius * x, posY - targetingradar_radius * y, posX - targetingradar_radius * x, posY + targetingradar_radius * y, pcolor.GetIntColor());
        TVScreen2DImmediate.Draw_Box(posX + targetingradar_radius * x, posY - targetingradar_radius * y, posX + targetingradar_radius * x, posY + targetingradar_radius * y, pcolor.GetIntColor());
        TVScreen2DImmediate.Draw_Box(posX - targetingradar_radius * x, posY + targetingradar_radius * y, posX + targetingradar_radius * x, posY + targetingradar_radius * y, pcolor.GetIntColor());
        timefactor /= divisor;
      }
      TVScreen2DImmediate.Action_End2D();

      TVScreen2DText.Action_BeginText();
      float letter_size = 4.5f;
      TVScreen2DText.TextureFont_DrawText(Owner.TargetingRadar_text, posX - letter_size * 8, top + 5, pcolor.GetIntColor(), FontFactory.Get(Font.T12).ID);
      TVScreen2DText.Action_EndText();
    }
  }
}
