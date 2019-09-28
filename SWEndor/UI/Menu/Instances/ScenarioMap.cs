using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.Core;
using SWEndor.Primitives;
using SWEndor.Scenarios;
using System;
using static SWEndor.UI.Widgets.Radar;

namespace SWEndor.UI.Menu.Pages
{
  public class ScenarioMap : Page
  {
    SelectionElement Cover = new SelectionElement();
    SelectionElement BackText = new SelectionElement();
    private static readonly int GridColorMajor = new TV_COLOR(1, 1, 0.2f, 0.6f).GetIntColor();
    private static readonly int GridColorMinor = new TV_COLOR(1, 1, 0.2f, 0.3f).GetIntColor();
    private static float zoom_ratio = 0.05f;
    private static float zoom_stepmult = 1.25f;
    private static float max_zoom_ratio = 2f;
    private static float min_zoom_ratio = 0.004f;
    private static float def_zoom_ratio = 0.05f;
    private static bool showtext = true;
    private static TV_2DVECTOR displacement = new TV_2DVECTOR();

    GameScenarioBase SelectedScenario = null;

    public ScenarioMap(Screen2D owner, GameScenarioBase selectedScenario) : base(owner)
    {
      SelectedScenario = selectedScenario;

      Cover.HighlightBoxPosition = new TV_2DVECTOR();
      Cover.HighlightBoxWidth = Engine.ScreenWidth;
      Cover.HighlightBoxHeight = Engine.ScreenHeight;
      Cover.UnHighlightBoxPositionColor = new TV_COLOR(0, 0, 0, 0.8f);

      BackText.Text = "Press Space to toggle names. \nDirectional arrows to scroll. \n[P] to reset position. \n[+] to zoom in. \n[-] to zoom out. \nBACKSPACE to reset zoom \nPress ESC to return to menu.";
      BackText.TextFont = FontFactory.Get(Font.T12).ID;
      BackText.TextPosition = new TV_2DVECTOR(20, Engine.ScreenHeight - 140);
      BackText.Selectable = true;
      BackText.OnKeyPress += SelectExit;
      BackText.UnHighlightBoxPositionColor = new TV_COLOR(0, 0, 0, 0);
      BackText.HighlightBoxPositionColor = new TV_COLOR(0, 0, 0, 0);

      Elements.Add(Cover);
      Elements.Add(BackText);
      SelectedElementID = Elements.IndexOf(BackText);
    }

    private bool SelectExit(CONST_TV_KEY key)
    {
      switch (key)
      {
        case CONST_TV_KEY.TV_KEY_ESCAPE:
          if (!Back())
          {
            Globals.Engine.Screen2D.CurrentPage = null;
            Globals.Engine.Screen2D.ShowPage = false;
          }
          return true;
        case CONST_TV_KEY.TV_KEY_MINUS:
          if (zoom_ratio > min_zoom_ratio)
          {
            zoom_ratio /= zoom_stepmult;
            displacement /= zoom_stepmult;
          }
          return false;

        case CONST_TV_KEY.TV_KEY_EQUALS:
          if (zoom_ratio < max_zoom_ratio)
          {
            zoom_ratio *= zoom_stepmult;
            displacement *= zoom_stepmult;
          }
          return false;

        case CONST_TV_KEY.TV_KEY_P:
          displacement = new TV_2DVECTOR();
          return true;

        case CONST_TV_KEY.TV_KEY_BACKSPACE:
          displacement *= def_zoom_ratio / zoom_ratio;
          zoom_ratio = def_zoom_ratio;
          return true;

        case CONST_TV_KEY.TV_KEY_SPACE:
          showtext = !showtext;
          return true;

        case CONST_TV_KEY.TV_KEY_UP:
          displacement -= new TV_2DVECTOR(0, 100);
          return false;

        case CONST_TV_KEY.TV_KEY_DOWN:
          displacement += new TV_2DVECTOR(0, 100);
          return false;

        case CONST_TV_KEY.TV_KEY_LEFT:
          displacement -= new TV_2DVECTOR(100, 0);
          return false;

        case CONST_TV_KEY.TV_KEY_RIGHT:
          displacement += new TV_2DVECTOR(100, 0);
          return false;
      }

      return false;
    }

