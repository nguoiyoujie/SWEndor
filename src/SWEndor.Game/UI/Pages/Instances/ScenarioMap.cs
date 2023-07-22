using MTV3D65;
using SWEndor.Game.Actors;
using SWEndor.Game.Actors.Models;
using SWEndor.Game.Core;
using SWEndor.Game.Models;
using Primrose.Primitives.Geometry;
using SWEndor.Game.AI.Actions;
using Primrose.Primitives.ValueTypes;

namespace SWEndor.Game.UI.Menu.Pages
{
  public class ScenarioMap : Page
  {
    readonly SelectionElement Cover = new SelectionElement();
    readonly SelectionElement BackText = new SelectionElement();
    private static readonly COLOR GridColorMajor = ColorLocalization.Get(ColorLocalKeys.GAME_MAP_GRID_MAJOR);
    private static readonly COLOR GridColorMinor = ColorLocalization.Get(ColorLocalKeys.GAME_MAP_GRID_MINOR);
    private static readonly COLOR MapTargetLine = new COLOR(1, 0.8f, 0.6f, 0.2f);
    private static float zoom_ratio = 0.05f;
    private static readonly float zoom_stepmult = 1.25f;
    private static readonly float max_zoom_ratio = 2f;
    private static readonly float min_zoom_ratio = 0.004f;
    private static readonly float def_zoom_ratio = 0.05f;
    private static bool showtext = true;
    private static TV_2DVECTOR displacement = new TV_2DVECTOR();

    public ScenarioMap(Screen2D owner) : base(owner)
    {
      Cover.HighlightBoxPosition = new TV_2DVECTOR();
      Cover.HighlightBoxWidth = owner.ScreenSize.x;
      Cover.HighlightBoxHeight = owner.ScreenSize.y;
      Cover.UnHighlightBoxColor = ColorLocalization.Get(ColorLocalKeys.GAME_MAP_BACKGROUND);

      BackText.Text = "Press Space to toggle names. \nDirectional arrows to scroll. \n[P] to reset position. \n[+] to zoom in. \n[-] to zoom out. \nBACKSPACE to reset zoom \nPress ESC to return to menu.";
      BackText.TextFont = FontFactory.Get(Font.T12).ID;
      BackText.TextPosition = new TV_2DVECTOR(20, Engine.ScreenHeight - 140);
      BackText.Selectable = true;
      BackText.OnKeyPress += SelectExit;
      BackText.UnHighlightBoxColor = ColorLocalization.Get(ColorLocalKeys.TRANSPARENT);
      BackText.HighlightBoxColor = ColorLocalization.Get(ColorLocalKeys.TRANSPARENT);

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
            Engine.Screen2D.CurrentPage = null;
            Engine.Screen2D.ShowPage = false;
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
      ActorInfo p = Engine.PlayerInfo.Actor;
      if (p != null && p.Active)
      {
        if (p.IsSystemOperational(SystemPart.RADAR))
          Engine.ActorFactory.DoEach(DrawElement);
        else
          DrawElement(Engine, p);
      }
      TVScreen2DImmediate.Action_End2D(); 
    }

    private void DrawGrid()
    {
      TVScreen2DImmediate.Draw_Line(Owner.ScreenCenter.x, 0, Owner.ScreenCenter.x, Owner.ScreenSize.y, GridColorMajor.Value);
      TVScreen2DImmediate.Draw_Line(0, Owner.ScreenCenter.y, Owner.ScreenSize.x, Owner.ScreenCenter.y, GridColorMajor.Value);

      int z = (int)(10000 * zoom_ratio);
      for (int i = z; i < Owner.ScreenCenter.x || i < Owner.ScreenCenter.y; i += z)
      {
        TVScreen2DImmediate.Draw_Line(Owner.ScreenCenter.x + i, 0, Owner.ScreenCenter.x + i, Owner.ScreenSize.y, GridColorMinor.Value);
        TVScreen2DImmediate.Draw_Line(0, Owner.ScreenCenter.y + i, Owner.ScreenSize.x, Owner.ScreenCenter.y + i, GridColorMinor.Value);

        TVScreen2DImmediate.Draw_Line(Owner.ScreenCenter.x - i, 0, Owner.ScreenCenter.x - i, Owner.ScreenSize.y, GridColorMinor.Value);
        TVScreen2DImmediate.Draw_Line(0, Owner.ScreenCenter.y - i, Owner.ScreenSize.x, Owner.ScreenCenter.y - i, GridColorMinor.Value);
      }
    }

