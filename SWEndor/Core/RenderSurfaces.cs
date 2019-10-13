﻿using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.Player;
using SWEndor.Primitives;
using System.IO;

namespace SWEndor.Core
{
  public class RenderSurfaces
  {
    public readonly Engine Engine;

    internal TVRenderSurface RS_PreTarget;
    internal TVRenderSurface RS_Target;
    internal TVRenderSurface RS_Target_Destroyed;
    internal int Tex_Target_Destroyed;
    internal TVRenderSurface RS_Target_Disabled;
    internal int Tex_Target_Disabled;
    internal TVRenderSurface RS_Target_Null;

    private ActorTypeInfo _playerType;

    internal RenderSurfaces(Engine engine)
    {
      Engine = engine;
      Init();
      RenderOnce();
    }

    private void Init()
    {
      using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
      {
        RS_PreTarget = Engine.TrueVision.TVScene.CreateAlphaRenderSurface(256, 256);
        RS_PreTarget.SetBackgroundColor(new TV_COLOR(0, 0, 0, 0).GetIntColor());
        RS_PreTarget.SetCamera(0, 0, 0, 0, 0, 100);
        RS_PreTarget.GetCamera().SetViewFrustum(60, 650000);

        RS_Target = Engine.TrueVision.TVScene.CreateAlphaRenderSurface(256, 256);
        RS_Target.SetBackgroundColor(new TV_COLOR(0, 0, 0, 0.4f).GetIntColor());
        RS_Target.SetCamera(0, 0, 0, 0, 0, 100);
        RS_Target.GetCamera().SetViewFrustum(60, 650000);

        RS_Target_Destroyed = Engine.TrueVision.TVScene.CreateAlphaRenderSurface(256, 256);
        RS_Target_Destroyed.SetBackgroundColor(new TV_COLOR(0, 0, 0, 0.8f).GetIntColor());
        RS_Target_Destroyed.SetCamera(0, 0, 0, 0, 0, 100);
        RS_Target_Destroyed.GetCamera().SetViewFrustum(60, 650000);

        RS_Target_Disabled = Engine.TrueVision.TVScene.CreateAlphaRenderSurface(256, 256);
        RS_Target_Disabled.SetBackgroundColor(new TV_COLOR(0, 0, 0, 0.8f).GetIntColor());
        RS_Target_Disabled.SetCamera(0, 0, 0, 0, 0, 100);
        RS_Target_Disabled.GetCamera().SetViewFrustum(60, 650000);

        RS_Target_Null = Engine.TrueVision.TVScene.CreateAlphaRenderSurface(256, 256);
        RS_Target_Null.SetBackgroundColor(new TV_COLOR(0, 0, 0, 0.8f).GetIntColor());
        RS_Target_Null.SetCamera(0, 0, 0, 0, 0, 100);
        RS_Target_Null.GetCamera().SetViewFrustum(60, 650000);
      }
    }

    internal void RenderOnce()
    {
      // RS_Target_Destroyed
      Tex_Target_Destroyed = Engine.TrueVision.TVTextureFactory.LoadTexture(Path.Combine(Globals.ImagePath, @"panel\broken.png"));
      int w = RS_Target_Destroyed.GetWidth();
      int h = RS_Target_Destroyed.GetHeight();
      RS_Target_Destroyed.StartRender(false);
      Engine.TrueVision.TVScreen2DImmediate.Action_Begin2D();
      Engine.TrueVision.TVScreen2DImmediate.Draw_Texture(Tex_Target_Destroyed, 0, 0, w, h);
      Engine.TrueVision.TVScreen2DImmediate.Draw_Box(2, 2, w - 2, h - 2, new TV_COLOR(1, 0, 0, 1).GetIntColor());
      Engine.TrueVision.TVScreen2DImmediate.Action_End2D();
      RS_Target_Destroyed.EndRender();

      Tex_Target_Disabled = Engine.TrueVision.TVTextureFactory.LoadTexture(Path.Combine(Globals.ImagePath, @"panel\disabled.png"));
      w = RS_Target_Disabled.GetWidth();
      h = RS_Target_Disabled.GetHeight();
      RS_Target_Disabled.StartRender(false);
      Engine.TrueVision.TVScreen2DImmediate.Action_Begin2D();
      Engine.TrueVision.TVScreen2DImmediate.Draw_Texture(Tex_Target_Disabled, 0, 0, w, h);
      Engine.TrueVision.TVScreen2DImmediate.Draw_Box(2, 2, w - 2, h - 2, new TV_COLOR(0.6f, 0.6f, 0.6f, 0.6f).GetIntColor());
      Engine.TrueVision.TVScreen2DImmediate.Action_End2D();
      RS_Target_Disabled.EndRender();

      w = RS_Target_Null.GetWidth();
      h = RS_Target_Null.GetHeight();
      RS_Target_Null.StartRender(false);
      Engine.TrueVision.TVScreen2DImmediate.Action_Begin2D();
      Engine.TrueVision.TVScreen2DImmediate.Draw_Box(2, 2, w - 2, h - 2, new TV_COLOR(0.6f, 0.6f, 0.6f, 0.6f).GetIntColor());
      Engine.TrueVision.TVScreen2DImmediate.Action_End2D();
      RS_Target_Null.EndRender();
    }

    internal void Render()
    {
      // RS_PreTarget
      // RS_Target
      if (Engine.PlayerInfo.TargetActor != null)
      {
        ActorInfo t = Engine.PlayerInfo.TargetActor;
        t = t?.ParentForCoords ?? t;
        if (t != null)
          t.UpdateRenderLine();
      }

      if (_playerType !=Engine.PlayerInfo.ActorType)
      {


        _playerType = Engine.PlayerInfo.ActorType;
      }

    }
  }
}
