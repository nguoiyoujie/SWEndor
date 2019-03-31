using MTV3D65;
using SWEndor.Actors;
using SWEndor.Scenarios;

namespace SWEndor.UI
{
  public class UIPage_PlayScenario : UIPage
  {
    UISelectionElement MainText = new UISelectionElement();
    UISelectionElement DescText = new UISelectionElement();
    UISelectionElement ButtonScenario = new UISelectionElement();
    UISelectionElement ButtonWing = new UISelectionElement();
    UISelectionElement ButtonDifficulty = new UISelectionElement();
    UISelectionElement ButtonPlay = new UISelectionElement();
    UISelectionElement ButtonBack = new UISelectionElement();
    int SelectedScenarioID = -1;
    GameScenarioBase SelectedScenario = null;
    int SelectedActorTypeInfoID = -1;
    ActorTypeInfo SelectedActorTypeInfo = null;
    int SelectedDifficultyID = -1;
    string SelectedDifficulty = null;

    public GameScenarioBase SelectedScenario1
    {
      get
      {
        return SelectedScenario;
      }

      set
      {
        SelectedScenario = value;
      }
    }

    public UIPage_PlayScenario()
    {
      float height_gap = 40;
      float x = 75;
      float y = 120;

      MainText.Text = "Setup Scenario";
      MainText.TextPosition = new TV_2DVECTOR(40, 60);

      ButtonScenario.Text = "Scenario";
      ButtonScenario.TextPosition = new TV_2DVECTOR(x, y);
      ButtonScenario.SecondaryTextPosition = new TV_2DVECTOR(x + 300, y);
      y += height_gap;
      ButtonScenario.HighlightBoxPosition = ButtonScenario.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonScenario.HighlightBoxWidth = 650;
      ButtonScenario.HighlightBoxHeight = 30;
      ButtonScenario.Selectable = true;
      ButtonScenario.OnKeyPress += SelectScenario;
      if (GameScenarioManager.Instance().ScenarioList.Count > 0)
      {
        ButtonScenario.SecondaryText = GameScenarioManager.Instance().ScenarioList[0].Name.ToUpper();
        SelectedScenario1 = GameScenarioManager.Instance().ScenarioList[0];
        SelectedScenarioID = 0;
      }


      ButtonWing.Text = "Choose your Fighter";
      ButtonWing.TextPosition = new TV_2DVECTOR(x, y);
      ButtonWing.SecondaryTextPosition = new TV_2DVECTOR(x + 300, y);
      y += height_gap;
      ButtonWing.HighlightBoxPosition = ButtonWing.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonWing.HighlightBoxWidth = 650;
      ButtonWing.HighlightBoxHeight = 30;
      ButtonWing.Selectable = true;
      ButtonWing.OnKeyPress += SelectWing;
      //ButtonScenario.SecondaryText = "X-Wing";
      if (SelectedScenario1.AllowedWings.Count > 0)
      {
        ButtonWing.SecondaryText = SelectedScenario1.AllowedWings[0].Name.ToUpper();
        SelectedActorTypeInfo = SelectedScenario1.AllowedWings[0];
        SelectedActorTypeInfoID = 0;
      }

      ButtonDifficulty.Text = "Difficulty";
      ButtonDifficulty.TextPosition = new TV_2DVECTOR(x, y);
      ButtonDifficulty.SecondaryTextPosition = new TV_2DVECTOR(x + 300, y);
      y += height_gap;
      ButtonDifficulty.HighlightBoxPosition = ButtonDifficulty.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonDifficulty.HighlightBoxWidth = 650;
      ButtonDifficulty.HighlightBoxHeight = 30;
      ButtonDifficulty.Selectable = true;
      ButtonDifficulty.OnKeyPress += SelectDifficulty;
      if (SelectedScenario1.AllowedDifficulties.Count > 0)
      {
        ButtonDifficulty.SecondaryText = SelectedScenario1.AllowedDifficulties[0].ToUpper();
        SelectedDifficulty = SelectedScenario1.AllowedDifficulties[0];
        SelectedDifficultyID = 0;
      }

      DescText.Text = SelectedScenario1.Description;
      DescText.TextFont = Font.GetFont("Text_14").ID;
      DescText.TextPosition = new TV_2DVECTOR(x, y);
      y += 120 +  height_gap;
      DescText.HighlightBoxPosition = DescText.TextPosition - new TV_2DVECTOR(5, 5);
      DescText.HighlightBoxWidth = 650;
      DescText.HighlightBoxHeight = 150;

      ButtonPlay.Text = "Launch!";
      ButtonPlay.TextPosition = new TV_2DVECTOR(x, y);
      y += height_gap;
      ButtonPlay.HighlightBoxPosition = ButtonPlay.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonPlay.HighlightBoxWidth = 150;
      ButtonPlay.HighlightBoxHeight = 30;
      ButtonPlay.Selectable = true;
      ButtonPlay.OnKeyPress += SelectPlay;

      ButtonBack.Text = "Exit";
      ButtonBack.TextPosition = new TV_2DVECTOR(x, Engine.Instance().ScreenHeight - 80);
      ButtonBack.HighlightBoxPosition = ButtonBack.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonBack.HighlightBoxWidth = 150;
      ButtonBack.HighlightBoxHeight = 30;
      ButtonBack.Selectable = true;
      ButtonBack.OnKeyPress += SelectBack;

      Elements.Add(MainText);
      Elements.Add(ButtonScenario);
      Elements.Add(ButtonWing);
      Elements.Add(ButtonDifficulty);
      Elements.Add(DescText);
      Elements.Add(ButtonPlay);
      Elements.Add(ButtonBack);
      SelectedElementID = Elements.IndexOf(ButtonScenario);
    }