    private void DrawElement(Engine engine, ActorInfo a)
    {
      //ActorInfo p = PlayerInfo.Actor;
      if (a != null)// && p != null)
      {
        TV_3DVECTOR ppos = Engine.PlayerCameraInfo.Position * zoom_ratio;
        TV_3DVECTOR apos = a.GetGlobalPosition() * zoom_ratio;

        float size = a.TypeInfo.RenderData.RadarSize * 3;

        if (a.Active
          && size > 0
          //&& !(a.TypeInfo is ActorTypes.Groups.LaserProjectile)
          && !a.UseParentCoords)
        {
          int icolor = a.Faction.Color.Value;
          float proty = Engine.PlayerCameraInfo.Rotation.y;

          XYCoord posvec = new XYCoord { X = ppos.x - apos.x, Y = ppos.z - apos.z };
          PolarCoord polar = posvec.ToPolarCoord;
          polar.Angle -= proty;

          XYCoord xy = polar.ToXYCoord;
          float x = Owner.ScreenCenter.x - xy.X - displacement.x;
          float y = Owner.ScreenCenter.y - xy.Y - displacement.y;

          switch (a.TypeInfo.RenderData.RadarType)
          {
            case RadarType.TRAILLINE:
              float ang = a.GetGlobalRotation().y - proty;
              PolarCoord pang = new PolarCoord { Angle = ang, Dist = 50 * zoom_ratio };
              XYCoord pxy = pang.ToXYCoord;
              float px = x - pxy.X;
              float py = y - pxy.Y;
              DrawLine(x, y, px, py, icolor);
              break;
            case RadarType.HOLLOW_SQUARE:
            case RadarType.FILLED_SQUARE:
              DrawSquare(x, y, size, icolor);
              if (showtext) DrawText(a.Name, x, y + size + 2, icolor);
              break;
            case RadarType.HOLLOW_CIRCLE_S:
            case RadarType.HOLLOW_CIRCLE_M:
            case RadarType.HOLLOW_CIRCLE_L:
            case RadarType.FILLED_CIRCLE_S:
            case RadarType.FILLED_CIRCLE_M:
            case RadarType.FILLED_CIRCLE_L:
              DrawTriangleGiant(a.GetBoundingBox(true), a.Scale, a.GetGlobalRotation().y, x, y, proty, icolor);
              if (showtext) DrawText(a.Name, x, y + size + 2, icolor);
              break;
            case RadarType.RECTANGLE_GIANT:
              DrawRectGiant(a.GetBoundingBox(true), a.Scale, a.GetGlobalRotation().y, x, y, proty, icolor);
              if (showtext) DrawText(a.Name, x, y, icolor);
              break;
            case RadarType.TRIANGLE_GIANT:
              DrawTriangleGiant(a.GetBoundingBox(true), a.Scale, a.GetGlobalRotation().y, x, y, proty, icolor);
              if (showtext) DrawText(a.Name, x, y, icolor);
              break;
            case RadarType.POLYGON:
              DrawPolygon(a.TypeInfo.RenderData.RadarPolygonPoints, a.Scale, a.GetGlobalRotation().y, x, y, proty, icolor);
              if (showtext) DrawText(a.Name, x, y, icolor);
              break; 
          }

          ActionInfo ai = a.CurrentAction;
          if (ai is Move
           || ai is ForcedMove
           || ai is AttackActor
           || ai is FollowActor
           || ai is HyperspaceIn
           )
          {
            TV_3DVECTOR vec = a.AI.Target.Position * zoom_ratio;
            XYCoord aiposvec = new XYCoord { X = ppos.x - vec.x, Y = ppos.z - vec.z };
            PolarCoord aipolar = aiposvec.ToPolarCoord;
            aipolar.Angle -= proty;

            XYCoord aixy = aipolar.ToXYCoord;
            float aix = Owner.ScreenCenter.x - aixy.X - displacement.x;
            float aiy = Owner.ScreenCenter.y - aixy.Y - displacement.y;
            DrawLine(x, y, aix, aiy, MapTargetLine.Value);
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

    private void DrawRectGiant(Box box, float3 scale, float rot_y, float x, float y, float proty, int color)
    {
      scale *= zoom_ratio;

      float bx = box.X.Min * scale.x;
      float bz = box.Z.Min * scale.z;
      float bx2 = box.X.Max * scale.x;
      float bz2 = box.Z.Max * scale.z;
      float ang = rot_y - proty;

      float cos = LookUp.Cos(ang, LookUp.Measure.DEGREES);
      float sin = LookUp.Sin(ang, LookUp.Measure.DEGREES);

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

    private void DrawTriangleGiant(Box box, float3 scale, float rot_y, float x, float y, float proty, int color)
    {
      scale *= zoom_ratio;

      float bx = box.X.Min * scale.x;
      float bz = box.Z.Min * scale.z;
      float bx2 = box.X.Max * scale.x;
      float bxm = (box.X.Min + box.X.Max) / 2 * scale.x;
      float bz2 = box.Z.Max * scale.z;
      float ang = rot_y - proty;

      float cos = LookUp.Cos(ang, LookUp.Measure.DEGREES);
      float sin = -LookUp.Sin(ang, LookUp.Measure.DEGREES);

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

    private void DrawPolygon(float3[] points, float3 scale, float rot_y, float x, float y, float proty, int color)
    {
      if (points == null || points.Length < 2) return;

      scale *= zoom_ratio;
      float bx = points[0].x * scale.x;
      float bz = points[0].z * scale.z;
      float ang = rot_y - proty;
      float cos = LookUp.Cos(ang, LookUp.Measure.DEGREES);
      float sin = -LookUp.Sin(ang, LookUp.Measure.DEGREES);
      TV_2DVECTOR pt0 = new TV_2DVECTOR(x, y) - new TV_2DVECTOR(bx * cos + bz * sin,
                                                                bz * cos - bx * sin);

      TV_2DVECTOR prevpt = pt0;

      for (int i = 1; i < points.Length; i++)
      {
        bx = points[i].x * scale.x;
        bz = points[i].z * scale.z;
        TV_2DVECTOR pt = new TV_2DVECTOR(x, y) - new TV_2DVECTOR(bx * cos + bz * sin,
                                                                 bz * cos - bx * sin);

        if (i == points.Length - 1)
        {
          DrawLine(pt.x, pt.y, pt0.x, pt0.y, color);
        }
        DrawLine(prevpt.x, prevpt.y, pt.x, pt.y, color);
        prevpt = pt;
      }
    }
  }
}
