using MTV3D65;
using SWEndor.Actors;
using Primrose.Primitives;
using System.IO;
using SWEndor.Actors.Models;
using SWEndor.UI;
using Primrose.Primitives.Geometry;
using Primrose.Primitives.Extensions;

namespace SWEndor.Core
{
  public class RenderSurfaces
  {
    public readonly Engine Engine;

    internal int Target_width;
    internal int Target_height;

    internal TVRenderSurface RS_PreTarget;
    internal TVRenderSurface RS_Target;
    internal TVRenderSurface RS_Target_Destroyed;
    internal int Tex_Target_Destroyed;
    internal TVRenderSurface RS_Target_Disabled;
    internal int Tex_Target_Disabled;
    internal TVRenderSurface RS_Target_Null;

    //private ActorTypeInfo _playerType;

    internal RenderSurfaces(Engine engine)
    {
      Engine = engine;
      Init();
      RenderOnce();
    }

    private void Init()
    {
      if (Engine.Settings.IsSmallResolution)
      {
        Target_width = 200;
        Target_height = 200;
      }
      else
      {
        Target_width = 256;
        Target_height = 256;
      }


      using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
      {
        RS_PreTarget = Engine.TrueVision.TVScene.CreateAlphaRenderSurface(Target_width, Target_height);
        RS_PreTarget.SetBackgroundColor(new TV_COLOR(0, 0, 0, 0).GetIntColor());
        RS_PreTarget.SetCamera(0, 0, 0, 0, 0, 100);
        RS_PreTarget.GetCamera().SetViewFrustum(60, 650000);

        RS_Target = Engine.TrueVision.TVScene.CreateAlphaRenderSurface(Target_width, Target_height);
        RS_Target.SetBackgroundColor(new TV_COLOR(0, 0, 0, 0.4f).GetIntColor());

        RS_Target_Destroyed = Engine.TrueVision.TVScene.CreateRenderSurface(Target_width, Target_height);
        RS_Target_Destroyed.SetBackgroundColor(new TV_COLOR(0, 0, 0, 0.8f).GetIntColor());

        RS_Target_Disabled = Engine.TrueVision.TVScene.CreateAlphaRenderSurface(Target_width, Target_height);
        RS_Target_Disabled.SetBackgroundColor(new TV_COLOR(0, 0, 0, 0.8f).GetIntColor());

        RS_Target_Null = Engine.TrueVision.TVScene.CreateAlphaRenderSurface(Target_width, Target_height);
        RS_Target_Null.SetBackgroundColor(new TV_COLOR(0, 0, 0, 0.8f).GetIntColor());
      }
    }

    internal void RenderOnce()
    {
      // RS_Target_Destroyed
      Tex_Target_Destroyed = Engine.TrueVision.TVTextureFactory.LoadTexture(Path.Combine(Globals.ImagePath, @"panel\broken.png"), "texTARGETER_DESTROYED");
      int w = Target_width;
      int h = Target_height;
      int warn_color = new TV_COLOR(1, 0, 0, 1).GetIntColor();
      int neu_color = new TV_COLOR(0.6f, 0.6f, 0.6f, 0.6f).GetIntColor();
      RS_Target_Destroyed.StartRender(false);
      Engine.TrueVision.TVScreen2DImmediate.Action_Begin2D();
      Engine.TrueVision.TVScreen2DImmediate.Draw_Texture(Tex_Target_Destroyed, 0, 0, w, h);
      Engine.TrueVision.TVScreen2DImmediate.Draw_Box(2, 2, w - 2, h - 2, warn_color);
      Engine.TrueVision.TVScreen2DImmediate.Action_End2D();
      RS_Target_Destroyed.EndRender();

      // RS_Target_Disabled
      Tex_Target_Disabled = Engine.TrueVision.TVTextureFactory.LoadTexture(Path.Combine(Globals.ImagePath, @"panel\disabled.png"), "texTARGETER_DISABLED");
      RS_Target_Disabled.StartRender(false);
      Engine.TrueVision.TVScreen2DImmediate.Action_Begin2D();
      Engine.TrueVision.TVScreen2DImmediate.Draw_Texture(Tex_Target_Disabled, 0, 0, w, h);
      Engine.TrueVision.TVScreen2DImmediate.Draw_Box(2, 2, w - 2, h - 2, neu_color);
      Engine.TrueVision.TVScreen2DImmediate.Action_End2D();
      RS_Target_Disabled.EndRender();

      // RS_Target_Null
      RS_Target_Null.StartRender(false);
      Engine.TrueVision.TVScreen2DImmediate.Action_Begin2D();
      Engine.TrueVision.TVScreen2DImmediate.Draw_Box(2, 2, w - 2, h - 2, neu_color);
      Engine.TrueVision.TVScreen2DImmediate.Action_End2D();
      RS_Target_Null.EndRender();
    }

