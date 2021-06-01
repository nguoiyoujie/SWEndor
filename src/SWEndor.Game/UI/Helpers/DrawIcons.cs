using MTV3D65;
using Primrose.Primitives.Extensions;
using Primrose.Primitives.Geometry;
using Primrose.Primitives.ValueTypes;
using SWEndor.Game.Actors;

namespace SWEndor.Game.UI.Helpers
{
  public static class DrawIcons
  {
    public static void DrawSquadLeader(TVScreen2DImmediate tv, Screen2D s2d, ActorInfo s)
    {
      COLOR scolor = s.Faction.Color;
      float sx = 0;
      float sy = 0;
      tv.Math_3DPointTo2D(s.GetGlobalPosition(), ref sx, ref sy);
      DrawIcon(tv, sx, sy, s2d.Textures.Texture_Target_leader, s2d.Textures.Texture_Target_size, scolor.Value);

      //TVScreen2DImmediate.Draw_Circle(sx, sy, iconsize + 5, 6, color.Value);
      //TVScreen2DImmediate.Draw_Circle(sx, sy, iconsize, 6, color.Value);
    }

    public static void DrawSquadMember(TVScreen2DImmediate tv, Screen2D s2d, ActorInfo s)
    {
      COLOR scolor = s.Faction.Color;
      float sx = 0;
      float sy = 0;
      tv.Math_3DPointTo2D(s.GetGlobalPosition(), ref sx, ref sy);
      DrawIcon(tv, sx, sy, s2d.Textures.Texture_Target_fighter, s2d.Textures.Texture_Target_size, scolor.Value);

      //TVScreen2DImmediate.Draw_Circle(sx, sy, iconsize, 6, color.Value);
    }

    public static void DrawProjectile(TVScreen2DImmediate tv, Screen2D s2d, ActorInfo s)
    {
      COLOR scolor = s.Faction.Color;
      float sx = 0;
      float sy = 0;
      tv.Math_3DPointTo2D(s.GetGlobalPosition(), ref sx, ref sy);
      DrawIcon(tv, sx, sy, s2d.Textures.Texture_Target_projectile, s2d.Textures.Texture_Target_size, scolor.Value);
    }

    public static void DrawAddon(TVScreen2DImmediate tv, Screen2D s2d, ActorInfo s)
    {
      COLOR scolor = s.Faction.Color;
      float sx = 0;
      float sy = 0;
      tv.Math_3DPointTo2D(s.GetGlobalPosition(), ref sx, ref sy);
      DrawIcon(tv, sx, sy, s2d.Textures.Texture_Target_hardpoint, s2d.Textures.Texture_Target_size, scolor.Value);
    }

    private static readonly float[] xs = new float[8];
    private static readonly float[] ys = new float[8];
    public static void DrawShip(TVScreen2DImmediate tv, Screen2D s2d, Box bound, COLOR color)
    {
      tv.Math_3DPointTo2D(new TV_3DVECTOR(bound.X.Min, bound.Y.Min, bound.Z.Min), ref xs[0], ref ys[0]);
      tv.Math_3DPointTo2D(new TV_3DVECTOR(bound.X.Min, bound.Y.Min, bound.Z.Max), ref xs[1], ref ys[1]);
      tv.Math_3DPointTo2D(new TV_3DVECTOR(bound.X.Min, bound.Y.Max, bound.Z.Min), ref xs[2], ref ys[2]);
      tv.Math_3DPointTo2D(new TV_3DVECTOR(bound.X.Max, bound.Y.Min, bound.Z.Min), ref xs[3], ref ys[3]);
      tv.Math_3DPointTo2D(new TV_3DVECTOR(bound.X.Min, bound.Y.Max, bound.Z.Max), ref xs[4], ref ys[4]);
      tv.Math_3DPointTo2D(new TV_3DVECTOR(bound.X.Max, bound.Y.Min, bound.Z.Max), ref xs[5], ref ys[5]);
      tv.Math_3DPointTo2D(new TV_3DVECTOR(bound.X.Max, bound.Y.Max, bound.Z.Min), ref xs[6], ref ys[6]);
      tv.Math_3DPointTo2D(new TV_3DVECTOR(bound.X.Max, bound.Y.Max, bound.Z.Max), ref xs[7], ref ys[7]);

      float x0 = xs[0];
      float x1 = xs[0];
      float y0 = ys[0];
      float y1 = ys[0];
      for (int i = 1; i < 8; i++)
      {
        x0 = x0.Min(xs[i]);
        x1 = x1.Max(xs[i]);
        y0 = y0.Min(ys[i]);
        y1 = y1.Max(ys[i]);
      }

      float xd = x1 - x0;
      float yd = y1 - y0;
      if (xd < 32) { x0 = x0 + xd / 2 - 16; x1 = x1 - xd / 2 + 16; }
      if (yd < 32) { y0 = y0 + yd / 2 - 16; y1 = y1 - yd / 2 + 16; }

      tv.Draw_Box(x0, y0, x1, y1, color.Value);
    }

