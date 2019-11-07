using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using Primrose.Primitives;
using System.IO;

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
      Target_width = 256;
      Target_height = 256;

      using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
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
          t.UpdateRenderLine();
      }
    }
  }
}