    public override void RenderTick()
    {
      base.RenderTick();
      TVScreen2DImmediate.Action_Begin2D();
      DrawGrid();
      ActorFactory.DoEach(DrawElement);
      TVScreen2DImmediate.Action_End2D(); 
    }

    private void DrawGrid()
    {
      TVScreen2DImmediate.Draw_Line(Engine.ScreenWidth / 2, 0, Engine.ScreenWidth / 2, Engine.ScreenHeight, GridColorMajor);
      TVScreen2DImmediate.Draw_Line(0, Engine.ScreenHeight / 2, Engine.ScreenWidth, Engine.ScreenHeight / 2, GridColorMajor);

      int z = (int)(10000 * zoom_ratio);
      for (int i = z; i < Engine.ScreenWidth / 2 || i < Engine.ScreenHeight / 2; i += z)
      {
        TVScreen2DImmediate.Draw_Line(Engine.ScreenWidth / 2 + i, 0, Engine.ScreenWidth / 2 + i, Engine.ScreenHeight, GridColorMinor);
        TVScreen2DImmediate.Draw_Line(0, Engine.ScreenHeight / 2 + i, Engine.ScreenWidth, Engine.ScreenHeight / 2 + i, GridColorMinor);

        TVScreen2DImmediate.Draw_Line(Engine.ScreenWidth / 2 - i, 0, Engine.ScreenWidth / 2 - i, Engine.ScreenHeight, GridColorMinor);
        TVScreen2DImmediate.Draw_Line(0, Engine.ScreenHeight / 2 - i, Engine.ScreenWidth, Engine.ScreenHeight / 2 - i, GridColorMinor);
      }
    }

    private void DrawElement(Engine engine, ActorInfo a)
    {
      //ActorInfo p = PlayerInfo.Actor;
      if (a != null)// && p != null)
      {
        TV_3DVECTOR ppos = PlayerCameraInfo.Position * zoom_ratio;
        TV_3DVECTOR apos = a.GetGlobalPosition() * zoom_ratio;

        float size = a.TypeInfo.RenderData.RadarSize * 3;

        if (a.Active
          && size > 0
          && !(a.TypeInfo is ActorTypes.Groups.Explosion)
          && !(a.TypeInfo is ActorTypes.Groups.LaserProjectile)
          && !a.UseParentCoords)
        {
          int acolor = a.Faction.Color.GetIntColor();
          float proty = PlayerCameraInfo.Rotation.y;

          XYCoord posvec = new XYCoord { X = ppos.x - apos.x, Y = ppos.z - apos.z };
          PolarCoord polar = posvec.ToPolarCoord;
          polar.Angle -= proty;

          XYCoord xy = polar.ToXYCoord;
          float x = Engine.ScreenWidth / 2 - xy.X - displacement.x;
          float y = Engine.ScreenHeight / 2 - xy.Y - displacement.y;

          switch (a.TypeInfo.RenderData.RadarType)
          {
            case RadarType.TRAILLINE:
              float ang = a.GetGlobalRotation().y - proty;
              PolarCoord pang = new PolarCoord { Angle = ang, Dist = 50 * zoom_ratio };
              XYCoord pxy = pang.ToXYCoord;
              float px = x - pxy.X;
              float py = y - pxy.Y;
              DrawLine(x, y, px, py, acolor);
              break;
            case RadarType.HOLLOW_SQUARE:
            case RadarType.FILLED_SQUARE:
              DrawSquare(x, y, size, acolor);
              if (showtext) DrawText(a.Name, x, y + size + 2, acolor);
              break;
            case RadarType.HOLLOW_CIRCLE_S:
            case RadarType.HOLLOW_CIRCLE_M:
            case RadarType.HOLLOW_CIRCLE_L:
            case RadarType.FILLED_CIRCLE_S:
            case RadarType.FILLED_CIRCLE_M:
            case RadarType.FILLED_CIRCLE_L:
              DrawTriangleGiant(a, x, y, proty, acolor);
              if (showtext) DrawText(a.Name, x, y + size + 2, acolor);
              break;
            case RadarType.RECTANGLE_GIANT:
              DrawRectGiant(a, x, y, proty, acolor);
              if (showtext) DrawText(a.Name, x, y, acolor);
              break;
            case RadarType.TRIANGLE_GIANT:
              DrawTriangleGiant(a, x, y, proty, acolor);
              if (showtext) DrawText(a.Name, x, y, acolor);
              break;
          }
        }
      }
    }

