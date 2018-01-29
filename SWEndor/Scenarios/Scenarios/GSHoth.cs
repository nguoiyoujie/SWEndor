using MTV3D65;
using System.Collections.Generic;
using System;

namespace SWEndor.Scenarios
{
  public class GSHoth : GameScenarioBase
  {
    public GSHoth()
    {
      Name = "Escape of Hoth";
      AllowedWings = new List<ActorTypeInfo> { FalconATI.Instance()
                                              };

      AllowedDifficulties = new List<string> { "normal"
                                               , "hard"
                                              };
    }

    private ActorInfo m_AScene = null;
    private ActorInfo m_AHoth = null;

    public override void Load(ActorTypeInfo wing, string difficulty)
    {
      base.Load(wing, difficulty);
      PlayerInfo.Instance().Name = "Solo";

      if (GameScenarioManager.Instance().GetGameStateB("in_game"))
        return;

      GameScenarioManager.Instance().SetGameStateB("in_game", true);
      RegisterEvents();
      GameScenarioManager.Instance().SceneCamera.SetLocalPosition(0, 0, 0);

      GameScenarioManager.Instance().IsCutsceneMode = false;
    }

    public override void GameTick()
    {
      base.GameTick();
    }

    public override void LoadFactions()
    {
      base.LoadFactions();

      FactionInfo.AddFaction("Rebels", new TV_COLOR(0.8f, 0, 0, 1)).AutoAI = true;
      FactionInfo.AddFaction("Rebels_Falcon", new TV_COLOR(0.8f, 0.8f, 0.8f, 1)).AutoAI = true;
      FactionInfo.AddFaction("Empire", new TV_COLOR(0, 0.8f, 0, 1)).AutoAI = true;

      FactionInfo.Get("Rebels").Allies.Add(FactionInfo.Get("Rebels_Wedge"));
      FactionInfo.Get("Rebels_Falcon").Allies.Add(FactionInfo.Get("Rebels"));
    }

    public override void LoadScene()
    {
      base.LoadScene();

      ActorCreationInfo acinfo = null;

      // Create Room
      if (m_AScene == null)
      {
        acinfo = new ActorCreationInfo(SceneRoomATI.Instance());
        acinfo.InitialState = ActorState.FIXED;
        acinfo.CreationTime = -1;
        m_AScene = ActorInfo.Create(acinfo);
      }

      // Create Hoth
      if (m_AHoth == null)
      {
        acinfo = new ActorCreationInfo(HothATI.Instance());
        acinfo.InitialState = ActorState.FIXED;
        acinfo.CreationTime = -1;
        acinfo.Position = new TV_3DVECTOR(0, 0, 18000);
        acinfo.Rotation = new TV_3DVECTOR(90, 0, 0);
        acinfo.InitialScale = new TV_3DVECTOR(6, 6, 6);
        m_AHoth = ActorInfo.Create(acinfo);
      }
    }
  }
}