    private static readonly float[] xa = new float[8];
    private static readonly float[] ya = new float[8];
    public static void DrawActiveTarget(TVScreen2DImmediate tv, Screen2D s2d, Box bound, COLOR color)
    {
      tv.Math_3DPointTo2D(new TV_3DVECTOR(bound.X.Min, bound.Y.Min, bound.Z.Min), ref xa[0], ref ya[0]);
      tv.Math_3DPointTo2D(new TV_3DVECTOR(bound.X.Min, bound.Y.Min, bound.Z.Max), ref xa[1], ref ya[1]);
      tv.Math_3DPointTo2D(new TV_3DVECTOR(bound.X.Min, bound.Y.Max, bound.Z.Min), ref xa[2], ref ya[2]);
      tv.Math_3DPointTo2D(new TV_3DVECTOR(bound.X.Max, bound.Y.Min, bound.Z.Min), ref xa[3], ref ya[3]);
      tv.Math_3DPointTo2D(new TV_3DVECTOR(bound.X.Min, bound.Y.Max, bound.Z.Max), ref xa[4], ref ya[4]);
      tv.Math_3DPointTo2D(new TV_3DVECTOR(bound.X.Max, bound.Y.Min, bound.Z.Max), ref xa[5], ref ya[5]);
      tv.Math_3DPointTo2D(new TV_3DVECTOR(bound.X.Max, bound.Y.Max, bound.Z.Min), ref xa[6], ref ya[6]);
      tv.Math_3DPointTo2D(new TV_3DVECTOR(bound.X.Max, bound.Y.Max, bound.Z.Max), ref xa[7], ref ya[7]);

      float x0 = xa[0];
      float x1 = xa[0];
      float y0 = ya[0];
      float y1 = ya[0];
      for (int i = 1; i < 8; i++)
      {
        x0 = x0.Min(xa[i]);
        x1 = x1.Max(xa[i]);
        y0 = y0.Min(ya[i]);
        y1 = y1.Max(ya[i]);
      }

      float xd = x1 - x0;
      float yd = y1 - y0;
      if (xd < 32) { x0 = x0 + xd / 2 - 16; x1 = x1 - xd / 2 + 16; }
      if (yd < 32) { y0 = y0 + yd / 2 - 16; y1 = y1 - yd / 2 + 16; }

      Draw4Icons(tv, x0, y0, x1, y1
        , s2d.Textures.Texture_Target_ship_top_left
        , s2d.Textures.Texture_Target_ship_top_right
        , s2d.Textures.Texture_Target_ship_bottom_left
        , s2d.Textures.Texture_Target_ship_bottom_right
        , s2d.Textures.Texture_Target_size
        , color.Value);
    }


    private static void DrawIcon(TVScreen2DImmediate tv, float x, float y, int itex, int2 texSize, int icolor)
    {
      tv.Draw_Texture(itex
                         , x - texSize.x / 2
                         , y - texSize.y / 2
                         , x + texSize.x / 2
                         , y + texSize.y / 2
                         , icolor
                         , icolor
                         , icolor
                         , icolor);
    }

    private static void Draw4Icons(TVScreen2DImmediate tv, float x0, float y0, float x1, float y1, int itex00, int itex01, int itex10, int itex11, int2 texSize, int icolor)
    {
      tv.Draw_Texture(itex00
                         , x0
                         , y0
                         , x0 + texSize.x
                         , y0 + texSize.y
                         , icolor
                         , icolor
                         , icolor
                         , icolor);

      tv.Draw_Texture(itex01
                         , x1 - texSize.x
                         , y0
                         , x1
                         , y0 + texSize.y
                         , icolor
                         , icolor
                         , icolor
                         , icolor);

      tv.Draw_Texture(itex10
                         , x0
                         , y1 - texSize.y
                         , x0 + texSize.x
                         , y1
                         , icolor
                         , icolor
                         , icolor
                         , icolor);

      tv.Draw_Texture(itex11
                         , x1 - texSize.x
                         , y1 - texSize.y
                         , x1
                         , y1
                         , icolor
                         , icolor
                         , icolor
                         , icolor);
    }
  }
}
