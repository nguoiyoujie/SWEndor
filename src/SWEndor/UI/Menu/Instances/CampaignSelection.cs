using MTV3D65;
using SWEndor.ActorTypes;
using Primrose.Primitives.Extensions;
using SWEndor.Scenarios;
using System.Collections.Generic;

namespace SWEndor.UI.Menu.Pages
{
  public class CampaignSelection : Page
  {
    SelectionElement MainText = new SelectionElement();
    SelectionElement DescText = new SelectionElement();
    SelectionElement NameText = new SelectionElement();
    List<SelectionElement> ChoiceText = new List<SelectionElement>();

    SelectionElement ButtonPrev = new SelectionElement();
    SelectionElement ButtonNext = new SelectionElement();
    SelectionElement ButtonBack = new SelectionElement();
    CampaignInfo SelectedCampaign = null;
    int page = 0;

    public CampaignSelection(Screen2D owner) : base(owner)
    {
      float height_gap = 40;
      float x = 350;
      float cx = 50;
      float y = 120;
      float cy = 120;

      MainText.Text = "Select Campaign";
      MainText.TextPosition = new TV_2DVECTOR(40, 60);

      while (cy < owner.ScreenSize.y - 150)
      {
        SelectionElement line = new SelectionElement();
        ChoiceText.Add(line);
        line.Text = "";
        line.TextPosition = new TV_2DVECTOR(cx, cy);
        line.TextFont = FontFactory.Get(Font.T12).ID;
        line.HighlightBoxPosition = line.TextPosition - new TV_2DVECTOR(5, 5);
        line.HighlightBoxWidth = 275;
        line.HighlightBoxHeight = 25;
        line.Selectable = false;
        line.OnKeyPress += SelectCampaign;
        line.AfterKeyPress += UpdateItem;
        cy += 30;
      }

      NameText.Text = "";
      NameText.TextFont = FontFactory.Get(Font.T14).ID;
      NameText.TextPosition = new TV_2DVECTOR(x, y);
      y += height_gap;
      NameText.HighlightBoxPosition = NameText.TextPosition - new TV_2DVECTOR(5, 5);
      NameText.HighlightBoxWidth = 375;
      NameText.HighlightBoxHeight = 30;
      if (GameScenarioManager.CampaignList.Count > 0)
      {
        NameText.Text = GameScenarioManager.CampaignList[0].Name.ToUpper();
        SelectedCampaign = GameScenarioManager.CampaignList[0];
      }

      DescText.TextFont = FontFactory.Get(Font.T12).ID;
      DescText.TextPosition = new TV_2DVECTOR(x, y);
      y += 120 +  height_gap;
      DescText.HighlightBoxPosition = DescText.TextPosition - new TV_2DVECTOR(5, 5);
      DescText.HighlightBoxWidth = 400;
      DescText.HighlightBoxHeight = 350;
      DescText.Text = SelectedCampaign.Description.Multiline(((int)DescText.HighlightBoxWidth - 10) / 9); // font 12 width = 9

      ButtonPrev.Text = "Prev";
      ButtonPrev.TextPosition = new TV_2DVECTOR(cx, owner.ScreenSize.y - 120);
      ButtonPrev.HighlightBoxPosition = ButtonPrev.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonPrev.HighlightBoxWidth = 120;
      ButtonPrev.HighlightBoxHeight = 30;
      ButtonPrev.Selectable = page > 0;
      ButtonPrev.OnKeyPress += SelectPrev;
      ButtonPrev.AfterKeyPress += UpdateItem;

      ButtonNext.Text = "Next";
      ButtonNext.TextPosition = new TV_2DVECTOR(cx + 155, owner.ScreenSize.y - 120);
      ButtonNext.HighlightBoxPosition = ButtonNext.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonNext.HighlightBoxWidth = 120;
      ButtonNext.HighlightBoxHeight = 30;
      ButtonNext.Selectable = (page + 1) * ChoiceText.Count < GameScenarioManager.CampaignList.Count;
      ButtonNext.OnKeyPress += SelectNext;
      ButtonNext.AfterKeyPress += UpdateItem;

      ButtonBack.Text = "Back";
      ButtonBack.TextPosition = new TV_2DVECTOR(cx, owner.ScreenSize.y - 80);
      ButtonBack.HighlightBoxPosition = ButtonBack.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonBack.HighlightBoxWidth = 120;
      ButtonBack.HighlightBoxHeight = 30;
      ButtonBack.Selectable = true;
      ButtonBack.OnKeyPress += SelectBack;
      ButtonBack.AfterKeyPress += UpdateItem;

      Elements.Add(MainText);
      Elements.Add(NameText);
      Elements.Add(DescText);
      Elements.AddRange(ChoiceText);
      Elements.Add(ButtonPrev);
      Elements.Add(ButtonNext);
      Elements.Add(ButtonBack);
      SelectedElementID = ChoiceText.Count > 0 ? Elements.IndexOf(ChoiceText[0]) : Elements.IndexOf(ButtonBack);

      UpdatePage();
    }

    private void UpdatePage()
    {
      for (int i = 0; i < ChoiceText.Count; i++)
      {
        int j = page * ChoiceText.Count + i;
        if (j < GameScenarioManager.CampaignList.Count)
        {
          ChoiceText[i].Text = GameScenarioManager.CampaignList[j].Name;
          ChoiceText[i].Selectable = true;
        }
        else
        {
          ChoiceText[i].Text = "";
          ChoiceText[i].Selectable = false;
        }
      }

      ButtonPrev.Selectable = page > 0;
      ButtonPrev.TextColor = ButtonPrev.Selectable ? ColorLocalization.Get(ColorLocalKeys.UI_TEXT) : new COLOR(0.5f, 0.5f, 0.5f, 1);
      ButtonNext.Selectable = (page + 1) * ChoiceText.Count < GameScenarioManager.CampaignList.Count;
      ButtonNext.TextColor = ButtonNext.Selectable ? ColorLocalization.Get(ColorLocalKeys.UI_TEXT) : new COLOR(0.5f, 0.5f, 0.5f, 1);
      if ((SelectedElementID == Elements.IndexOf(ButtonPrev) && !ButtonPrev.Selectable)
        || (SelectedElementID == Elements.IndexOf(ButtonNext) && !ButtonNext.Selectable))
        SelectedElementID = ChoiceText.Count > 0 ? Elements.IndexOf(ChoiceText[0]) : Elements.IndexOf(ButtonBack);
    }

    private bool UpdateItem(CONST_TV_KEY key)
    {
      for (int i = 0; i < ChoiceText.Count; i++)
      {
        if (SelectedElementID == Elements.IndexOf(ChoiceText[i]))
        {
          int c = page * ChoiceText.Count + i;
          SelectedCampaign = GameScenarioManager.CampaignList[c];
          NameText.Text = SelectedCampaign.Name.ToUpper();
          DescText.Text = SelectedCampaign.Description.Multiline(((int)DescText.HighlightBoxWidth - 10) / 9);
        }
      }
      return true;
    }

    private bool SelectPrev(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        page--;
        UpdatePage();
        return true;
      }
      return false;
    }

    private bool SelectNext(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        page++;
        UpdatePage();
        return true;
      }
      return false;
    }

    private bool SelectCampaign(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        if (SelectedCampaign != null)
        {
          EnterPage(new ScenarioSelection(Owner, SelectedCampaign));
          return true;
        }
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