    private void DrawLine(float x0, float y0, float x1, float y1, int color)
    {
      TVScreen2DImmediate.Draw_Line(x0, y0, x1, y1, color);
    }

    private void DrawText(string text, float x0, float y0, int color)
    {
      TVScreen2DText.Action_BeginText();
      float letter_size = 2.5f;
      TVScreen2DText.TextureFont_DrawText(text, x0 - letter_size * text.Length, y0, color, FontFactory.Get(Font.T10).ID);
      TVScreen2DText.Action_EndText();
    }

    private void DrawSquare(float x, float y, float size, int color)
    {
      TVScreen2DImmediate.Draw_FilledBox(x - size, y - size, x + size, y + size, color);
    }

    private void DrawCircle(float x, float y, float size, int points, int color)
    {
      TVScreen2DImmediate.Draw_FilledCircle(x, y, size, points, color);
    }

    private void DrawRectGiant(ActorInfo a, float x, float y, float proty, int color)
    {
      BoundingBox box = a.GetBoundingBox(true);
      float scale = a.Scale * zoom_ratio;

      float bx = box.X.Min * scale;
      float bz = box.Z.Min * scale;
      float bx2 = box.X.Max * scale;
      float bz2 = box.Z.Max * scale;
      float ang = a.GetGlobalRotation().y - proty;

      float cos = (float)Math.Cos(ang * Globals.PI / 180);
      float sin = -(float)Math.Sin(ang * Globals.PI / 180);

      TV_2DVECTOR pt1 = new TV_2DVECTOR(x, y) - new TV_2DVECTOR(bx * cos + bz * sin,
                                                                bz * cos - bx * sin);

      TV_2DVECTOR pt2 = new TV_2DVECTOR(x, y) - new TV_2DVECTOR(bx * cos + bz2 * sin,
                                                          bz2 * cos - bx * sin);

      TV_2DVECTOR pt3 = new TV_2DVECTOR(x, y) - new TV_2DVECTOR(bx2 * cos + bz2 * sin,
                                                          bz2 * cos - bx2 * sin);

      TV_2DVECTOR pt4 = new TV_2DVECTOR(x, y) - new TV_2DVECTOR(bx2 * cos + bz * sin,
                                                          bz * cos - bx2 * sin);

      DrawLine(pt1.x, pt1.y, pt2.x, pt2.y, color);
      DrawLine(pt2.x, pt2.y, pt3.x, pt3.y, color);
      DrawLine(pt3.x, pt3.y, pt4.x, pt4.y, color);
      DrawLine(pt4.x, pt4.y, pt1.x, pt1.y, color);
    }

    private void DrawTriangleGiant(ActorInfo a, float x, float y, float proty, int color)
    {
      BoundingBox box = a.GetBoundingBox(true);
      float scale = a.Scale * zoom_ratio;

      float bx = box.X.Min * scale;
      float bz = box.Z.Min * scale;
      float bx2 = box.X.Max * scale;
      float bxm = (box.X.Min + box.X.Max) / 2 * scale;
      float bz2 = box.Z.Max * scale;
      float ang = a.GetGlobalRotation().y - proty;

      float cos = (float)Math.Cos(ang * Globals.PI / 180);
      float sin = -(float)Math.Sin(ang * Globals.PI / 180);

      TV_2DVECTOR pt1 = new TV_2DVECTOR(x, y) - new TV_2DVECTOR(bx * cos + bz * sin,
                                                                bz * cos - bx * sin);

      TV_2DVECTOR pt2 = new TV_2DVECTOR(x, y) - new TV_2DVECTOR(bxm * cos + bz2 * sin,
                                                                bz2 * cos - bxm * sin);

      TV_2DVECTOR pt3 = new TV_2DVECTOR(x, y) - new TV_2DVECTOR(bx2 * cos + bz * sin,
                                                                bz * cos - bx2 * sin);

      DrawLine(pt1.x, pt1.y, pt2.x, pt2.y, color);
      DrawLine(pt2.x, pt2.y, pt3.x, pt3.y, color);
      DrawLine(pt3.x, pt3.y, pt1.x, pt1.y, color);
    }
  }
}