    internal void Render()
    {
      // RS_PreTarget
      // RS_Target
      if (Engine.PlayerInfo.TargetActor != null 
        // TO-DO: Link the condition directly to Screen2D.Target
          && !Engine.Screen2D.ShowPage
          && Engine.PlayerInfo.Actor != null
          && !Engine.PlayerInfo.Actor.IsDyingOrDead
          && Engine.Screen2D.ShowUI)
      {
        ActorInfo t = Engine.PlayerInfo.TargetActor;
        t = t?.ParentForCoords ?? t;
        if (t != null)
          UpdateRenderLine(t);
      }
    }

    private void UpdateRenderLine(ActorInfo actor)
    {
      TV_3DVECTOR r = Engine.PlayerCameraInfo.Camera.GetRotation();
      Sphere sph = actor.GetBoundingSphere(false);
      TVCamera c = Engine.Surfaces.RS_PreTarget.GetCamera();
      c.SetRotation(r.x, r.y, r.z);
      c.SetPosition(sph.X, sph.Y, sph.Z);
      TV_3DVECTOR d2 = c.GetFrontPosition(-sph.R * 2.5f);
      c.SetPosition(d2.x, d2.y, d2.z);

      Engine.Surfaces.RS_PreTarget.StartRender(false);
      using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
        actor.Render(true);
      Engine.Surfaces.RS_PreTarget.EndRender();

      // post process:
      Engine.Surfaces.RS_Target.StartRender(false);
      int tex = Engine.Surfaces.RS_PreTarget.GetTexture();
      int icolor = actor.Faction.Color.Value;
      int w = Engine.Surfaces.Target_width;
      int h = Engine.Surfaces.Target_height;
      Engine.TrueVision.TVScreen2DImmediate.Action_Begin2D();
      Engine.TrueVision.TVScreen2DImmediate.Draw_Texture(tex
                                , 0
                                , 0
                                , w
                                , h
                                , icolor);

      Engine.TrueVision.TVScreen2DImmediate.Draw_Box(2, 2, w - 2, h - 2, icolor);
      Engine.TrueVision.TVScreen2DImmediate.Action_End2D();

      ActorInfo tp = actor.ParentForCoords ?? actor;
      int fntID = Engine.FontFactory.Get(Font.T12).ID;
      Engine.TrueVision.TVScreen2DText.Action_BeginText();
      // Name
      Engine.TrueVision.TVScreen2DText.TextureFont_DrawText(tp.Name
                                        , 10
                                        , 10
                                        , icolor
                                        , fntID);

      // Shields
      Engine.TrueVision.TVScreen2DText.TextureFont_DrawText("SHD"
                                              , 15
                                              , h - 45
                                              , icolor
                                              , fntID);

      Engine.TrueVision.TVScreen2DText.TextureFont_DrawText((tp.MaxShd == 0) ? "----" : "{0:0}%".F(tp.Shd_Perc)
                                              , 15 + 40
                                              , h - 45
                                              , ((tp.MaxShd == 0) ? new COLOR(1, 1, 1, 0.4f) : tp.Shd_Color).Value
                                              , fntID);

      // Hull
      Engine.TrueVision.TVScreen2DText.TextureFont_DrawText("HULL"
                                              , 15
                                              , h - 25
                                              , icolor
                                              , fntID);

      Engine.TrueVision.TVScreen2DText.TextureFont_DrawText((tp.MaxHull == 0) ? "100%" : "{0:0}%".F(tp.Hull_Perc)
                                              , 15 + 40
                                              , h - 25
                                              , ((tp.MaxHull == 0) ? new COLOR(0, 1, 0, 1) : tp.Hull_Color).Value
                                              , fntID);

      // Systems
      int i = 0;
      int maxpart = tp.TypeInfo.SystemData.Parts.Length;
      fntID = Engine.FontFactory.Get(Font.T08).ID;
      foreach (SystemPart part in tp.TypeInfo.SystemData.Parts)
      {
        SystemState s = tp.GetStatus(part);
        ColorLocalKeys k = s == SystemState.ACTIVE ? ColorLocalKeys.GAME_SYSTEMSTATE_ACTIVE :
                           s == SystemState.DISABLED ? ColorLocalKeys.GAME_SYSTEMSTATE_DISABLED :
                           s == SystemState.DESTROYED ? ColorLocalKeys.GAME_SYSTEMSTATE_DESTROYED :
                                                        ColorLocalKeys.GAME_SYSTEMSTATE_NULL;
        int scolor = ColorLocalization.Get(k).Value;

        Engine.TrueVision.TVScreen2DText.TextureFont_DrawText(part.GetShortName()
                                                      , w - 5 - 25 * (1 + i % 4)
                                                      , h - 5 - 12 * (1 + maxpart / 4 - i / 4)
                                                      , scolor
                                                      , fntID);
        i++;
      }

      Engine.TrueVision.TVScreen2DText.Action_EndText();
      Engine.Surfaces.RS_Target.EndRender();
    }
  }
}
