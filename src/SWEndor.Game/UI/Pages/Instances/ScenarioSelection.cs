using MTV3D65;
using SWEndor.Game.ActorTypes;
using Primrose.Primitives.Extensions;
using SWEndor.Game.Scenarios;

namespace SWEndor.Game.UI.Menu.Pages
{
  public class ScenarioSelection : Page
  {
    SelectionElement MainText = new SelectionElement();
    SelectionElement DescText = new SelectionElement();
    SelectionElement ButtonScenario = new SelectionElement();
    SelectionElement ButtonWing = new SelectionElement();
    SelectionElement ButtonDifficulty = new SelectionElement();
    SelectionElement ButtonPlay = new SelectionElement();
    SelectionElement ButtonBack = new SelectionElement();

    CampaignInfo Campaign;
    int SelectedScenarioID = -1;
    ScenarioBase SelectedScenario = null;
    int SelectedActorTypeInfoID = -1;
    ActorTypeInfo SelectedActorTypeInfo = null;
    int SelectedDifficultyID = -1;
    string SelectedDifficulty = null;

    public ScenarioSelection(Screen2D owner, CampaignInfo campaign) : base(owner)
    {
      Campaign = campaign;

      float height_gap = 40;
      float x = 75;
      float y = 120;

      MainText.Text = "Setup Scenario";
      MainText.TextPosition = new TV_2DVECTOR(40, 60);

      ButtonScenario.Text = "Scenario";
      ButtonScenario.TextFont = FontFactory.Get(Font.T14).ID;
      ButtonScenario.TextPosition = new TV_2DVECTOR(x, y);
      ButtonScenario.SecondaryTextPosition = new TV_2DVECTOR(x + 250, y);
      y += height_gap;
      ButtonScenario.HighlightBoxPosition = ButtonScenario.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonScenario.HighlightBoxWidth = 650;
      ButtonScenario.HighlightBoxHeight = 30;
      ButtonScenario.Selectable = true;
      ButtonScenario.OnKeyPress += SelectScenario;
      if (Campaign.Scenarios.Count > 0)
      {
        ButtonScenario.SecondaryText = Campaign.Scenarios[0].Info.Name.ToUpper();
        SelectedScenario = Campaign.Scenarios[0];
        SelectedScenarioID = 0;
      }

      ButtonWing.Text = "Choose your Fighter";
      ButtonWing.TextFont = FontFactory.Get(Font.T14).ID;
      ButtonWing.TextPosition = new TV_2DVECTOR(x, y);
      ButtonWing.SecondaryTextPosition = new TV_2DVECTOR(x + 250, y);
      y += height_gap;
      ButtonWing.HighlightBoxPosition = ButtonWing.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonWing.HighlightBoxWidth = 650;
      ButtonWing.HighlightBoxHeight = 30;
      ButtonWing.Selectable = true;
      ButtonWing.OnKeyPress += SelectWing;
      //ButtonScenario.SecondaryText = "XWING";
      if (SelectedScenario.Info.AllowedWings.Length > 0)
      {
        ActorTypeInfo act = SelectedScenario.Info.AllowedWings[0];
        string name = act.Designation == null ? act.Name : (act.Name + " [" + act.Designation + "]");
        ButtonWing.SecondaryText = name.ToUpper();
        SelectedActorTypeInfo = act;
        SelectedActorTypeInfoID = 0;
      }

      ButtonDifficulty.Text = "Difficulty";
      ButtonDifficulty.TextFont = FontFactory.Get(Font.T14).ID;
      ButtonDifficulty.TextPosition = new TV_2DVECTOR(x, y);
      ButtonDifficulty.SecondaryTextPosition = new TV_2DVECTOR(x + 250, y);
      y += height_gap;
      ButtonDifficulty.HighlightBoxPosition = ButtonDifficulty.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonDifficulty.HighlightBoxWidth = 650;
      ButtonDifficulty.HighlightBoxHeight = 30;
      ButtonDifficulty.Selectable = true;
      ButtonDifficulty.OnKeyPress += SelectDifficulty;
      if (SelectedScenario.Info.AllowedDifficulties.Length > 0)
      {
        ButtonDifficulty.SecondaryText = SelectedScenario.Info.AllowedDifficulties[0].ToUpper();
        SelectedDifficulty = SelectedScenario.Info.AllowedDifficulties[0];
        SelectedDifficultyID = 0;
      }

      DescText.TextFont = FontFactory.Get(Font.T12).ID;
      DescText.TextPosition = new TV_2DVECTOR(x, y);
      y += 120 +  height_gap;
      DescText.HighlightBoxPosition = DescText.TextPosition - new TV_2DVECTOR(5, 5);
      DescText.HighlightBoxWidth = 650;
      DescText.HighlightBoxHeight = 150;
      DescText.Text = FormatDesc();

      ButtonPlay.Text = "Launch!";
      ButtonPlay.TextPosition = new TV_2DVECTOR(x, y);
      y += height_gap;
      ButtonPlay.HighlightBoxPosition = ButtonPlay.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonPlay.HighlightBoxWidth = 150;
      ButtonPlay.HighlightBoxHeight = 30;
      ButtonPlay.Selectable = true;
      ButtonPlay.OnKeyPress += SelectPlay;

      ButtonBack.Text = "Back";
      ButtonBack.TextPosition = new TV_2DVECTOR(x, owner.ScreenSize.y - 80);
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

    private string FormatDesc()
    {
      return SelectedScenario.Info.Description.Multiline(((int)DescText.HighlightBoxWidth - 10) / 9); // font 12 width = 9
    }

    private bool SelectScenario(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        SelectedElementID = Elements.IndexOf(ButtonPlay);
        return true;
      }

      if (Campaign.Scenarios.Count > 0)
      {
        if (key == CONST_TV_KEY.TV_KEY_LEFT)
        {
          int newSelectedScenario = SelectedScenarioID;

          newSelectedScenario--;
          if (newSelectedScenario < 0)
            newSelectedScenario = Campaign.Scenarios.Count - 1;

          ButtonScenario.SecondaryText = Campaign.Scenarios[newSelectedScenario].Info.Name.ToUpper();
          SelectedScenario = Campaign.Scenarios[newSelectedScenario];
          SelectedScenarioID = newSelectedScenario;

          SelectedActorTypeInfoID = 1;
          SelectWing(CONST_TV_KEY.TV_KEY_LEFT);

          SelectedDifficultyID = 1;
          SelectDifficulty(CONST_TV_KEY.TV_KEY_LEFT);

          DescText.Text = FormatDesc();

          return true;
        }
        else if (key == CONST_TV_KEY.TV_KEY_RIGHT)
        {
          int newSelectedScenario = SelectedScenarioID;

          newSelectedScenario++;
          if (newSelectedScenario >= Campaign.Scenarios.Count)
            newSelectedScenario = 0;

          ButtonScenario.SecondaryText = Campaign.Scenarios[newSelectedScenario].Info.Name.ToUpper();
          SelectedScenario = Campaign.Scenarios[newSelectedScenario];
          SelectedScenarioID = newSelectedScenario;

          SelectedActorTypeInfoID = 1;
          SelectWing(CONST_TV_KEY.TV_KEY_LEFT);

          SelectedDifficultyID = 1;
          SelectDifficulty(CONST_TV_KEY.TV_KEY_LEFT);

          DescText.Text = FormatDesc();

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

      if (SelectedScenario != null && SelectedScenario.Info.AllowedWings.Length > 0)
      {
        if (key == CONST_TV_KEY.TV_KEY_LEFT)
        {
          int newSelectedWing = SelectedActorTypeInfoID;

          newSelectedWing--;
          if (newSelectedWing < 0)
            newSelectedWing = SelectedScenario.Info.AllowedWings.Length - 1;

          ActorTypeInfo act = SelectedScenario.Info.AllowedWings[newSelectedWing];
          string name = act.Designation == null ? act.Name : (act.Name + " [" + act.Designation + "]");
          ButtonWing.SecondaryText = name.ToUpper();
          SelectedActorTypeInfo = act;
          SelectedActorTypeInfoID = newSelectedWing;

          return true;
        }
        else if (key == CONST_TV_KEY.TV_KEY_RIGHT)
        {
          int newSelectedWing = SelectedActorTypeInfoID;

          newSelectedWing++;
          if (newSelectedWing >= SelectedScenario.Info.AllowedWings.Length)
            newSelectedWing = 0;

          ActorTypeInfo act = SelectedScenario.Info.AllowedWings[newSelectedWing];
          string name = act.Designation == null ? act.Name : (act.Name + " [" + act.Designation + "]");
          ButtonWing.SecondaryText = name.ToUpper();
          SelectedActorTypeInfo = act;
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

      if (SelectedScenario != null && SelectedScenario.Info.AllowedWings.Length > 0)
      {
        if (key == CONST_TV_KEY.TV_KEY_LEFT)
        {
          int newSelectedDiff = SelectedDifficultyID;

          newSelectedDiff--;
          if (newSelectedDiff < 0)
            newSelectedDiff = SelectedScenario.Info.AllowedDifficulties.Length - 1;

          ButtonDifficulty.SecondaryText = SelectedScenario.Info.AllowedDifficulties[newSelectedDiff].ToUpper();
          SelectedDifficulty = SelectedScenario.Info.AllowedDifficulties[newSelectedDiff];
          SelectedDifficultyID = newSelectedDiff;

          return true;
        }
        else if (key == CONST_TV_KEY.TV_KEY_RIGHT)
        {
          int newSelectedDiff = SelectedDifficultyID;

          newSelectedDiff++;
          if (newSelectedDiff >= SelectedScenario.Info.AllowedDifficulties.Length)
            newSelectedDiff = 0;

          ButtonDifficulty.SecondaryText = SelectedScenario.Info.AllowedDifficulties[newSelectedDiff].ToUpper();
          SelectedDifficulty = SelectedScenario.Info.AllowedDifficulties[newSelectedDiff];
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
        EnterPage(new LoadingScenario(Owner, SelectedScenario, SelectedActorTypeInfo, SelectedDifficulty));
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
