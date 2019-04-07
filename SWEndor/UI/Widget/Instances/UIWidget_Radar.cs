using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.Player;
using System;
using System.Collections.Generic;

namespace SWEndor.UI
{
  public class UIWidget_Radar : UIWidget
  {
    private TV_2DVECTOR radar_center = new TV_2DVECTOR(0, Engine.Instance().ScreenHeight * 0.24f) + new TV_2DVECTOR(Engine.Instance().ScreenWidth / 2, Engine.Instance().ScreenHeight / 2);
    private float radar_radius = Engine.Instance().ScreenHeight * 0.16f; //100;
    private float radar_range = 4000;
    private float radar_blinkfreq = 2.5f;
    private float radar_bigshiprenderunit = 50;
    private TV_2DVECTOR targetingradar_center = new TV_2DVECTOR(Engine.Instance().ScreenWidth / 2, Engine.Instance().ScreenHeight * 0.28f);
    private float targetingradar_radius = Engine.Instance().ScreenHeight * 0.12f; //100;

    public UIWidget_Radar() : base("radar") { }

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
            && Screen2D.Instance().ShowRadar);
      }
    }

    public override void Draw()
    {
      ActorInfo p = PlayerInfo.Instance().Actor;
      if (p == null || p.CreationState != CreationState.ACTIVE)
        return;

      if (Screen2D.Instance().OverrideTargetingRadar)
        DrawTargetingRadar(p);
      else
        DrawRadar(p);
    }

    private void DrawTargetingRadar(ActorInfo p)
    {
      TV_COLOR pcolor = (p.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : p.Faction.Color;

      float posX = targetingradar_center.x;
      float posY = targetingradar_center.y;
      float left = posX - targetingradar_radius;
      float right = posX + targetingradar_radius;
      float top = posY - targetingradar_radius;
      float bottom = posY + targetingradar_radius;
      float timefactor = Game.Instance().GameTime % 1;
      float divisor = 1.75f;
      while (timefactor + 1 / divisor < 1)
      {
        timefactor += 1 / divisor;
      }

      Engine.Instance().TVScreen2DImmediate.Action_Begin2D();
      Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(left, top, right, bottom, new TV_COLOR(0, 0, 0, 0.25f).GetIntColor());
      Engine.Instance().TVScreen2DImmediate.Draw_Box(left, top, right, bottom, pcolor.GetIntColor());
      Engine.Instance().TVScreen2DImmediate.Draw_Box(posX - targetingradar_radius * 0.1f, posY - targetingradar_radius * 0.2f, posX + targetingradar_radius * 0.1f, posY + targetingradar_radius * 0.2f, pcolor.GetIntColor());
      Engine.Instance().TVScreen2DImmediate.Draw_Line(left, top, posX - targetingradar_radius * 0.1f, posY - targetingradar_radius * 0.2f, pcolor.GetIntColor());
      Engine.Instance().TVScreen2DImmediate.Draw_Line(left, bottom, posX - targetingradar_radius * 0.1f, posY + targetingradar_radius * 0.2f, pcolor.GetIntColor());
      Engine.Instance().TVScreen2DImmediate.Draw_Line(right, top, posX + targetingradar_radius * 0.1f, posY - targetingradar_radius * 0.2f, pcolor.GetIntColor());
      Engine.Instance().TVScreen2DImmediate.Draw_Line(right, bottom, posX + targetingradar_radius * 0.1f, posY + targetingradar_radius * 0.2f, pcolor.GetIntColor());

      while (timefactor / divisor > 0.1f)
      {
        float x = 0.1f + timefactor * 0.9f;
        float y = 0.2f + timefactor * 0.8f;
        Engine.Instance().TVScreen2DImmediate.Draw_Box(posX - targetingradar_radius * x, posY - targetingradar_radius * y, posX - targetingradar_radius * x, posY + targetingradar_radius * y, pcolor.GetIntColor());
        Engine.Instance().TVScreen2DImmediate.Draw_Box(posX + targetingradar_radius * x, posY - targetingradar_radius * y, posX + targetingradar_radius * x, posY + targetingradar_radius * y, pcolor.GetIntColor());
        Engine.Instance().TVScreen2DImmediate.Draw_Box(posX - targetingradar_radius * x, posY + targetingradar_radius * y, posX + targetingradar_radius * x, posY + targetingradar_radius * y, pcolor.GetIntColor());
        timefactor /= divisor;
      }
      Engine.Instance().TVScreen2DImmediate.Action_End2D();

      Engine.Instance().TVScreen2DText.Action_BeginText();
      float letter_size = 4.5f;
      Engine.Instance().TVScreen2DText.TextureFont_DrawText(Screen2D.Instance().TargetingRadar_text, posX - letter_size * 8, top + 5, pcolor.GetIntColor(), Font.Factory.Get("Text_12").ID);
      Engine.Instance().TVScreen2DText.Action_EndText();
    }

    private void DrawRadar(ActorInfo p)
    {
      TV_COLOR pcolor = (p.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : p.Faction.Color;

      Engine.Instance().TVScreen2DImmediate.Action_Begin2D();
      Engine.Instance().TVScreen2DImmediate.Draw_FilledCircle(radar_center.x, radar_center.y, radar_radius + 2, 300, new TV_COLOR(0, 0, 0, 0.25f).GetIntColor());
      Engine.Instance().TVScreen2DImmediate.Draw_Circle(radar_center.x, radar_center.y, radar_radius + 2, 300, pcolor.GetIntColor());
      Engine.Instance().TVScreen2DImmediate.Draw_Circle(radar_center.x, radar_center.y, radar_radius - 2, 300, pcolor.GetIntColor());

      foreach (ActorInfo a in ActorInfo.Factory.GetHoldingList())
      {
        TV_3DVECTOR ppos = p.GetPosition();
        TV_3DVECTOR apos = a.GetPosition();

        if (a.CreationState == CreationState.ACTIVE 
          && a.TypeInfo.RadarSize > 0 
          && (a.TypeInfo.AlwaysShowInRadar || ActorDistanceInfo.GetRoughDistance(new TV_3DVECTOR(ppos.x, 0, ppos.z), new TV_3DVECTOR(apos.x, 0, apos.z)) < radar_range * 2))
        {
          TV_COLOR acolor = (a.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : a.Faction.Color;

          TV_2DVECTOR posvec = new TV_2DVECTOR(ppos.x, ppos.z) - new TV_2DVECTOR(apos.x, apos.z);
          float proty = p.GetRotation().y;
          float dist = Engine.Instance().TVMathLibrary.GetDistanceVec2D(new TV_2DVECTOR(), posvec);
          float angl = Engine.Instance().TVMathLibrary.Direction2Ang(posvec.x, posvec.y) - proty;
          if (dist > radar_range)
            dist = radar_range;

          float x = radar_center.x - radar_radius * dist / radar_range * (float)Math.Sin(angl * Globals.PI / 180);
          float y = radar_center.y + radar_radius * dist / radar_range * (float)Math.Cos(angl * Globals.PI / 180);

          if (a.TypeInfo.TargetType.HasFlag(TargetType.FIGHTER))
          {
            Engine.Instance().TVScreen2DImmediate.Draw_FilledCircle(x, y, a.TypeInfo.RadarSize, 4, acolor.GetIntColor());
          }
          else if (a.TypeInfo.TargetType.HasFlag(TargetType.STRUCTURE))
          {
            Engine.Instance().TVScreen2DImmediate.Draw_FilledCircle(x, y, a.TypeInfo.RadarSize, 12, acolor.GetIntColor());
          }
          else if(a.TypeInfo.TargetType.HasFlag(TargetType.ADDON))
          {
            Engine.Instance().TVScreen2DImmediate.Draw_Circle(x, y, a.TypeInfo.RadarSize, 6, acolor.GetIntColor());
          }
          else if (a.TypeInfo is ExplosionGroup)
          {
            Engine.Instance().TVScreen2DImmediate.Draw_FilledCircle(x, y, a.TypeInfo.RadarSize, 36, new TV_COLOR(0.75f, 0.75f, 0, 1).GetIntColor());
          }
          else if (a.TypeInfo is WarshipGroup)
          {
            float gt = Game.Instance().GameTime * radar_blinkfreq;
            if (a.CombatInfo.Strength / a.TypeInfo.MaxStrength > 0.1f || (gt - (int)gt > 0.5f))
            {
              TV_3DVECTOR boxmin = new TV_3DVECTOR();
              TV_3DVECTOR boxmax = new TV_3DVECTOR();
              a.GetBoundingBox(ref boxmin, ref boxmax, true);
              boxmin = new TV_3DVECTOR(boxmin.x * a.Scale.x, boxmin.y * a.Scale.y, boxmin.z * a.Scale.z);
              boxmax = new TV_3DVECTOR(boxmax.x * a.Scale.x, boxmax.y * a.Scale.y, boxmax.z * a.Scale.z);

              List<TV_2DVECTOR?> ts = new List<TV_2DVECTOR?>();
              float bx = boxmin.x;
              float bz = boxmin.z;
              int i = 0;
              float ang = a.GetRotation().y;
              while (i < 4)
              {
                TV_2DVECTOR temp = posvec;
                temp -= new TV_2DVECTOR(bx * (float)Math.Cos(ang * Globals.PI / 180) + bz * (float)Math.Sin(ang * Globals.PI / 180),
                                        bz * (float)Math.Cos(ang * Globals.PI / 180) - bx * (float)Math.Sin(ang * Globals.PI / 180));
                float tdist = Engine.Instance().TVMathLibrary.GetDistanceVec2D(new TV_2DVECTOR(), temp);
                float tangl = Engine.Instance().TVMathLibrary.Direction2Ang(temp.x, temp.y) - proty;
                if (tdist > radar_range)
                {
                  tdist = radar_range;
                  ts.Add(null);
                }
                float tx = radar_center.x - radar_radius * tdist / radar_range * (float)Math.Sin(tangl * Globals.PI / 180);
                float ty = radar_center.y + radar_radius * tdist / radar_range * (float)Math.Cos(tangl * Globals.PI / 180);
                if (ts.Count > 1 && ts[ts.Count - 1] != null && ts[ts.Count - 2] != null)
                  ts[ts.Count - 1] = new TV_2DVECTOR(tx, ty);
                else
                  ts.Add(new TV_2DVECTOR(tx, ty));

                switch (i)
                {
                  case 3:
                    bz -= radar_bigshiprenderunit;
                    if (bz < boxmin.z)
                    {
                      bz = boxmin.z;
                      i++;
                      ts.Add(null);
                    }
                    break;
                  case 2:
                    bx -= radar_bigshiprenderunit;
                    if (bx < boxmin.x)
                    {
                      bx = boxmin.x;
                      i++;
                      ts.Add(null);
                    }
                    break;
                  case 1:
                    bz += radar_bigshiprenderunit;
                    if (bz > boxmax.z)
                    {
                      bz = boxmax.z;
                      i++;
                      ts.Add(null);
                    }
                    break;
                  case 0:
                    bx += radar_bigshiprenderunit;
                    if (bx > boxmax.x)
                    {
                      bx = boxmax.x;
                      i++;
                      ts.Add(null);
                    }
                    break;
                }
              }

              for (int t = 0; t < ts.Count; t++)
              {
                int u = (t == ts.Count - 1) ? 0 : t + 1;
                if (ts[t] != null && ts[u] != null)
                  Engine.Instance().TVScreen2DImmediate.Draw_Line(((TV_2DVECTOR)ts[t]).x, ((TV_2DVECTOR)ts[t]).y, ((TV_2DVECTOR)ts[u]).x, ((TV_2DVECTOR)ts[u]).y, acolor.GetIntColor());
              }
            }
          }
          else if (a.TypeInfo is StarDestroyerGroup)
          {
            float gt = Game.Instance().GameTime * radar_blinkfreq;
            if (a.CombatInfo.Strength / a.TypeInfo.MaxStrength > 0.1f || (gt - (int)gt > 0.5f))
            {
              TV_3DVECTOR boxmin = new TV_3DVECTOR();
              TV_3DVECTOR boxmax = new TV_3DVECTOR();
              a.GetBoundingBox(ref boxmin, ref boxmax, true);
              boxmin = new TV_3DVECTOR(boxmin.x * a.Scale.x, boxmin.y * a.Scale.y, boxmin.z * a.Scale.z);
              boxmax = new TV_3DVECTOR(boxmax.x * a.Scale.x, boxmax.y * a.Scale.y, boxmax.z * a.Scale.z);

              List<TV_2DVECTOR?> ts = new List<TV_2DVECTOR?>();
              float bx = boxmin.x;
              float bz = boxmin.z;
              int i = 0;
              float ang = a.GetRotation().y;
              while (i < 3)
              {
                TV_2DVECTOR temp = posvec;
                temp -= new TV_2DVECTOR(bx * (float)Math.Cos(ang * Globals.PI / 180) + bz * (float)Math.Sin(ang * Globals.PI / 180),
                                        bz * (float)Math.Cos(ang * Globals.PI / 180) - bx * (float)Math.Sin(ang * Globals.PI / 180));
                float tdist = Engine.Instance().TVMathLibrary.GetDistanceVec2D(new TV_2DVECTOR(), temp);
                float tangl = Engine.Instance().TVMathLibrary.Direction2Ang(temp.x, temp.y) - proty;
                if (tdist > radar_range)
                {
                  tdist = radar_range;
                  ts.Add(null);
                }
                float tx = radar_center.x - radar_radius * tdist / radar_range * (float)Math.Sin(tangl * Globals.PI / 180);
                float ty = radar_center.y + radar_radius * tdist / radar_range * (float)Math.Cos(tangl * Globals.PI / 180);
                if (ts.Count > 1 && ts[ts.Count - 1] != null && ts[ts.Count - 2] != null)
                  ts[ts.Count - 1] = new TV_2DVECTOR(tx, ty);
                else
                  ts.Add(new TV_2DVECTOR(tx, ty));

                switch (i)
                {
                  case 2:
                    bx -= radar_bigshiprenderunit;
                    if (bx <= boxmin.x)
                    {
                      bx = boxmin.x;
                      i++;
                      ts.Add(null);
                    }
                    break;
                  case 1:
                    bz -= radar_bigshiprenderunit;
                    bx += radar_bigshiprenderunit * (boxmax.x - boxmin.x) / (boxmax.z - boxmin.z) / 2;
                    if (bz <= boxmin.z)
                    {
                      bz = boxmin.z;
                      i++;
                      ts.Add(null);
                    }
                    break;
                  case 0:
                    bz += radar_bigshiprenderunit;
                    bx += radar_bigshiprenderunit * (boxmax.x - boxmin.x) / (boxmax.z - boxmin.z) / 2;
                    if (bz >= boxmax.z)
                    {
                      bz = boxmax.z;
                      i++;
                      ts.Add(null);
                    }
                    break;

                }
              }
              
              for (int t = 0; t < ts.Count; t++)
              {
                int u = (t == ts.Count - 1) ? 0 : t + 1;
                if (ts[t] != null && ts[u] != null)
                  Engine.Instance().TVScreen2DImmediate.Draw_Line(((TV_2DVECTOR)ts[t]).x, ((TV_2DVECTOR)ts[t]).y, ((TV_2DVECTOR)ts[u]).x, ((TV_2DVECTOR)ts[u]).y, acolor.GetIntColor());
              }
            }
          }
          else if (a.TypeInfo is ProjectileGroup)
          {
            TV_2DVECTOR prevtemp = new TV_2DVECTOR(ppos.x, ppos.z) - new TV_2DVECTOR(a.PrevPosition.x, a.PrevPosition.z);
            float prevdist = Engine.Instance().TVMathLibrary.GetDistanceVec2D(new TV_2DVECTOR(), prevtemp);
            float prevangl = Engine.Instance().TVMathLibrary.Direction2Ang(prevtemp.x, prevtemp.y) - p.GetRotation().y;
            if (dist < radar_range && prevdist < radar_range)
            {
              float px = radar_center.x - radar_radius * prevdist / radar_range * (float)Math.Sin(prevangl * Globals.PI / 180);
              float py = radar_center.y + radar_radius * prevdist / radar_range * (float)Math.Cos(prevangl * Globals.PI / 180);

              Engine.Instance().TVScreen2DImmediate.Draw_Line(x, y, px, py, new TV_COLOR(0.8f, 0.5f, 0, 1).GetIntColor());
            }
          }
        }
      }
      Engine.Instance().TVScreen2DImmediate.Action_End2D();
    }
  }
}