    private bool SelectScenario(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        SelectedElementID = Elements.IndexOf(ButtonPlay);
        return true;
      }

      if (GameScenarioManager.Instance().ScenarioList.Count > 0)
      {
        if (key == CONST_TV_KEY.TV_KEY_LEFT)
        {
          int newSelectedScenario = SelectedScenarioID;

          newSelectedScenario--;
          if (newSelectedScenario < 0)
            newSelectedScenario = GameScenarioManager.Instance().ScenarioList.Count - 1;

          ButtonScenario.SecondaryText = GameScenarioManager.Instance().ScenarioList[newSelectedScenario].Name.ToUpper();
          SelectedScenario1 = GameScenarioManager.Instance().ScenarioList[newSelectedScenario];
          SelectedScenarioID = newSelectedScenario;

          SelectedActorTypeInfoID = 1;
          SelectWing(CONST_TV_KEY.TV_KEY_LEFT);

          SelectedDifficultyID = 1;
          SelectDifficulty(CONST_TV_KEY.TV_KEY_LEFT);

          DescText.Text = SelectedScenario1.Description;

          return true;
        }
        else if (key == CONST_TV_KEY.TV_KEY_RIGHT)
        {
          int newSelectedScenario = SelectedScenarioID;

          newSelectedScenario++;
          if (newSelectedScenario >= GameScenarioManager.Instance().ScenarioList.Count)
            newSelectedScenario = 0;

          ButtonScenario.SecondaryText = GameScenarioManager.Instance().ScenarioList[newSelectedScenario].Name.ToUpper();
          SelectedScenario1 = GameScenarioManager.Instance().ScenarioList[newSelectedScenario];
          SelectedScenarioID = newSelectedScenario;

          SelectedActorTypeInfoID = 1;
          SelectWing(CONST_TV_KEY.TV_KEY_LEFT);

          SelectedDifficultyID = 1;
          SelectDifficulty(CONST_TV_KEY.TV_KEY_LEFT);

          DescText.Text = SelectedScenario1.Description;

          return true;
        }
      }
      return false;
    }

    private bool SelectWing(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        SelectedElementID = Elements.IndexOf(ButtonPlay);
        return true;
      }

      if (SelectedScenario1 != null && SelectedScenario1.AllowedWings.Count > 0)
      {
        if (key == CONST_TV_KEY.TV_KEY_LEFT)
        {
          int newSelectedWing = SelectedActorTypeInfoID;

          newSelectedWing--;
          if (newSelectedWing < 0)
            newSelectedWing = SelectedScenario1.AllowedWings.Count - 1;

          ButtonWing.SecondaryText = SelectedScenario1.AllowedWings[newSelectedWing].Name.ToUpper();
          SelectedActorTypeInfo = SelectedScenario1.AllowedWings[newSelectedWing];
          SelectedActorTypeInfoID = newSelectedWing;

          return true;
        }
        else if (key == CONST_TV_KEY.TV_KEY_RIGHT)
        {
          int newSelectedWing = SelectedActorTypeInfoID;

          newSelectedWing++;
          if (newSelectedWing >= SelectedScenario1.AllowedWings.Count)
            newSelectedWing = 0;

          ButtonWing.SecondaryText = SelectedScenario1.AllowedWings[newSelectedWing].Name.ToUpper();
          SelectedActorTypeInfo = SelectedScenario1.AllowedWings[newSelectedWing];
          SelectedActorTypeInfoID = newSelectedWing;

          return true;
        }
      }
      return false;
    }

    private bool SelectDifficulty(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        SelectedElementID = Elements.IndexOf(ButtonPlay);
        return true;
      }

      if (SelectedScenario1 != null && SelectedScenario1.AllowedWings.Count > 0)
      {
        if (key == CONST_TV_KEY.TV_KEY_LEFT)
        {
          int newSelectedDiff = SelectedDifficultyID;

          newSelectedDiff--;
          if (newSelectedDiff < 0)
            newSelectedDiff = SelectedScenario1.AllowedDifficulties.Count - 1;

          ButtonDifficulty.SecondaryText = SelectedScenario1.AllowedDifficulties[newSelectedDiff].ToUpper();
          SelectedDifficulty = SelectedScenario1.AllowedDifficulties[newSelectedDiff];
          SelectedDifficultyID = newSelectedDiff;

          return true;
        }
        else if (key == CONST_TV_KEY.TV_KEY_RIGHT)
        {
          int newSelectedDiff = SelectedDifficultyID;

          newSelectedDiff++;
          if (newSelectedDiff >= SelectedScenario1.AllowedDifficulties.Count)
            newSelectedDiff = 0;

          ButtonDifficulty.SecondaryText = SelectedScenario1.AllowedDifficulties[newSelectedDiff].ToUpper();
          SelectedDifficulty = SelectedScenario1.AllowedDifficulties[newSelectedDiff];
          SelectedDifficultyID = newSelectedDiff;

          return true;
        }
      }
      return false;
    }

    private bool SelectPlay(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        EnterPage(new UIPage_LoadingScenario(SelectedScenario1, SelectedActorTypeInfo, SelectedDifficulty));
        return false;
      }
      return false;
    }

    private bool SelectBack(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        Back();
        return true;
      }
      return false;
    }
  }
}
