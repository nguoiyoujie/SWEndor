using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor
{
  public class Screen2D
  {
    private static Screen2D _instance;
    public static Screen2D Instance()
    {
      if (_instance == null) { _instance = new Screen2D(); }
      return _instance;
    }

    // fonts
    internal int FontID08 = -1;
    internal int FontID10 = -1;
    internal int FontID12 = -1;
    internal int FontID14 = -1;
    internal int FontID16 = -1;
    internal int FontID24 = -1;
    internal int FontID36 = -1;
    internal int FontID48 = -1;

    // PreRender Text
    public List<string> LoadingTextLines = new List<string>(); 

    // Page
    public bool ShowPage = false;
    public UIPage CurrentPage = null;

    // Message Text
    public string CenterText = "";
    public float CenterTextExpireTime = 0;
    public TV_COLOR CenterTextColor = new TV_COLOR(1, 1, 1, 1);

    // Radar
    //private TV_2DVECTOR radar_center = new TV_2DVECTOR(Engine.Instance().ScreenWidth / 2 - 150, -Engine.Instance().ScreenHeight / 2 + 350);
    private TV_2DVECTOR radar_center = new TV_2DVECTOR(0, Engine.Instance().ScreenHeight * 0.24f);

    private float radar_radius = Engine.Instance().ScreenHeight * 0.16f; //100;
    private float radar_range = 4000;
    private float radar_blinkfreq = 2.5f;
    private float radar_bigshiprenderunit = 50;
    private TV_2DVECTOR targetingradar_center = new TV_2DVECTOR(Engine.Instance().ScreenWidth / 2, Engine.Instance().ScreenHeight * 0.28f);
    private float targetingradar_radius = Engine.Instance().ScreenHeight * 0.12f; //100;
    public string TargetingRadar_text = "";
    public bool OverrideTargetingRadar = false;

    // Aim Target
    private ActorInfo m_target = null;
    private float m_targetX = 0;
    private float m_targetY = 0;
    private float m_targetSize = 5;
    private float m_targetBigSize = 0;

    // Right Info
    private TV_2DVECTOR bar_topleft = new TV_2DVECTOR(Engine.Instance().ScreenWidth * 0.85f - 5 , 25); //new TV_2DVECTOR(0, -150);
    private float bar_length = Engine.Instance().ScreenWidth * 0.15f;
    private float bar_height = 16;
    private float bar_barheight = 6;

    // Middle Info
    private float infomiddlegap = 15;
    private float infowidth_left = 160;
    private float infowidth_right = 160;
    private float infoheight = 20;
    private float infotop = 50;

    // Left Info
    private float leftinfo_left = 20;
    private float leftinfo_stagetop = 15;
    private float leftinfo_stageheight = 30;
    private float leftinfo_stagewidth = 260;
    private float leftinfo_weapontop = 50;
    private float leftinfo_weaponwidth = 95;
    private float leftinfo_weaponheight = 40;

    // Hit Info
    private float prevstrengthfrac = 0;

    // Steering 
    TV_2DVECTOR steering_position = new TV_2DVECTOR(10, 110);
    float steering_height = 60;
    float steering_width = 60;


    private Screen2D()
    {
      radar_center += new TV_2DVECTOR(Engine.Instance().ScreenWidth / 2, Engine.Instance().ScreenHeight / 2);
    }

    public void Draw()
    {
      if (ShowPage)
      {
        DrawPage();
      }
      else
      {
        if (PlayerInfo.Instance().Actor != null
            && PlayerInfo.Instance().Actor.ActorState != ActorState.DEAD
            && PlayerInfo.Instance().Actor.ActorState != ActorState.DYING
            && !(PlayerInfo.Instance().Actor.TypeInfo is InvisibleCameraATI))
        {
          if (PlayerInfo.Instance().ShowUI)
          {
            if (PlayerInfo.Instance().ShowStatus)
            {
              using (new PerfElement("render_2D_drawbars"))
                DrawBars();
              using (new PerfElement("render_2D_hitinfo"))
                DrawHitInfo();
            }
            if (PlayerInfo.Instance().ShowRadar)
            {
              if (OverrideTargetingRadar)
              {
                using (new PerfElement("render_2D_drawtargetingradar"))
                  DrawTargetingRadar();
              }
              else
              {
                using (new PerfElement("render_2D_drawradar"))
                  DrawRadar();
              }
            }
            if (PlayerInfo.Instance().ShowScore)
            {
              //DrawScore();
            }
            if (PlayerInfo.Instance().PlayerAIEnabled)
            {
              using (new PerfElement("render_2D_drawAIinfo"))
                DrawAIInfoText();
            }
            using (new PerfElement("render_2D_drawinfo"))
              DrawInfo();
            using (new PerfElement("render_2D_drawsteering"))
              DrawSteering();
            using (new PerfElement("render_2D_drawweaponinfo"))
              DrawWeaponInfo();
            using (new PerfElement("render_2D_drawcrosshair"))
              DrawCrossHair();
            using (new PerfElement("render_2D_drawtarget"))
              DrawTarget();
          }
        }
        if (PlayerInfo.Instance().ShowUI)
        {
          using (new PerfElement("render_2D_drawcentertext"))
            DrawCenterText();
        }
        DrawMouseLocation();
      }
      if (Settings.ShowPerformance)
      {
        using (new PerfElement("render_2D_drawperftext"))
          DrawPerfInfoText();
      }

      //DrawSelectedObjectInfoText();
      //DrawObjectInfoText();
      //DrawGeneralInfoText();
    }

    private void DrawPage()
    {
      if (CurrentPage != null)
        CurrentPage.Show();
    }

    public void UpdateText(string text, float expiretime, TV_COLOR color)
    {
      CenterText = text;
      CenterTextExpireTime = expiretime;
      CenterTextColor = color;
    }

    public void ClearText()
    {
      CenterText = "";
    }

    private void DrawCenterText()
    {
      if (CenterTextExpireTime < Game.Instance().GameTime)
        return;

      if (CenterText.Length == 0)
        return;

      float letter_size = 4.5f;

      Engine.Instance().TVScreen2DImmediate.Action_Begin2D();
      Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(Engine.Instance().ScreenWidth / 2 - 5 - letter_size * CenterText.Length
                                                         , Engine.Instance().ScreenHeight / 2 - 62
                                                         , Engine.Instance().ScreenWidth / 2 + 5 + letter_size * CenterText.Length
                                                         , Engine.Instance().ScreenHeight / 2 - 33
                                                         , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      Engine.Instance().TVScreen2DImmediate.Action_End2D();

      Engine.Instance().TVScreen2DText.Action_BeginText();
      Engine.Instance().TVScreen2DText.TextureFont_DrawText(CenterText
                                                            , Engine.Instance().ScreenWidth / 2 - letter_size * CenterText.Length
                                                            , Engine.Instance().ScreenHeight / 2 - 60
                                                            , CenterTextColor.GetIntColor()
                                                            , FontID12);
      Engine.Instance().TVScreen2DText.Action_EndText();
    }

    private void DrawMouseLocation()
    {
      Engine.Instance().TVScreen2DImmediate.Action_Begin2D();
      int mX = 0;
      int mY = 0;

      InputManager.Instance().INPUT_ENGINE.GetMousePosition(ref mX, ref mY);

      TV_COLOR color = new TV_COLOR(1, 1, 1, 0.5f);
      if (PlayerInfo.Instance().Actor != null && PlayerInfo.Instance().Actor.Faction != null)
        color = PlayerInfo.Instance().Actor.Faction.Color;

      Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(mX - 2, mY - 2, mX + 2, mY + 2, color.GetIntColor());
      //Engine.Instance().TVScreen2DImmediate.Draw_Line(mX + 2, mY - 2, mX - 2, mY + 2, color.GetIntColor());

      Engine.Instance().TVScreen2DImmediate.Action_End2D();
    }

    public string perftext = "";
    private void DrawPerfInfoText()
    {
      TV_2DVECTOR loc = new TV_2DVECTOR(10, 215);
      int lines = perftext.Split('\n').Length;

      Engine.Instance().TVScreen2DImmediate.Action_Begin2D();
      Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(loc.x - 5, loc.y - 5, loc.x + 405, loc.y + 40 / 3 * lines + 5, new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      Engine.Instance().TVScreen2DImmediate.Action_End2D();

      Engine.Instance().TVScreen2DText.Action_BeginText();
      Engine.Instance().TVScreen2DText.TextureFont_DrawText(perftext
        , loc.x, loc.y, new TV_COLOR(0.6f, 0.8f, 0.6f, 1).GetIntColor(), FontID08);
      Engine.Instance().TVScreen2DText.Action_EndText();
    }

    private void DrawAIInfoText()
    {
      ActorInfo p = PlayerInfo.Instance().Actor;
      if (p != null && p.CreationState == CreationState.ACTIVE)
      {
        TV_2DVECTOR loc = new TV_2DVECTOR(10, 175);

        ActionInfo action = p.CurrentAction;
        string actiontext = "";
        while (action != null)
        {
          actiontext += "\n" + action.ToString();
          action = action.NextAction;
        }

        int lines = actiontext.Split('\n').Length;

        Engine.Instance().TVScreen2DImmediate.Action_Begin2D();

        if (p.CurrentAction != null)
        {
          TV_3DVECTOR targetpos = new TV_3DVECTOR();
          if (p.CurrentAction is Actions.AttackActor)
          {
            targetpos = ((Actions.AttackActor)p.CurrentAction).Target_Position;
            TV_3DVECTOR targetactpos = ((Actions.AttackActor)p.CurrentAction).Target_Actor.GetPosition();
            //Engine.Instance().TVScreen2DImmediate.Draw_Line3D(p.Position.x, p.Position.y, p.Position.z, targetpos.x, targetpos.y, targetpos.z);
            Engine.Instance().TVScreen2DImmediate.Draw_Box3D(targetpos - new TV_3DVECTOR(25, 25, 25), targetpos + new TV_3DVECTOR(25, 25, 25), new TV_COLOR(1, 0.5f, 0.2f, 1).GetIntColor());
            Engine.Instance().TVScreen2DImmediate.Draw_Box3D(targetactpos - new TV_3DVECTOR(50, 50, 50), targetactpos + new TV_3DVECTOR(50, 50, 50), new TV_COLOR(1, 0.5f, 0.2f, 1).GetIntColor());
          }
          else if (p.CurrentAction is Actions.Move)
          {
            targetpos = ((Actions.Move)p.CurrentAction).Target_Position;
            Engine.Instance().TVScreen2DImmediate.Draw_Line3D(p.Position.x, p.Position.y, p.Position.z, targetpos.x, targetpos.y, targetpos.z);
          }
          else if (p.CurrentAction is Actions.ForcedMove)
          {
            targetpos = ((Actions.ForcedMove)p.CurrentAction).Target_Position;
            Engine.Instance().TVScreen2DImmediate.Draw_Line3D(p.Position.x, p.Position.y, p.Position.z, targetpos.x, targetpos.y, targetpos.z);
          }
          else if (p.CurrentAction is Actions.Rotate)
          {
            targetpos = ((Actions.Rotate)p.CurrentAction).Target_Position;
            Engine.Instance().TVScreen2DImmediate.Draw_Line3D(p.Position.x, p.Position.y, p.Position.z, targetpos.x, targetpos.y, targetpos.z);
          }
          else if (p.CurrentAction is Actions.FollowActor)
          {
            targetpos = ((Actions.FollowActor)p.CurrentAction).Target_Actor.GetPosition();
            Engine.Instance().TVScreen2DImmediate.Draw_Line3D(p.Position.x, p.Position.y, p.Position.z, targetpos.x, targetpos.y, targetpos.z);
          }
          else if (p.CurrentAction is Actions.Evade)
          {
            targetpos = ((Actions.Evade)p.CurrentAction).Target_Position;
            Engine.Instance().TVScreen2DImmediate.Draw_Line3D(p.Position.x, p.Position.y, p.Position.z, targetpos.x, targetpos.y, targetpos.z);
          }
          else if (p.CurrentAction is Actions.AvoidCollisionRotate)
          {
            targetpos = ((Actions.AvoidCollisionRotate)p.CurrentAction).Target_Position;
            Engine.Instance().TVScreen2DImmediate.Draw_Line3D(p.Position.x, p.Position.y, p.Position.z, targetpos.x, targetpos.y, targetpos.z);
          }

          TV_3DVECTOR prostart = p.GetRelativePositionXYZ(0, 0, p.TypeInfo.max_dimensions.z + 10);
          TV_3DVECTOR proend0 = p.GetRelativePositionXYZ(0, 0, p.TypeInfo.max_dimensions.z + 10 + p.ProspectiveCollisionScanDistance);

          Engine.Instance().TVScreen2DImmediate.Draw_Line3D(prostart.x
                                                          , prostart.y
                                                          , prostart.z
                                                          , proend0.x
                                                          , proend0.y
                                                          , proend0.z
                                                          , new TV_COLOR(1, 0.5f, 0.2f, 1).GetIntColor()
                                                          );

          /*
          if (p.IsInProspectiveCollision)
          {
            Engine.Instance().TVScreen2DImmediate.Draw_Line3D(prostart.x
                                                            , prostart.y
                                                            , prostart.z
                                                            , p.ProspectiveCollisionGoodLocation.x
                                                            , p.ProspectiveCollisionGoodLocation.y
                                                            , p.ProspectiveCollisionGoodLocation.z
                                                            , new TV_COLOR(0.5f, 1, 0.2f, 1).GetIntColor()
                                                            );
          }
          */
        }

        Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(loc.x - 5, loc.y - 5, loc.x + 405, loc.y + 40 / 3 * lines + 5, new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
        Engine.Instance().TVScreen2DImmediate.Action_End2D();

        Engine.Instance().TVScreen2DText.Action_BeginText();
        Engine.Instance().TVScreen2DText.TextureFont_DrawText(actiontext
          , loc.x, loc.y, new TV_COLOR(0.6f, 0.8f, 0.6f, 1).GetIntColor(), FontID08);
        Engine.Instance().TVScreen2DText.Action_EndText();
      }
    }


    private void DrawGeneralInfoText()
    {
      TV_2DVECTOR loc = new TV_2DVECTOR(30, 175);

      Engine.Instance().TVScreen2DText.Action_BeginText();
      TVCamera tvp = PlayerInfo.Instance().Camera;

      Engine.Instance().TVScreen2DText.TextureFont_DrawText(string.Format("POS: {0:0.00},{1:0.00},{2:0.00}\nROT: {3:0.00},{4:0.00},{5:0.00}\nTime/Loop: {6:0000.0000}\nTime: {7:0000.0000}",
          tvp.GetPosition().x,
          tvp.GetPosition().y,
          tvp.GetPosition().z,
          tvp.GetRotation().x,
          tvp.GetRotation().y,
          tvp.GetRotation().z,
          Game.Instance().TimeSinceRender,
          Game.Instance().GameTime
          )
        , loc.x, loc.y, new TV_COLOR(0.6f, 0.8f, 0.6f, 1).GetIntColor());

      Engine.Instance().TVScreen2DText.Action_EndText();
    }

    private void DrawObjectInfoText()
    {
      Engine.Instance().TVScreen2DText.Action_BeginText();

      TV_2DVECTOR loc = new TV_2DVECTOR(30, 375);
      string swingcount = "";
      Dictionary<string, int> wingcount = new Dictionary<string, int>();
      foreach (ActorInfo a in ActorFactory.Instance().GetActorList())
      {
        if (a != null && a.CreationState == CreationState.ACTIVE)
        {
          if (!wingcount.ContainsKey("All Objects"))
            wingcount.Add("All Objects", 1);
          else
            wingcount["All Objects"]++;
        }
        if (a != null && a.TypeInfo is ProjectileGroup && a.CreationState == CreationState.ACTIVE && a.Faction != null)
        {
          if (!wingcount.ContainsKey("Projectiles"))
            wingcount.Add("Projectiles", 1);
          else
            wingcount["Projectiles"]++;
        }
        if (a != null && (a.TypeInfo is FighterGroup || a.TypeInfo is TIEGroup) && a.CreationState == CreationState.ACTIVE && a.Faction != null)
        {
          if (!wingcount.ContainsKey(a.Faction.Name + " Wings"))
            wingcount.Add(a.Faction.Name + " Wings", 1);
          else
            wingcount[a.Faction.Name + " Wings"]++;
        }
      }
      foreach (KeyValuePair<string, int> kvp in wingcount)
      {
        swingcount += kvp.Key + ": " + kvp.Value + "\n";
      }
      Engine.Instance().TVScreen2DText.TextureFont_DrawText(swingcount
      , loc.x, loc.y, new TV_COLOR(0.6f, 0.8f, 0.6f, 1).GetIntColor());

        //, 930, 830, new TV_COLOR(0.8f, 0.8f, 0.2f, 1).GetIntColor(), FontID12);

      Engine.Instance().TVScreen2DText.Action_EndText();
    }

    private void DrawSelectedObjectInfoText()
    {
      Engine.Instance().TVScreen2DText.Action_BeginText();

      TVCollisionResult tvcres = Engine.Instance().TVScene.MousePick((int)Engine.Instance().ScreenWidth / 2, (int)Engine.Instance().ScreenHeight / 2);
      if (tvcres.GetCollisionMesh() != null)
      {
        int n = 0;
        if (int.TryParse(tvcres.GetCollisionMesh().GetMeshName(), out n))
        {
          ActorInfo a = ActorFactory.Instance().GetActor(n);

          if (a != null) //&& a.TypeInfo.CollisionEnabled)
          {
            TV_3DVECTOR vec = tvcres.GetCollisionImpact() - a.GetPosition();
            TV_3DVECTOR vvec = new TV_3DVECTOR();
            TV_3DVECTOR rot = a.GetRotation();
            Engine.Instance().TVMathLibrary.TVVec3Rotate(ref vvec, vec, -rot.y, rot.x, rot.z);

            /*
            string text = string.Format("{0}({1:0.0},{2:0.0},{3:0.0})\nPos: {4:0.0},{5:0.0},{6:0.0}\nSpd: {7: 0.0}\n{8}  {9:0.0},{10:0.0},{11:0.0}\n"
              , a.Name
              , vvec.x
              , vvec.y
              , vvec.z
              , a.GetPosition().x
              , a.GetPosition().y
              , a.GetPosition().z
              , a.Speed
              , a.AI.Master.CurrAI
              , a.AI.Master.Move_TargetPosition.x
              , a.AI.Master.Move_TargetPosition.y
              , a.AI.Master.Move_TargetPosition.z
              );
            foreach (AIElement ai in a.AI.Orders)
            {
              text += string.Format("\n{0}  ({1:0.0},{2:0.0},{3:0.0})"
                                    , ai.AIType
                                    , ai.TargetPosition.x
                                    , ai.TargetPosition.y
                                    , ai.TargetPosition.z
                                    );
            }
            
            Engine.Instance().TVScreen2DText.TextureFont_DrawText(text
          , 900, 400, new TV_COLOR(0.8f, 0.8f, 0.2f, 1).GetIntColor(), FontID12);
          */
          }
        }
      }
      Engine.Instance().TVScreen2DText.Action_EndText();
    }

    private void DrawTargetingRadar()
    {
      ActorInfo p = PlayerInfo.Instance().Actor;
      if (p != null && p.CreationState == CreationState.ACTIVE)
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
          timefactor += 1/ divisor;
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
        Engine.Instance().TVScreen2DText.TextureFont_DrawText(TargetingRadar_text, posX - letter_size * 8, top + 5, pcolor.GetIntColor(), FontID12);
        Engine.Instance().TVScreen2DText.Action_EndText();
      }
    }

    private void DrawRadar()
    {
      ActorInfo p = PlayerInfo.Instance().Actor;
      if (p != null && p.CreationState == CreationState.ACTIVE)
      {
        TV_COLOR pcolor = (p.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : p.Faction.Color;

        /*
        Engine.Instance().TVScreen2DText.Action_BeginText();
        Engine.Instance().TVScreen2DText.TextureFont_DrawText(string.Format("TOGGLE: {0} "
            , ((CONST_TV_KEY)InputManager.FunctionKeyMap.GetItem("g_ui_radar_toggle")).ToString().Replace("TV_KEY_", "").Replace("-1", "").PadLeft(1)
            )
          , radar_center.x - radar_radius - 5
          , radar_center.y - radar_radius - 5
          , pcolor.GetIntColor()
          , FontID08
          );
        Engine.Instance().TVScreen2DText.Action_EndText();
        */

        Engine.Instance().TVScreen2DImmediate.Action_Begin2D();
        Engine.Instance().TVScreen2DImmediate.Draw_FilledCircle(radar_center.x, radar_center.y, radar_radius + 2, 300, new TV_COLOR(0, 0, 0, 0.25f).GetIntColor());
        Engine.Instance().TVScreen2DImmediate.Draw_Circle(radar_center.x, radar_center.y, radar_radius + 2, 300, pcolor.GetIntColor());
        Engine.Instance().TVScreen2DImmediate.Draw_Circle(radar_center.x, radar_center.y, radar_radius - 2, 300, pcolor.GetIntColor());

        foreach (ActorInfo a in ActorFactory.Instance().GetActorList())
        {
          if (a.CreationState == CreationState.ACTIVE && a.TypeInfo.RadarSize > 0 && (a.TypeInfo.AlwaysShowInRadar || ActorDistanceInfo.GetRoughDistance(new TV_3DVECTOR(p.GetPosition().x, 0, p.GetPosition().z) , new TV_3DVECTOR(a.GetPosition().x, 0, a.GetPosition().z)) < radar_range * 2))
          {
            TV_COLOR acolor = (a.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : a.Faction.Color;

            if (a.TypeInfo is AddOnGroup)
            {
              TV_2DVECTOR temp = new TV_2DVECTOR(p.GetPosition().x, p.GetPosition().z) - new TV_2DVECTOR(a.GetPosition().x, a.GetPosition().z);
              float dist = Engine.Instance().TVMathLibrary.GetDistanceVec2D(new TV_2DVECTOR(), temp);
              float angl = Engine.Instance().TVMathLibrary.Direction2Ang(temp.x, temp.y);
              angl -= p.GetRotation().y;
              if (dist > radar_range)
              {
                dist = radar_range;
              }

              float x = radar_center.x - radar_radius * dist / radar_range * (float)Math.Sin(angl * Globals.PI / 180);
              float y = radar_center.y + radar_radius * dist / radar_range * (float)Math.Cos(angl * Globals.PI / 180);

              Engine.Instance().TVScreen2DImmediate.Draw_Circle(x, y, a.TypeInfo.RadarSize, 6, acolor.GetIntColor());
            }
            else if (a.TypeInfo is FighterGroup || a.TypeInfo is TIEGroup || a.TypeInfo is SurfaceTowerGroup)
            {
              TV_2DVECTOR temp = new TV_2DVECTOR(p.GetPosition().x, p.GetPosition().z) - new TV_2DVECTOR(a.GetPosition().x, a.GetPosition().z);
              float dist = Engine.Instance().TVMathLibrary.GetDistanceVec2D(new TV_2DVECTOR(), temp);
              float angl = Engine.Instance().TVMathLibrary.Direction2Ang(temp.x, temp.y);
              angl -= p.GetRotation().y;
              if (dist > radar_range)
              {
                dist = radar_range;
              }

              float x = radar_center.x - radar_radius * dist / radar_range * (float)Math.Sin(angl * Globals.PI / 180);
              float y = radar_center.y + radar_radius * dist / radar_range * (float)Math.Cos(angl * Globals.PI / 180);

              if (a.TypeInfo is LandoFalconATI || a.TypeInfo is WedgeXWingATI || a.TypeInfo is TIE_X1_ATI)
              {
                Engine.Instance().TVScreen2DImmediate.Draw_Box(x - a.TypeInfo.RadarSize - 1, y - a.TypeInfo.RadarSize - 1, x + a.TypeInfo.RadarSize + 1, y + a.TypeInfo.RadarSize + 1, acolor.GetIntColor());
                Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(x - a.TypeInfo.RadarSize, y - a.TypeInfo.RadarSize, x + a.TypeInfo.RadarSize, y + a.TypeInfo.RadarSize, acolor.GetIntColor());
              }
              else
              {
                Engine.Instance().TVScreen2DImmediate.Draw_FilledCircle(x, y, a.TypeInfo.RadarSize, 12, acolor.GetIntColor());
              }
            }
            else if (a.TypeInfo is WarshipGroup)
            {
              float gt = Game.Instance().GameTime * radar_blinkfreq;
              if (a.Strength / a.TypeInfo.MaxStrength > 0.1f || (gt - (int)gt > 0.5f))
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
                while (i < 4)
                {
                  float ang = a.GetRotation().y;
                  TV_2DVECTOR temp = new TV_2DVECTOR(p.GetPosition().x, p.GetPosition().z) - new TV_2DVECTOR(a.GetPosition().x, a.GetPosition().z);
                  temp -= new TV_2DVECTOR(bx * (float)Math.Cos(ang * Globals.PI / 180) + bz * (float)Math.Sin(ang * Globals.PI / 180),
                                          bz * (float)Math.Cos(ang * Globals.PI / 180) - bx * (float)Math.Sin(ang * Globals.PI / 180));
                  float dist = Engine.Instance().TVMathLibrary.GetDistanceVec2D(new TV_2DVECTOR(), temp);
                  float angl = Engine.Instance().TVMathLibrary.Direction2Ang(temp.x, temp.y);
                  angl -= p.GetRotation().y;
                  if (dist > radar_range)
                  {
                    dist = radar_range;
                    ts.Add(null);
                  }
                  float x = radar_center.x - radar_radius * dist / radar_range * (float)Math.Sin(angl * Globals.PI / 180);
                  float y = radar_center.y + radar_radius * dist / radar_range * (float)Math.Cos(angl * Globals.PI / 180);
                  ts.Add(new TV_2DVECTOR(x, y));

                  switch (i)
                  {
                    case 3:
                      bz -= radar_bigshiprenderunit;
                      if (bz < boxmin.z)
                      {
                        bz = boxmin.z;
                        i++;
                      }
                      break;
                    case 2:
                      bx -= radar_bigshiprenderunit;
                      if (bx < boxmin.x)
                      {
                        bx = boxmin.x;
                        i++;
                      }
                      break;
                    case 1:
                      bz += radar_bigshiprenderunit;
                      if (bz > boxmax.z)
                      {
                        bz = boxmax.z;
                        i++;
                      }
                      break;
                    case 0:
                      bx += radar_bigshiprenderunit;
                      if (bx > boxmax.x)
                      {
                        bx = boxmax.x;
                        i++;
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
              if (a.Strength / a.TypeInfo.MaxStrength > 0.1f || (gt - (int)gt > 0.5f))
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
                while (i < 3)
                {
                  float ang = a.GetRotation().y;
                  TV_2DVECTOR temp = new TV_2DVECTOR(p.GetPosition().x, p.GetPosition().z) - new TV_2DVECTOR(a.GetPosition().x, a.GetPosition().z);
                  temp -= new TV_2DVECTOR(bx * (float)Math.Cos(ang * Globals.PI / 180) + bz * (float)Math.Sin(ang * Globals.PI / 180),
                                          bz * (float)Math.Cos(ang * Globals.PI / 180) - bx * (float)Math.Sin(ang * Globals.PI / 180));
                  float dist = Engine.Instance().TVMathLibrary.GetDistanceVec2D(new TV_2DVECTOR(), temp);
                  float angl = Engine.Instance().TVMathLibrary.Direction2Ang(temp.x, temp.y);
                  angl -= p.GetRotation().y;
                  if (dist > radar_range)
                  {
                    dist = radar_range;
                    ts.Add(null);
                  }
                  float x = radar_center.x - radar_radius * dist / radar_range * (float)Math.Sin(angl * Globals.PI / 180);
                  float y = radar_center.y + radar_radius * dist / radar_range * (float)Math.Cos(angl * Globals.PI / 180);
                  ts.Add(new TV_2DVECTOR(x, y));

                  switch (i)
                  {
                    case 2:
                      bx -= radar_bigshiprenderunit;
                      if (bx <= boxmin.x)
                      {
                        bx = boxmin.x;
                        i++;
                      }
                      break;
                    case 1:
                      bz -= radar_bigshiprenderunit;
                      bx += radar_bigshiprenderunit * (boxmax.x - boxmin.x) / (boxmax.z - boxmin.z) / 2;
                      if (bz <= boxmin.z)
                      {
                        bz = boxmin.z;
                        i++;
                      }
                      break;
                    case 0:
                      bz += radar_bigshiprenderunit;
                      bx += radar_bigshiprenderunit * (boxmax.x - boxmin.x) / (boxmax.z - boxmin.z) / 2;
                      if (bz >= boxmax.z)
                      {
                        bz = boxmax.z;
                        i++;
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
              TV_2DVECTOR temp = new TV_2DVECTOR(p.GetPosition().x, p.GetPosition().z) - new TV_2DVECTOR(a.GetPosition().x, a.GetPosition().z);
              TV_2DVECTOR prevtemp = new TV_2DVECTOR(p.GetPosition().x, p.GetPosition().z) - new TV_2DVECTOR(a.PrevPosition.x, a.PrevPosition.z);
              float dist = Engine.Instance().TVMathLibrary.GetDistanceVec2D(new TV_2DVECTOR(), temp);
              float prevdist = Engine.Instance().TVMathLibrary.GetDistanceVec2D(new TV_2DVECTOR(), prevtemp);
              float angl = Engine.Instance().TVMathLibrary.Direction2Ang(temp.x, temp.y);
              float prevangl = Engine.Instance().TVMathLibrary.Direction2Ang(prevtemp.x, prevtemp.y);
              angl -= p.GetRotation().y;
              prevangl -= p.GetRotation().y;
              if (dist < radar_range && prevdist < radar_range)
              {
                float x = radar_center.x - radar_radius * dist / radar_range * (float)Math.Sin(angl * Globals.PI / 180);
                float y = radar_center.y + radar_radius * dist / radar_range * (float)Math.Cos(angl * Globals.PI / 180);

                float px = radar_center.x - radar_radius * prevdist / radar_range * (float)Math.Sin(prevangl * Globals.PI / 180);
                float py = radar_center.y + radar_radius * prevdist / radar_range * (float)Math.Cos(prevangl * Globals.PI / 180);

                Engine.Instance().TVScreen2DImmediate.Draw_Line(x, y, px, py, new TV_COLOR(0.8f, 0.5f, 0, 1).GetIntColor());
              }
            }
            else if (a.TypeInfo is ExplosionGroup)
            {
              TV_2DVECTOR temp = new TV_2DVECTOR(p.GetPosition().x, p.GetPosition().z) - new TV_2DVECTOR(a.GetPosition().x, a.GetPosition().z);
              float dist = Engine.Instance().TVMathLibrary.GetDistanceVec2D(new TV_2DVECTOR(), temp);
              float angl = Engine.Instance().TVMathLibrary.Direction2Ang(temp.x, temp.y);
              angl -= p.GetRotation().y;
              if (dist > radar_range)
              {
                dist = radar_range;
              }

              float x = radar_center.x - radar_radius * dist / radar_range * (float)Math.Sin(angl * Globals.PI / 180);
              float y = radar_center.y + radar_radius * dist / radar_range * (float)Math.Cos(angl * Globals.PI / 180);

              Engine.Instance().TVScreen2DImmediate.Draw_FilledCircle(x, y, a.TypeInfo.RadarSize, 36, new TV_COLOR(0.75f, 0.75f, 0, 1).GetIntColor());
            }
          }
        }
      }
      Engine.Instance().TVScreen2DImmediate.Action_End2D();
    }

    private bool PickTarget(bool pick_allies)
    {
      ActorInfo p = PlayerInfo.Instance().Actor;
      bool ret = false;

      if (p != null && p.CreationState == CreationState.ACTIVE)
      {
        // Attempt center pick
        TVCollisionResult tvcres = Engine.Instance().TVScene.MousePick((int)Engine.Instance().ScreenWidth / 2, (int)Engine.Instance().ScreenHeight / 2);
        if (tvcres.GetCollisionMesh() != null)
        {
          int n = 0;
          if (int.TryParse(tvcres.GetCollisionMesh().GetMeshName(), out n))
          {
            ActorInfo a = ActorFactory.Instance().GetActor(n);

            if (a != null
              && p != a
              && a.CreationState == CreationState.ACTIVE
              && a.ActorState != ActorState.DYING
              && a.ActorState != ActorState.DEAD
              && a.TypeInfo.IsSelectable
              && (pick_allies || !p.Faction.IsAlliedWith(a.Faction))
              && PlayerInfo.Instance().Camera.IsPointVisible(a.GetPosition())
              )
            {
              float dist = ActorDistanceInfo.GetDistance(p, a, 7501);
              if (dist < 7500)
              {
                float x = 0;
                float y = 0;
                float limit = 0.015f * dist;
                if (limit < 50)
                  limit = 50;
                m_targetX = limit;
                m_targetY = limit;
                Engine.Instance().TVScreen2DImmediate.Math_3DPointTo2D(a.GetPosition(), ref x, ref y);

                x -= Engine.Instance().ScreenWidth / 2;
                y -= Engine.Instance().ScreenHeight / 2;

                if (Math.Abs(x) < limit && Math.Abs(y) < limit && (Math.Abs(x) + Math.Abs(y)) < (Math.Abs(m_targetX) + Math.Abs(m_targetY)))
                {
                  m_target = a;
                  m_targetX = x;
                  m_targetY = y;
                  ret = true;
                }
              }
            }
          }
        }

        if (m_target == null)
        {
          // Attempt close enough
          foreach (ActorInfo a in ActorFactory.Instance().GetActorList())
          {
            if (a != null
              && p != a
              && a.CreationState == CreationState.ACTIVE
              && a.ActorState != ActorState.DYING
              && a.ActorState != ActorState.DEAD
              && a.TypeInfo.IsSelectable
              && (pick_allies || !p.Faction.IsAlliedWith(a.Faction))
              && PlayerInfo.Instance().Camera.IsPointVisible(a.GetPosition())
              )
            {
              float dist = ActorDistanceInfo.GetDistance(p, a, 7501);
              if (dist < 7500)
              {
                float x = 0;
                float y = 0;
                float limit = 0.015f * dist;
                if (limit < 50)
                  limit = 50;
                m_targetX = limit;
                m_targetY = limit;
                Engine.Instance().TVScreen2DImmediate.Math_3DPointTo2D(a.GetPosition(), ref x, ref y);

                x -= Engine.Instance().ScreenWidth / 2;
                y -= Engine.Instance().ScreenHeight / 2;

                if (Math.Abs(x) < limit && Math.Abs(y) < limit && (Math.Abs(x) + Math.Abs(y)) < (Math.Abs(m_targetX) + Math.Abs(m_targetY)))
                {
                  m_target = a;
                  m_targetX = x;
                  m_targetY = y;
                  ret = true;
                }
              }
            }
          }
        }
      }
      return ret;
    }

    private void DrawTarget()
    {
      ActorInfo p = PlayerInfo.Instance().Actor;
      ActorInfo prev_target = m_target;

      if (p != null && p.CreationState == CreationState.ACTIVE)
      {
        PickTarget(!PlayerInfo.Instance().IsTorpedoMode);

        if (m_target == null)
        {
          PlayerInfo.Instance().AimTarget = null;
        }
        else
        {
          float x = 0;
          float y = 0;
          Engine.Instance().TVScreen2DImmediate.Math_3DPointTo2D(m_target.GetPosition(), ref x, ref y);
          float dist = ActorDistanceInfo.GetDistance(p, m_target, 7501);
          float limit = 0.005f * dist;
          if (limit < 50)
            limit = 50;

          if (m_target.CreationState != CreationState.ACTIVE
          || m_target.ActorState == ActorState.DYING
          || m_target.ActorState == ActorState.DEAD
          || !m_target.IsCombatObject
          || dist > 7500
          || Math.Abs(x - Engine.Instance().ScreenWidth / 2) > limit
          || Math.Abs(y - Engine.Instance().ScreenHeight / 2) > limit
          || (PlayerInfo.Instance().Actor.Faction.IsAlliedWith(m_target.Faction) && PlayerInfo.Instance().IsTorpedoMode)
          || !PlayerInfo.Instance().Camera.IsPointVisible(m_target.GetPosition()))
          {
            m_target = null;
            PlayerInfo.Instance().AimTarget = null;
          }
          else
          {
            TV_COLOR acolor = (m_target.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : m_target.Faction.Color;
            string name = m_target.Name;
            Engine.Instance().TVScreen2DImmediate.Action_Begin2D();
            if (PlayerInfo.Instance().IsTorpedoMode)
            {
              if (!PlayerInfo.Instance().Actor.Faction.IsAlliedWith(m_target.Faction) && prev_target != m_target)
              {
                SoundManager.Instance().SetSound("Button_3");
                m_targetBigSize = 15;
              }
              if (m_targetBigSize > m_targetSize)
              {
                m_targetBigSize -= 25 * Game.Instance().TimeSinceRender;
                Engine.Instance().TVScreen2DImmediate.Draw_Box(x - m_targetBigSize, y - m_targetBigSize, x + m_targetBigSize, y + m_targetBigSize, acolor.GetIntColor());
              }

              WeaponInfo weap;
              int burst = 0;
              p.TypeInfo.InterpretWeapon(p, PlayerInfo.Instance().SecondaryWeapon, out weap, out burst);
              if (weap != null && weap.Ammo > 0)
              {
                Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(x - m_targetSize, y - m_targetSize, x + m_targetSize, y + m_targetSize, acolor.GetIntColor());
              }
              else
              {
                Engine.Instance().TVScreen2DImmediate.Draw_Box(x - m_targetSize, y - m_targetSize, x + m_targetSize, y + m_targetSize, acolor.GetIntColor());
              }
            }
            else
            {
              Engine.Instance().TVScreen2DImmediate.Draw_Box(x - m_targetSize, y - m_targetSize, x + m_targetSize, y + m_targetSize, acolor.GetIntColor());
            }
            Engine.Instance().TVScreen2DImmediate.Action_End2D();

            Engine.Instance().TVScreen2DText.Action_BeginText();
            Engine.Instance().TVScreen2DText.TextureFont_DrawText(string.Format("{0}\nDamage: {1:0}%"
              , name
              , (int)(100 * (1 - m_target.StrengthFrac))
              )
              , x, y + m_targetSize + 10, acolor.GetIntColor()
              , FontID10
              );
            Engine.Instance().TVScreen2DText.Action_EndText();

            PlayerInfo.Instance().AimTarget = PlayerInfo.Instance().Actor.Faction.IsAlliedWith(m_target.Faction) ? null : m_target;

            if (PlayerInfo.Instance().Actor.Faction != null && !PlayerInfo.Instance().Actor.Faction.IsAlliedWith(m_target.Faction) && !PlayerInfo.Instance().IsTorpedoMode)
            {
              // Targeting cross
              // Anticipate
              float d = dist / Globals.LaserSpeed; // Laser Speed
              TV_3DVECTOR target = m_target.GetRelativePositionXYZ(0, 0, m_target.Speed * d);
              Engine.Instance().TVScreen2DImmediate.Math_3DPointTo2D(target, ref x, ref y);

              Engine.Instance().TVScreen2DImmediate.Action_Begin2D();
              Engine.Instance().TVScreen2DImmediate.Draw_Line(x - m_targetSize, y, x + m_targetSize, y, acolor.GetIntColor());
              Engine.Instance().TVScreen2DImmediate.Draw_Line(x, y - m_targetSize, x, y + m_targetSize, acolor.GetIntColor());
              Engine.Instance().TVScreen2DImmediate.Action_End2D();
            }
          }
        }
      }
    }

    private void DrawSteering()
    {
      ActorInfo p = PlayerInfo.Instance().Actor;

      if (p != null && p.CreationState == CreationState.ACTIVE)
      {
        TV_COLOR pcolor = (p.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : p.Faction.Color;

        Engine.Instance().TVScreen2DImmediate.Action_Begin2D();

        Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(steering_position.x
                                                     , steering_position.y
                                                     , steering_position.x + steering_width
                                                     , steering_position.y + steering_height
                                                     , new TV_COLOR(0, 0, 0, 0.25f).GetIntColor());

        Engine.Instance().TVScreen2DImmediate.Draw_Box(steering_position.x
                                                     , steering_position.y
                                                     , steering_position.x + steering_width
                                                     , steering_position.y + steering_height
                                                     , pcolor.GetIntColor());

        Engine.Instance().TVScreen2DImmediate.Draw_Line(steering_position.x + steering_width / 2
                                                     , steering_position.y
                                                     , steering_position.x + steering_width / 2
                                                     , steering_position.y + steering_height
                                                     , pcolor.GetIntColor());

        Engine.Instance().TVScreen2DImmediate.Draw_Line(steering_position.x
                                                     , steering_position.y + steering_height / 2
                                                     , steering_position.x + steering_width 
                                                     , steering_position.y + steering_height / 2
                                                     , pcolor.GetIntColor());

        float xfrac = p.YTurnAngle / p.MaxTurnRate / 2;
        float yfrac = p.XTurnAngle / p.MaxTurnRate / 2;
        float size = 3;

        Engine.Instance().TVScreen2DImmediate.Draw_Box(steering_position.x + steering_width * (xfrac + 0.5f) - size
                                                     , steering_position.y + steering_height * (yfrac + 0.5f) - size
                                                     , steering_position.x + steering_width * (xfrac + 0.5f) + size
                                                     , steering_position.y + steering_height * (yfrac + 0.5f) + size
                                                     , pcolor.GetIntColor());

        Engine.Instance().TVScreen2DImmediate.Action_End2D();
      }
    }

    private void DrawCrossHair()
    {
      ActorInfo p = PlayerInfo.Instance().Actor;

      if (p != null && p.CreationState == CreationState.ACTIVE)
      {
        WeaponInfo weap = null;
        int burst = 1;
        TV_COLOR pcolor = (p.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : p.Faction.Color;

        p.TypeInfo.InterpretWeapon(p, PlayerInfo.Instance().PrimaryWeapon, out weap, out burst);
        if (weap != null)
        {
          Engine.Instance().TVScreen2DImmediate.Action_Begin2D();

          float xs = 1;
          float ys = 1.5f;
          float k = 16;
          float l = 4;

          for (int i = 0; i < weap.FirePositions.Length; i++)
          {
            TV_2DVECTOR vec0 = new TV_2DVECTOR(weap.FirePositions[i].x * xs, -weap.FirePositions[i].y * ys);
            if (vec0.x < -10) vec0.x = -10;
            if (vec0.x > 10) vec0.x = 10;
            if (vec0.y < -10) vec0.y = -10;
            if (vec0.y > 10) vec0.y = 10;

            TV_2DVECTOR vec1 = new TV_2DVECTOR();
            TV_2DVECTOR vec2 = new TV_2DVECTOR();
            int wpi = i - weap.CurrentPositionIndex;
            if (wpi < 0)
              wpi += weap.FirePositions.Length;
            bool highlighted = (wpi >= 0 && wpi < burst);
            if (vec0.x != 0)
            {
              float m = vec0.y / vec0.x;
              if (m < 0) { m = -m; }
              vec1 = new TV_2DVECTOR(vec0.x + (vec0.x > 0 ? 1 : -1) * (k + l * m) / (1 + m)
                                   , vec0.y + (vec0.y > 0 ? 1 : -1) * (k * m - l) / (1 + m));
              vec2 = new TV_2DVECTOR(vec0.x + (vec0.x > 0 ? 1 : -1) * (k -  l * m) / (1 + m)
                                   , vec0.y + (vec0.y > 0 ? 1 : -1) * (k * m + l) / (1 + m));


              Engine.Instance().TVScreen2DImmediate.Draw_Triangle(vec0.x + Engine.Instance().ScreenWidth / 2
                                                    , vec0.y + Engine.Instance().ScreenHeight / 2
                                                    , vec1.x + Engine.Instance().ScreenWidth / 2
                                                    , vec1.y + Engine.Instance().ScreenHeight / 2
                                                    , vec2.x + Engine.Instance().ScreenWidth / 2
                                                    , vec2.y + Engine.Instance().ScreenHeight / 2
                                                    , highlighted ? new TV_COLOR(1, 0.5f, 0, 1).GetIntColor() : pcolor.GetIntColor());
            }
            else
            {
              Engine.Instance().TVScreen2DImmediate.Draw_Line(vec0.x + l + Engine.Instance().ScreenWidth / 2
                                                            , vec0.y + Engine.Instance().ScreenHeight / 2
                                                            , vec0.x + Engine.Instance().ScreenWidth / 2
                                                            , vec0.y + l + Engine.Instance().ScreenHeight / 2
                                                            , highlighted ? new TV_COLOR(1, 0.5f, 0, 1).GetIntColor() : pcolor.GetIntColor());

              Engine.Instance().TVScreen2DImmediate.Draw_Line(vec0.x - l + Engine.Instance().ScreenWidth / 2
                                                            , vec0.y + Engine.Instance().ScreenHeight / 2
                                                            , vec0.x + Engine.Instance().ScreenWidth / 2
                                                            , vec0.y + l + Engine.Instance().ScreenHeight / 2
                                                            , highlighted ? new TV_COLOR(1, 0.5f, 0, 1).GetIntColor() : pcolor.GetIntColor());

              Engine.Instance().TVScreen2DImmediate.Draw_Line(vec0.x + l + Engine.Instance().ScreenWidth / 2
                                                            , vec0.y + Engine.Instance().ScreenHeight / 2
                                                            , vec0.x + Engine.Instance().ScreenWidth / 2
                                                            , vec0.y - l + Engine.Instance().ScreenHeight / 2
                                                            , highlighted ? new TV_COLOR(1, 0.5f, 0, 1).GetIntColor() : pcolor.GetIntColor());

              Engine.Instance().TVScreen2DImmediate.Draw_Line(vec0.x - l + Engine.Instance().ScreenWidth / 2
                                                            , vec0.y + Engine.Instance().ScreenHeight / 2
                                                            , vec0.x + Engine.Instance().ScreenWidth / 2
                                                            , vec0.y - l + Engine.Instance().ScreenHeight / 2
                                                            , highlighted ? new TV_COLOR(1, 0.5f, 0, 1).GetIntColor() : pcolor.GetIntColor());

            }
          }
          Engine.Instance().TVScreen2DImmediate.Action_End2D();
        }

        p.TypeInfo.InterpretWeapon(p, PlayerInfo.Instance().SecondaryWeapon, out weap, out burst);
        if (weap != null)
        {
          if (PlayerInfo.Instance().SecondaryWeapon.Contains("torp"))
          {
            float p1_x = -40;
            float p1_y = 28;
            float p2_x = -36;
            float p2_y = 36;
            float p3_x = 6;
            float p3_y = 10;
            float tremain = weap.Ammo;
            float tmax = weap.MaxAmmo;
            int t = 0;

            while (t < tremain || t < tmax)
            {
              if (t < tremain)
              {
                Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(p1_x + Engine.Instance().ScreenWidth / 2
                                                      , p1_y + Engine.Instance().ScreenHeight / 2
                                                      , p2_x + Engine.Instance().ScreenWidth / 2
                                                      , p2_y + Engine.Instance().ScreenHeight / 2
                                                      , pcolor.GetIntColor());
              }
              else
              {
                Engine.Instance().TVScreen2DImmediate.Draw_Box(p1_x + Engine.Instance().ScreenWidth / 2
                                      , p1_y + Engine.Instance().ScreenHeight / 2
                                      , p2_x + Engine.Instance().ScreenWidth / 2
                                      , p2_y + Engine.Instance().ScreenHeight / 2
                                      , pcolor.GetIntColor());
              }

              p1_x += p3_x;
              p2_x += p3_x;

              t++;
              if (t % 10 == 0)
              {
                p1_x = 19;
                p2_x = 23;
                p1_y += p3_y;
                p2_y += p3_y;
              }
            }
          }
          else if (weap.MaxAmmo > 0)
          {
            float p1_x = -25;
            float p1_y = 28;
            float p2_x = 25;
            float p2_y = 33;
            float tremain = (float)weap.Ammo / weap.MaxAmmo;

            Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(p1_x + Engine.Instance().ScreenWidth / 2
                                                      , p1_y + Engine.Instance().ScreenHeight / 2
                                                      , p1_x + (p2_x - p1_x) * tremain + Engine.Instance().ScreenWidth / 2
                                                      , p2_y + Engine.Instance().ScreenHeight / 2
                                                      , new TV_COLOR(0.6f, 0.6f, 1, 1).GetIntColor());

            Engine.Instance().TVScreen2DImmediate.Draw_Box(p1_x + Engine.Instance().ScreenWidth / 2
                                                      , p1_y + Engine.Instance().ScreenHeight / 2
                                                      , p2_x + Engine.Instance().ScreenWidth / 2
                                                      , p2_y + Engine.Instance().ScreenHeight / 2
                                                      , new TV_COLOR(1, 0.5f, 0, 1).GetIntColor());
          }
          Engine.Instance().TVScreen2DImmediate.Action_End2D();
        }


        /*
        float p1_x = 5;
        float p1_y = 5;
        float p2_x = 22;
        float p2_y = 8;
        float p3_x = 16;
        float p3_y = 14;
        TV_COLOR pcolor = (p.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : p.Faction.Color;

        Engine.Instance().TVScreen2DImmediate.Action_Begin2D();
        if (p.TypeInfo is XWingATI)
        {
          p1_x = 5;
          p1_y = 5;
          p2_x = 22;
          p2_y = 8;
          p3_x = 16;
          p3_y = 14;

          for (int i = -1; i <= 1; i += 2)
          {
            for (int j = -1; j <= 1; j += 2)
            {
              int n = 0;
              if (i > 0)
              {
                if (j > 0)
                {
                  n = 2;
                }
                else
                {
                  n = 0;
                }
              }
              else
              {
                if (j > 0)
                {
                  n = 1;
                }
                else
                {
                  n = 3;
                }
              }

              Engine.Instance().TVScreen2DImmediate.Draw_Triangle(p1_x * i + Engine.Instance().ScreenWidth / 2
                                                                  , p1_y * j + Engine.Instance().ScreenHeight / 2
                                                                  , p2_x * i + Engine.Instance().ScreenWidth / 2
                                                                  , p2_y * j + Engine.Instance().ScreenHeight / 2
                                                                  , p3_x * i + Engine.Instance().ScreenWidth / 2
                                                                  , p3_y * j + Engine.Instance().ScreenHeight / 2
                                                                  , (p.TypeInfo.PlayerInfo.Instance().PrimaryWeapon.IsStateFDefined("LaserPosition") && p.GetStateF("LaserPosition") == n) ? new TV_COLOR(1, 0.5f, 0, 1).GetIntColor() : pcolor.GetIntColor());
            }
          }
        }
        else if (p.TypeInfo is YWingATI)
        {
          p1_x = 0;
          p1_y = 0;
          p2_x = 6;
          p2_y = 12;
          p3_x = -6;
          p3_y = 12;

          Engine.Instance().TVScreen2DImmediate.Draw_Triangle(p1_x + Engine.Instance().ScreenWidth / 2
                                                              , p1_y + Engine.Instance().ScreenHeight / 2
                                                              , p2_x + Engine.Instance().ScreenWidth / 2
                                                              , p2_y + Engine.Instance().ScreenHeight / 2
                                                              , p3_x + Engine.Instance().ScreenWidth / 2
                                                              , p3_y + Engine.Instance().ScreenHeight / 2
                                                              , new TV_COLOR(1, 0.5f, 0, 1).GetIntColor());

          Engine.Instance().TVScreen2DImmediate.Draw_Line(0 + Engine.Instance().ScreenWidth / 2
                                                              , 0 + Engine.Instance().ScreenHeight / 2
                                                              , 0 + Engine.Instance().ScreenWidth / 2
                                                              , -25 + Engine.Instance().ScreenHeight / 2
                                                              , new TV_COLOR(1, 0.5f, 0, 1).GetIntColor());

          Engine.Instance().TVScreen2DImmediate.Draw_Line(-10 + Engine.Instance().ScreenWidth / 2
                                                              , -25 + Engine.Instance().ScreenHeight / 2
                                                              , 10 + Engine.Instance().ScreenWidth / 2
                                                              , -25 + Engine.Instance().ScreenHeight / 2
                                                              , new TV_COLOR(1, 0.5f, 0, 1).GetIntColor());


          Engine.Instance().TVScreen2DImmediate.Draw_Line(0 + Engine.Instance().ScreenWidth / 2
                                                              , 12 + Engine.Instance().ScreenHeight / 2
                                                              , 0 + Engine.Instance().ScreenWidth / 2
                                                              , 25 + Engine.Instance().ScreenHeight / 2
                                                              , new TV_COLOR(1, 0.5f, 0, 1).GetIntColor());

          Engine.Instance().TVScreen2DImmediate.Draw_Line(-25 + Engine.Instance().ScreenWidth / 2
                                                              , 25 + Engine.Instance().ScreenHeight / 2
                                                              , 25 + Engine.Instance().ScreenWidth / 2
                                                              , 25 + Engine.Instance().ScreenHeight / 2
                                                              , new TV_COLOR(1, 0.5f, 0, 1).GetIntColor());

          if ((PlayerInfo.Instance().PrimaryWeapon.Contains("photon") || PlayerInfo.Instance().SecondaryWeapon.Contains("photon")) && p.GetStateB("EnablePhoton"))
          {
            p1_x = -25;
            p1_y = 28;
            p2_x = 25;
            p2_y = 33;

            float tremain = p.GetStateF("PhotonRemaining") / p.GetStateF("PhotonMax");


            Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(p1_x + Engine.Instance().ScreenWidth / 2
                                                      , p1_y + Engine.Instance().ScreenHeight / 2
                                                      , p1_x + (p2_x - p1_x) * tremain + Engine.Instance().ScreenWidth / 2
                                                      , p2_y + Engine.Instance().ScreenHeight / 2
                                                      , new TV_COLOR(0.6f, 0.6f, 1, 1).GetIntColor());

            Engine.Instance().TVScreen2DImmediate.Draw_Box(p1_x + Engine.Instance().ScreenWidth / 2
                                                      , p1_y + Engine.Instance().ScreenHeight / 2
                                                      , p2_x + Engine.Instance().ScreenWidth / 2
                                                      , p2_y + Engine.Instance().ScreenHeight / 2
                                                      , new TV_COLOR(1, 0.5f, 0, 1).GetIntColor());
          }
        }
        if (p.TypeInfo is AWingATI)
        {
          p1_x = 10;
          p1_y = 0;
          p2_x = 20;
          p2_y = 5;
          p3_x = 20;
          p3_y = -5;

          for (int i = -1; i <= 1; i += 2)
          {
            int n = 0;
            if (i > 0)
            {
              n = 0;
            }
            else
            {
              n = 2;
            }

            Engine.Instance().TVScreen2DImmediate.Draw_Triangle(p1_x * i + Engine.Instance().ScreenWidth / 2
                                                                , p1_y + Engine.Instance().ScreenHeight / 2
                                                                , p2_x * i + Engine.Instance().ScreenWidth / 2
                                                                , p2_y + Engine.Instance().ScreenHeight / 2
                                                                , p3_x * i + Engine.Instance().ScreenWidth / 2
                                                                , p3_y + Engine.Instance().ScreenHeight / 2
                                                                , (p.IsStateFDefined("LaserPosition") && p.GetStateF("LaserPosition") == n) ? new TV_COLOR(1, 0.5f, 0, 1).GetIntColor() : pcolor.GetIntColor());

            Engine.Instance().TVScreen2DImmediate.Draw_Circle(Engine.Instance().ScreenWidth / 2
                                                                , Engine.Instance().ScreenHeight / 2
                                                                , 5
                                                                , 24
                                                                , (p.IsStateFDefined("LaserPosition") && p.GetStateF("LaserPosition") != 0 && p.GetStateF("LaserPosition") != 2) ? new TV_COLOR(1, 0.5f, 0, 1).GetIntColor() : pcolor.GetIntColor());

          }
        }
        if (p.TypeInfo is BWingATI)
        {
          p1_x = 10;
          p1_y = 0;
          p2_x = 22;
          p2_y = 5;
          p3_x = 22;
          p3_y = -5;

          Engine.Instance().TVScreen2DImmediate.Draw_Triangle(p1_x + Engine.Instance().ScreenWidth / 2
                                                              , p1_y + Engine.Instance().ScreenHeight / 2
                                                              , p2_x + Engine.Instance().ScreenWidth / 2
                                                              , p2_y + Engine.Instance().ScreenHeight / 2
                                                              , p3_x + Engine.Instance().ScreenWidth / 2
                                                              , p3_y + Engine.Instance().ScreenHeight / 2
                                                              , (p.IsStateFDefined("LaserPosition") && p.GetStateF("LaserPosition") == 3) ? new TV_COLOR(1, 0.5f, 0, 1).GetIntColor() : pcolor.GetIntColor());

          Engine.Instance().TVScreen2DImmediate.Draw_Triangle(-p1_x + Engine.Instance().ScreenWidth / 2
                                                              , -p1_y + Engine.Instance().ScreenHeight / 2
                                                              , -p2_x + Engine.Instance().ScreenWidth / 2
                                                              , -p2_y + Engine.Instance().ScreenHeight / 2
                                                              , -p3_x + Engine.Instance().ScreenWidth / 2
                                                              , -p3_y + Engine.Instance().ScreenHeight / 2
                                                              , (p.IsStateFDefined("LaserPosition") && p.GetStateF("LaserPosition") == 2) ? new TV_COLOR(1, 0.5f, 0, 1).GetIntColor() : pcolor.GetIntColor());

          Engine.Instance().TVScreen2DImmediate.Draw_Triangle(p1_y + Engine.Instance().ScreenWidth / 2
                                                              , p1_x + Engine.Instance().ScreenHeight / 2
                                                              , p2_y + Engine.Instance().ScreenWidth / 2
                                                              , p2_x + Engine.Instance().ScreenHeight / 2
                                                              , p3_y + Engine.Instance().ScreenWidth / 2
                                                              , p3_x + Engine.Instance().ScreenHeight / 2
                                                              , (p.IsStateFDefined("LaserPosition") && p.GetStateF("LaserPosition") == 0) ? new TV_COLOR(1, 0.5f, 0, 1).GetIntColor() : pcolor.GetIntColor());

          Engine.Instance().TVScreen2DImmediate.Draw_Triangle(-p1_y + Engine.Instance().ScreenWidth / 2
                                                              , -p1_x + Engine.Instance().ScreenHeight / 2
                                                              , -p2_y + Engine.Instance().ScreenWidth / 2
                                                              , -p2_x + Engine.Instance().ScreenHeight / 2
                                                              , -p3_y + Engine.Instance().ScreenWidth / 2
                                                              , -p3_x + Engine.Instance().ScreenHeight / 2
                                                              , (p.IsStateFDefined("LaserPosition") && p.GetStateF("LaserPosition") == 1) ? new TV_COLOR(1, 0.5f, 0, 1).GetIntColor() : pcolor.GetIntColor());

          Engine.Instance().TVScreen2DImmediate.Draw_Line(0 + Engine.Instance().ScreenWidth / 2
                                                    , 22 + Engine.Instance().ScreenHeight / 2
                                                    , 0 + Engine.Instance().ScreenWidth / 2
                                                    , 25 + Engine.Instance().ScreenHeight / 2
                                                    , pcolor.GetIntColor());

          Engine.Instance().TVScreen2DImmediate.Draw_Line(-15 + Engine.Instance().ScreenWidth / 2
                                                              , 25 + Engine.Instance().ScreenHeight / 2
                                                              , 15 + Engine.Instance().ScreenWidth / 2
                                                              , 25 + Engine.Instance().ScreenHeight / 2
                                                              , pcolor.GetIntColor());

          if ((PlayerInfo.Instance().PrimaryWeapon.Contains("photon") || PlayerInfo.Instance().SecondaryWeapon.Contains("photon")) && p.GetStateB("EnablePhoton"))
          {
            p1_x = -15;
            p1_y = 28;
            p2_x = 15;
            p2_y = 33;

            float tremain = p.GetStateF("PhotonRemaining") / p.GetStateF("PhotonMax");


            Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(p1_x + Engine.Instance().ScreenWidth / 2
                                                      , p1_y + Engine.Instance().ScreenHeight / 2
                                                      , p1_x + (p2_x - p1_x) * tremain + Engine.Instance().ScreenWidth / 2
                                                      , p2_y + Engine.Instance().ScreenHeight / 2
                                                      , new TV_COLOR(0.6f, 0.6f, 1, 1).GetIntColor());

            Engine.Instance().TVScreen2DImmediate.Draw_Box(p1_x + Engine.Instance().ScreenWidth / 2
                                                      , p1_y + Engine.Instance().ScreenHeight / 2
                                                      , p2_x + Engine.Instance().ScreenWidth / 2
                                                      , p2_y + Engine.Instance().ScreenHeight / 2
                                                      , new TV_COLOR(1, 0.5f, 0, 1).GetIntColor());
          }
        }

        if (PlayerInfo.Instance().IsTorpedoMode & p.GetStateB("EnableTorp"))
        {
          p1_x = 19;
          p1_y = 17;
          p2_x = 23;
          p2_y = 25;
          p3_x = 6;
          p3_y = 10;
          float tremain = p.GetStateF("TorpRemaining");
          float tmax = p.GetStateF("TorpMax");
          int t = 0;

          while (t < tremain || t < tmax)
          {
            if (t < tremain)
            {
              Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(p1_x + Engine.Instance().ScreenWidth / 2
                                                    , p1_y + Engine.Instance().ScreenHeight / 2
                                                    , p2_x + Engine.Instance().ScreenWidth / 2
                                                    , p2_y + Engine.Instance().ScreenHeight / 2
                                                    , pcolor.GetIntColor());
            }
            else
            {
              Engine.Instance().TVScreen2DImmediate.Draw_Box(p1_x + Engine.Instance().ScreenWidth / 2
                                    , p1_y + Engine.Instance().ScreenHeight / 2
                                    , p2_x + Engine.Instance().ScreenWidth / 2
                                    , p2_y + Engine.Instance().ScreenHeight / 2
                                    , pcolor.GetIntColor());
            }

            p1_x += p3_x;
            p2_x += p3_x;

            t++;
            if (t % 10 == 0)
            {
              p1_x = 19;
              p2_x = 23;
              p1_y += p3_y;
              p2_y += p3_y;
            }
          }
        }

        if (p.TypeInfo is FalconATI)
        {
          p1_x = 5;
          p1_y = 25;
          p2_x = -60;
          p2_y = -5;
          p3_x = 5;
          p3_y = 60;

          Engine.Instance().TVScreen2DImmediate.Draw_Line(0 + Engine.Instance().ScreenWidth / 2
                                                          , p1_x + Engine.Instance().ScreenHeight / 2
                                                          , 0 + Engine.Instance().ScreenWidth / 2
                                                          , p1_y + Engine.Instance().ScreenHeight / 2
                                                          , pcolor.GetIntColor());

          Engine.Instance().TVScreen2DImmediate.Draw_Line(p2_x + Engine.Instance().ScreenWidth / 2
                                                          , 0 + Engine.Instance().ScreenHeight / 2
                                                          , p2_y + Engine.Instance().ScreenWidth / 2
                                                          , 0 + Engine.Instance().ScreenHeight / 2
                                                          , pcolor.GetIntColor());

          Engine.Instance().TVScreen2DImmediate.Draw_Line(p3_x + Engine.Instance().ScreenWidth / 2
                                                          , 0 + Engine.Instance().ScreenHeight / 2
                                                          , p3_y + Engine.Instance().ScreenWidth / 2
                                                          , 0 + Engine.Instance().ScreenHeight / 2
                                                          , pcolor.GetIntColor());
        }
        Engine.Instance().TVScreen2DImmediate.Action_End2D();
        */
      }
    }

    private void DrawSingleBar(int barnumber, string text, float barlengthfrac, TV_COLOR color)
    {
      float h = barnumber * 1.2f;
      if (barlengthfrac < 0)
        barlengthfrac = 0;
      else if (barlengthfrac > 1)
        barlengthfrac = 1;

      Engine.Instance().TVScreen2DImmediate.Action_Begin2D();

      // Background
      Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(bar_topleft.x - 120
                                    , bar_topleft.y + bar_height * (h - 0.1f)
                                    , bar_topleft.x + bar_length + 5
                                    , bar_topleft.y + bar_height * (h + 1.1f)
                                    , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      Engine.Instance().TVScreen2DImmediate.Action_End2D();

      // Bar Background
      Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(bar_topleft.x
                                          , bar_topleft.y + bar_height * (h + 0.6f) - bar_barheight / 2
                                          , bar_topleft.x + bar_length
                                          , bar_topleft.y + bar_height * (h + 0.6f) + bar_barheight / 2
                                          , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());

      Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(bar_topleft.x
                                                  , bar_topleft.y + bar_height * (h + 0.6f) - bar_barheight / 2
                                                  , bar_topleft.x + bar_length * barlengthfrac
                                                  , bar_topleft.y + bar_height * (h + 0.6f) + bar_barheight / 2
                                                  , color.GetIntColor());

      Engine.Instance().TVScreen2DImmediate.Action_End2D();


      Engine.Instance().TVScreen2DText.Action_BeginText();

      Engine.Instance().TVScreen2DText.TextureFont_DrawText(text
        , bar_topleft.x - 115
        , bar_topleft.y + bar_height * h
        , color.GetIntColor()
        , FontID12
        );

      Engine.Instance().TVScreen2DText.Action_EndText();
    }

    private void DrawBars()
    {
      ActorInfo p = PlayerInfo.Instance().Actor;

      if (p != null && p.CreationState == CreationState.ACTIVE)
      {
        TV_COLOR pcolor = (p.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : p.Faction.Color;

        //Health Bar
        DrawSingleBar(0
                      , string.Format("HP [{0}%]", Math.Ceiling(PlayerInfo.Instance().StrengthFrac * 100))
                      , PlayerInfo.Instance().StrengthFrac
                      , PlayerInfo.Instance().HealthColor
                      );

        //Speed Bar
        DrawSingleBar(1
              , string.Format("SPEED ")
              , p.Speed / p.MaxSpeed
              , new TV_COLOR(0.7f, 0.8f, 0.4f, 1)
              );

        int barnumber = 2;

        //Allies
        foreach (ActorInfo a in GameScenarioManager.Instance().CriticalAllies.Values)
        {
          DrawSingleBar(barnumber
              , a.SideBarName.PadRight(12).Remove(11)
              , a.StrengthFrac
              , new TV_COLOR(0, 0.8f, 0.6f, 1)
              );
          barnumber++;
        }

        //Enemies
        foreach (ActorInfo a in GameScenarioManager.Instance().CriticalEnemies.Values)
        {
          DrawSingleBar(barnumber
              , a.SideBarName.PadRight(12).Remove(11)
              , a.StrengthFrac
              , new TV_COLOR(1f, 0, 0, 1)
              );
          barnumber++;
        }

        /*
        Engine.Instance().TVScreen2DText.Action_BeginText();
        Engine.Instance().TVScreen2DText.TextureFont_DrawText(string.Format("TOGGLE: {0} "
            , ((CONST_TV_KEY)InputManager.FunctionKeyMap.GetItem("g_ui_status_toggle")).ToString().Replace("TV_KEY_", "").Replace("-1", "").PadLeft(1)
            )
          , bar_topleft.x - 90
          , bar_topleft.y - 15
          , pcolor.GetIntColor()
          , FontID08
          );
        Engine.Instance().TVScreen2DText.Action_EndText();
        */
      }
    }

    private void DrawWeaponInfo()
    {
      ActorInfo p = PlayerInfo.Instance().Actor;

      if (p != null && p.CreationState == CreationState.ACTIVE)
      {
        TV_COLOR pcolor = (p.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : p.Faction.Color;

        Engine.Instance().TVScreen2DImmediate.Action_Begin2D();
        Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(leftinfo_left - 5
                                      , leftinfo_weapontop - 5
                                      , leftinfo_left + leftinfo_weaponwidth * 2 + 5
                                      , leftinfo_weapontop + leftinfo_weaponheight + 5
                                      , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());

        Engine.Instance().TVScreen2DImmediate.Draw_Box(leftinfo_left - 5
                              , leftinfo_weapontop - 5
                              , leftinfo_left + leftinfo_weaponwidth
                              , leftinfo_weapontop + leftinfo_weaponheight + 5
                              , pcolor.GetIntColor());

        Engine.Instance().TVScreen2DImmediate.Draw_Box(leftinfo_left + leftinfo_weaponwidth
                              , leftinfo_weapontop - 5
                              , leftinfo_left + leftinfo_weaponwidth * 2 + 5
                              , leftinfo_weapontop + leftinfo_weaponheight + 5
                              , pcolor.GetIntColor());
        Engine.Instance().TVScreen2DImmediate.Action_End2D();


        Engine.Instance().TVScreen2DText.Action_BeginText();

        Engine.Instance().TVScreen2DText.TextureFont_DrawText(string.Format("SWITCH: {0} {1}"
          , ((CONST_TV_KEY)InputManager.FunctionKeyMap.GetItem("g_weap1mode+")).ToString().Replace("TV_KEY_", "").Replace("-1", "").PadLeft(1)
          , ((CONST_TV_KEY)InputManager.FunctionKeyMap.GetItem("g_weap1mode-")).ToString().Replace("TV_KEY_", "").Replace("-1", "").PadLeft(1)
          )
        , leftinfo_left
        , leftinfo_weapontop
        , pcolor.GetIntColor()
        , FontID08
        );

        Engine.Instance().TVScreen2DText.TextureFont_DrawText(PlayerInfo.Instance().PrimaryWeapon.ToUpper() //.Replace("A", "").Replace("E", "").Replace("I", "").Replace("O", "").Replace("U", "")
        , leftinfo_left
        , leftinfo_weapontop + 20
        , pcolor.GetIntColor()
        , FontID16
        );

        Engine.Instance().TVScreen2DText.TextureFont_DrawText(string.Format("SWITCH: {0} {1}"
          , ((CONST_TV_KEY)InputManager.FunctionKeyMap.GetItem("g_weap2mode+")).ToString().Replace("TV_KEY_", "").Replace("-1", "").PadLeft(1)
          , ((CONST_TV_KEY)InputManager.FunctionKeyMap.GetItem("g_weap2mode-")).ToString().Replace("TV_KEY_", "").Replace("-1", "").PadLeft(1)
          )
        , leftinfo_left + leftinfo_weaponwidth + 5
        , leftinfo_weapontop
        , pcolor.GetIntColor()
        , FontID08
        );

        Engine.Instance().TVScreen2DText.TextureFont_DrawText(PlayerInfo.Instance().SecondaryWeapon.ToUpper() //.Replace("A", "").Replace("E", "").Replace("I", "").Replace("O", "").Replace("U", "")
        , leftinfo_left + leftinfo_weaponwidth + 5
        , leftinfo_weapontop + 20
        , pcolor.GetIntColor()
        , FontID16
        );
        Engine.Instance().TVScreen2DText.Action_EndText();
      }
    }

    private void DrawInfo()
    {
      ActorInfo p = PlayerInfo.Instance().Actor;

      if (p != null && p.CreationState == CreationState.ACTIVE)
      {
        Engine.Instance().TVScreen2DImmediate.Action_Begin2D();
        Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(Engine.Instance().ScreenWidth / 2 - infomiddlegap - infowidth_left - 5
                                      , infotop - 5
                                      , Engine.Instance().ScreenWidth / 2 - infomiddlegap + 5
                                      , infotop + infoheight * 4 + 5
                                      , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
        Engine.Instance().TVScreen2DImmediate.Action_End2D();

        TV_COLOR pcolor = (p.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : p.Faction.Color;

        Engine.Instance().TVScreen2DText.Action_BeginText();
        Engine.Instance().TVScreen2DText.TextureFont_DrawText(string.Format("LIVES: {0,8:0}\nSCORE: {1,8:00000000}\nKILLS: {2,8:0}\nHITS:  {3,8:0}"
        , PlayerInfo.Instance().Lives
        , PlayerInfo.Instance().Score.Score
        , PlayerInfo.Instance().Score.Kills
        , PlayerInfo.Instance().Score.Hits
        )
        , Engine.Instance().ScreenWidth / 2 - infomiddlegap - infowidth_left
        , infotop
        , pcolor.GetIntColor()
        , FontID12
        );
        Engine.Instance().TVScreen2DText.Action_EndText();


        if (GameScenarioManager.Instance().Scenario != null)
        {
          Engine.Instance().TVScreen2DImmediate.Action_Begin2D();
          Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(leftinfo_left - 5
                                        , leftinfo_stagetop - 5
                                        , leftinfo_left + leftinfo_stagewidth + 5
                                        , leftinfo_stageheight + 5
                                        , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());

          Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(Engine.Instance().ScreenWidth / 2 + infomiddlegap - 5
                                        , infotop - 5
                                        , Engine.Instance().ScreenWidth / 2 + -infomiddlegap + infowidth_right + 5
                                        , infotop + infoheight * 4 + 5
                                        , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
          Engine.Instance().TVScreen2DImmediate.Action_End2D();

          Engine.Instance().TVScreen2DText.Action_BeginText();
          // Scenario Title, Difficulty
          Engine.Instance().TVScreen2DText.TextureFont_DrawText(string.Format("{0}: {1}"
            , GameScenarioManager.Instance().Scenario.Name
            , GameScenarioManager.Instance().Difficulty
            )
            , leftinfo_left
            , leftinfo_stagetop
            , pcolor.GetIntColor()
            , FontID12
            );

          // StageNumber
          Engine.Instance().TVScreen2DText.TextureFont_DrawText(string.Format("STAGE: {0}"
            , GameScenarioManager.Instance().StageNumber
            )
            , Engine.Instance().ScreenWidth / 2 + infomiddlegap
            , infotop
            , pcolor.GetIntColor()
            , FontID12
            );

          Engine.Instance().TVScreen2DText.TextureFont_DrawText(GameScenarioManager.Instance().Line1Text
            , Engine.Instance().ScreenWidth / 2 + infomiddlegap
            , infotop + infoheight
            , GameScenarioManager.Instance().Line1Color.GetIntColor()
            , FontID12
            );

          Engine.Instance().TVScreen2DText.TextureFont_DrawText(GameScenarioManager.Instance().Line2Text
            , Engine.Instance().ScreenWidth / 2 + infomiddlegap
            , infotop + infoheight * 2
            , GameScenarioManager.Instance().Line2Color.GetIntColor()
            , FontID12
            );

          Engine.Instance().TVScreen2DText.TextureFont_DrawText(GameScenarioManager.Instance().Line3Text
            , Engine.Instance().ScreenWidth / 2 + infomiddlegap
            , infotop + infoheight * 3
            , GameScenarioManager.Instance().Line3Color.GetIntColor()
            , FontID12
            );

          /*
          // Wings
          int count = 0;

          if (GameScenarioManager.Instance().Scenario.TimeSinceLostWing < Game.Instance().Time || Game.Instance().Time % 0.4f > 0.2f)
          {
            foreach (ActorInfo a in GameScenarioManager.Instance().AllyFighters.Values)
            {
              if (a.CreationState == CreationState.ACTIVE)
              {
                count++;
              }
            }

            Engine.Instance().TVScreen2DText.TextureFont_DrawText(string.Format("WINGS: {0} / {1}"
                , count
                , GameScenarioManager.Instance().Scenario.RebelFighterLimit
                )
                , Engine.Instance().ScreenWidth / 2 + infomiddlegap
                , infotop + infoheight
                , new TV_COLOR(1f, 1f, 0.3f, 1).GetIntColor()
                , FontID12
                );
          }

          // Ships
          count = 0;
          if (GameScenarioManager.Instance().Scenario.TimeSinceLostShip < Game.Instance().Time || Game.Instance().Time % 0.4f > 0.2f)
          {
            foreach (ActorInfo a in GameScenarioManager.Instance().AllyShips.Values)
            {
              if (a.CreationState == CreationState.ACTIVE)
              {
                count++;
              }
            }

            Engine.Instance().TVScreen2DText.TextureFont_DrawText(string.Format("SHIPS: {0}"
                , count
                )
                , Engine.Instance().ScreenWidth / 2 + infomiddlegap
                , infotop + infoheight * 2
                , new TV_COLOR(1f, 1f, 0.3f, 1).GetIntColor()
                , FontID12
                );
          }

          // TIEs
          count = 0;
          foreach (ActorInfo a in GameScenarioManager.Instance().EnemyFighters.Values)
          {
            if (a.CreationState == CreationState.ACTIVE)
            {
              count++;
            }
          }

          Engine.Instance().TVScreen2DText.TextureFont_DrawText(string.Format("TIE:   {0}"
              , count
              )
              , Engine.Instance().ScreenWidth / 2 + infomiddlegap
              , infotop + infoheight * 3
              , new TV_COLOR(0.7f, 1f, 0.3f, 1).GetIntColor()
              , FontID12
              );
          */

          Engine.Instance().TVScreen2DText.Action_EndText();
        }
      }
    }

    private void DrawScore()
    {
      Engine.Instance().TVScreen2DImmediate.Action_Begin2D();
      Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(radar_center.x - 150
                                    , radar_center.y + radar_radius + 10 + bar_height * 4.8f
                                    , radar_center.x + 150
                                    , radar_center.y + radar_radius + 600
                                    , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      Engine.Instance().TVScreen2DImmediate.Action_End2D();

      SortedDictionary<float, ActorInfo> HighScorers = new SortedDictionary<float, ActorInfo>();
      foreach (ActorInfo a in ActorFactory.Instance().GetActorList())
      {
        if (a.CreationState == CreationState.ACTIVE && a.Score.Score > 0)
        {
          if (!HighScorers.ContainsKey(a.Score.Score + (float)a.ID / 100000))
            HighScorers.Add(a.Score.Score + (float)a.ID / 100000, a);
        }
      }
      string hiscoretext = "LEADERS         | SCORE    | KILL | HIT";
      int count = (HighScorers.Count > 20) ? 20 : HighScorers.Count;
      int hi = HighScorers.Count - 1;
      ActorInfo[] hilist = new ActorInfo[HighScorers.Count];
      HighScorers.Values.CopyTo(hilist, 0);
      while (count > 0 && hi >= 0)
      {
        hiscoretext += string.Format("\n{0,15}   {1,8:0}   {2,4:0}   {3,4:0}", (hilist[hi].Name.Length > 15) ? hilist[hi].Name.Remove(15): hilist[hi].Name.PadRight(15), hilist[hi].Score.Score, hilist[hi].Score.Kills, hilist[hi].Score.Hits);
        count--;
        hi--;
      }

      Engine.Instance().TVScreen2DText.TextureFont_DrawText(hiscoretext
      , radar_center.x - 150
      , radar_center.y + radar_radius + 10 + bar_height * 3.8f
      , new TV_COLOR(0.7f, 1f, 0.3f, 1).GetIntColor()
      , FontID12
      );

    }

    private void DrawHitInfo()
    {
      if (m_target == null)
      {
        prevstrengthfrac = 0;
        return;
      }

      Engine.Instance().TVScreen2DImmediate.Action_Begin2D();
      float barlength = Engine.Instance().ScreenWidth * 0.75f - 100;
      TV_COLOR tcolor = (m_target.Faction != null) ? m_target.Faction.Color : new TV_COLOR(1, 0.5f, 0, 1);
      TV_COLOR tpcolor = new TV_COLOR(tcolor.r, tcolor.g, tcolor.b, 0.3f);

      Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(50
                                        , Engine.Instance().ScreenHeight - 25
                                        , barlength + 50
                                        , Engine.Instance().ScreenHeight - 20
                                        , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());

      Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(50
                                        , Engine.Instance().ScreenHeight - 25
                                        , 50 + barlength * m_target.StrengthFrac
                                        , Engine.Instance().ScreenHeight - 20
                                        , tpcolor.GetIntColor());

      Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(50
                                        , Engine.Instance().ScreenHeight - 25
                                        , 50 + barlength * prevstrengthfrac
                                        , Engine.Instance().ScreenHeight - 20
                                        , tcolor.GetIntColor());

      if (prevstrengthfrac == 0)
      {
        prevstrengthfrac = m_target.StrengthFrac;
      }
      else
      {
        prevstrengthfrac = prevstrengthfrac + (m_target.StrengthFrac - prevstrengthfrac) * 0.2f;
      }
      Engine.Instance().TVScreen2DImmediate.Action_End2D();


      Engine.Instance().TVScreen2DText.Action_BeginText();
      Engine.Instance().TVScreen2DText.TextureFont_DrawText(m_target.Name
                                        , 65
                                        , Engine.Instance().ScreenHeight - 50
                                        , tcolor.GetIntColor()
                                        , FontID12);
      Engine.Instance().TVScreen2DText.Action_EndText();
    }

  }
}
