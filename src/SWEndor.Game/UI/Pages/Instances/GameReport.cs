using MTV3D65;
using SWEndor.Game.Core;
using Primrose.Primitives.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Primrose.Primitives.Factories;
using SWEndor.Game.Actors.Components;
using System;
using static SWEndor.Game.Actors.Components.ScoreInfo;

namespace SWEndor.Game.UI.Menu.Pages
{
  public class GameReport : Page
  {
    const int LineLimit = 16;
    const int SmallLineLimit = 40;

    SelectionElement MainText = new SelectionElement();
    SelectionElement ReportText = new SelectionElement();
    SelectionElement ButtonNext = new SelectionElement();
    SelectionElement ButtonPrev = new SelectionElement();
    SelectionElement ButtonReturn = new SelectionElement();
    SelectionElement[] EventTexts = new SelectionElement[SmallLineLimit];
    StringBuilder sb = new StringBuilder();
    int Page = 0;

    public GameReport(Screen2D owner) : base(owner)
    {
      MainText.Text = "After Action Report";
      MainText.TextFont = FontFactory.Get(Font.T24).ID;
      MainText.TextPosition = new TV_2DVECTOR(owner.ScreenCenter.x - 352, 60);

      ReportText.UnHighlightBoxColor = ColorLocalization.Get(ColorLocalKeys.UI_UNHIGHLIGHT_DARK_BACKGROUND);
      ReportText.TextFont = FontFactory.Get(Font.T12).ID;
      ReportText.TextPosition = new TV_2DVECTOR(owner.ScreenCenter.x - 292, 120);
      ReportText.HighlightBoxPosition = new TV_2DVECTOR(owner.ScreenCenter.x - 365, 45);
      ReportText.HighlightBoxWidth = 365 * 2;
      ReportText.HighlightBoxHeight = Engine.ScreenHeight / 2 + 242 - 55;

      ButtonNext.Text = "Next";
      ButtonNext.TextPosition = new TV_2DVECTOR(owner.ScreenCenter.x + 170, owner.ScreenCenter.y + 260);
      ButtonNext.HighlightBoxPosition = ButtonNext.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonNext.HighlightBoxWidth = 200;
      ButtonNext.HighlightBoxHeight = 30;
      ButtonNext.Selectable = true;
      ButtonNext.OnKeyPress += SelectNext;

      ButtonPrev.Text = "Prev";
      ButtonPrev.TextPosition = new TV_2DVECTOR(owner.ScreenCenter.x - 360, owner.ScreenCenter.y + 260);
      ButtonPrev.HighlightBoxPosition = ButtonPrev.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonPrev.HighlightBoxWidth = 200;
      ButtonPrev.HighlightBoxHeight = 30;
      ButtonPrev.Selectable = true;
      ButtonPrev.OnKeyPress += SelectPrev;

      ButtonReturn.Text = "Return to Menu";
      ButtonReturn.TextPosition = new TV_2DVECTOR(owner.ScreenCenter.x + 392, owner.ScreenCenter.y + 300);
      ButtonReturn.HighlightBoxPosition = ButtonReturn.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonReturn.HighlightBoxWidth = 200;
      ButtonReturn.HighlightBoxHeight = 30;
      ButtonReturn.Selectable = true;
      ButtonReturn.OnKeyPress += SelectReturn;

      for (int i = 0; i < EventTexts.Length; i++)
      {
        SelectionElement line = new SelectionElement();
        EventTexts[i] = line;
        line.Text = "";
        line.TextPosition = new TV_2DVECTOR(owner.ScreenCenter.x - 292, 200 + i * 13);
        line.TextFont = FontFactory.Get(Font.T10).ID;
        line.UnHighlightBoxColor = ColorLocalization.Get(ColorLocalKeys.TRANSPARENT);
        line.HighlightBoxColor = ColorLocalization.Get(ColorLocalKeys.TRANSPARENT);
        line.SecondaryText = "";
        line.SecondaryTextFont = FontFactory.Get(Font.T12).ID;
        line.SecondaryTextPosition = line.TextPosition + new TV_2DVECTOR(50, 0);
        line.Selectable = false;
      }

      Elements.Add(ReportText);
      Elements.Add(MainText);
      for (int i = 0; i < EventTexts.Length; i++)
      {
        Elements.Add(EventTexts[i]);
      }
      Elements.Add(ButtonPrev);
      Elements.Add(ButtonNext);
      Elements.Add(ButtonReturn);
      SelectedElementID = Elements.IndexOf(ButtonNext);

      UpdatePage();
    }

    private bool SelectNext(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        Page++;
        UpdatePage();
        return true;
      }
      else if (key == CONST_TV_KEY.TV_KEY_RIGHT)
      {
        SelectedElementID = Elements.IndexOf(ButtonReturn);
        return true;
      }
      else if (key == CONST_TV_KEY.TV_KEY_LEFT)
      {
        SelectedElementID = Elements.IndexOf(ButtonPrev);
        return true;
      }
      return false;
    }

    private bool SelectPrev(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        Page--;
        UpdatePage();
        return true;
      }
      else if (key == CONST_TV_KEY.TV_KEY_RIGHT)
      {
        SelectedElementID = Elements.IndexOf(ButtonNext);
        return true;
      }
      else if (key == CONST_TV_KEY.TV_KEY_LEFT)
      {
        SelectedElementID = Elements.IndexOf(ButtonReturn);
        return true;
      }
      return false;
    }

    private bool SelectReturn(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        GameScenarioManager.Reset();
        GameScenarioManager.LoadMainMenu();
        return true;
      }
      else if (key == CONST_TV_KEY.TV_KEY_RIGHT)
      {
        SelectedElementID = Elements.IndexOf(ButtonPrev);
        return true;
      }
      else if (key == CONST_TV_KEY.TV_KEY_LEFT)
      {
        SelectedElementID = Elements.IndexOf(ButtonNext);
        return true;
      }
      return false;
    }

    private void UpdatePagePart(int index, int totalindex, int linelimit, string sectionstring, float sectionscore, string scoreformat, Registry<int> registry, string itemscoreformat, bool ignoreCountIfOne = true)
    {
      sb.Append((sectionstring.Length > 52) ? (sectionstring.Remove(49) + "...") : sectionstring.PadRight(52));
      sb.AppendLine(sectionscore.ToString(scoreformat).PadLeft(9));
      sb.AppendLine();
      int current = 0;
      int min = index * linelimit;
      int max = (index + 1) * linelimit;
      if (registry.Count > 0)
      {
        List<string> kills = registry.GetKeys().ToList();
        kills.Sort();
        sb.AppendLine("List {0} of {1}".F(index + 1, totalindex));
        sb.AppendLine();
        foreach (string name in kills)
        {
          if (current >= max) { break; }
          if (current++ < min) { continue; }
          int value = registry[name];
          sb.Append((name.Length > 52) ? (name.Remove(49) + "...") : name.PadRight(52));
          if (!ignoreCountIfOne || value > 1)
          {
            sb.Append(value.ToString(itemscoreformat).PadLeft(9));
          }
          sb.AppendLine();
        }
      }
    }

    private void UpdatePagePart(int index, int totalindex, int linelimit, string sectionstring, float sectionscore, string scoreformat, Registry<float> registry, string itemscoreformat)
    {
      sb.Append((sectionstring.Length > 52) ? (sectionstring.Remove(49) + "...") : sectionstring.PadRight(52));
      sb.AppendLine(sectionscore.ToString(scoreformat).PadLeft(9));
      sb.AppendLine();
      int current = 0;
      int min = index * linelimit;
      int max = (index + 1) * linelimit;
      if (registry.Count > 0)
      {
        List<string> kills = registry.GetKeys().ToList();
        kills.Sort();
        sb.AppendLine("List {0} of {1}".F(index + 1, totalindex));
        sb.AppendLine();
        foreach (string name in kills)
        {
          if (current >= max) { break; }
          if (current++ < min) { continue; }
          float value = registry[name];
          sb.Append((name.Length > 52) ? (name.Remove(49) + "...") : name.PadRight(52));
          sb.Append(value.ToString(itemscoreformat).PadLeft(9));
          sb.AppendLine();
        }
      }
    }

    private void UpdatePageEvents(int index, int totalindex, int linelimit, string sectionstring, List<ScoreInfo.Entry> entries)
    {
      sb.AppendLine(sectionstring);
      int min = index * linelimit;
      int max = (index + 1) * linelimit;
      for (int i = min; i < max && i < entries.Count; i++)
      {
        int id = i - min;
        ScoreInfo.Entry entry = entries[i];
        SelectionElement line = EventTexts[id];
        line.Text = LookUpString.GetTimeDisplay(entry.GameTime);
        string targetname = string.IsNullOrEmpty(entry.TargetName) ? string.Empty.PadRight(32) : (entry.TargetName.Length > 32) ? (entry.TargetName.Remove(29) + "...") : entry.TargetName.PadRight(32);
        string blankvalue = " ".PadLeft(9);
        string damagevalue = damagevalue = entry.DamageValue.ToString("####0.00").PadLeft(9);
        string scorevalue = blankvalue;
        switch (entry.Type)
        {
          case EntryType.HIT:
          case EntryType.KILL:
          case EntryType.OBJECTIVE_MET:
            scorevalue = entry.ScoreValue.ToString("########").PadLeft(9);
            break;
          case EntryType.OBJECTIVE_FAILED:
          case EntryType.FRIENDLY_HIT:
          case EntryType.FRIENDLY_KILL:
          case EntryType.DAMAGE_TAKEN:
          case EntryType.DEATH:
            scorevalue = (-entry.ScoreValue).ToString("########").PadLeft(9);
            break;
          case EntryType.MESSAGE_LOG:
          case EntryType.MESSAGE_LOG_SPLIT:
            targetname = entry.TargetName;
            break;
        }
        switch (entry.Type)
        {
          case EntryType.HIT:
            line.SecondaryTextColor = ColorLocalization.Get(ColorLocalKeys.UI_TEXT_GOOD);
            line.SecondaryText = "HIT         {0} {1} {2}".F(targetname, damagevalue, scorevalue);
            break;
          case EntryType.KILL:
            line.SecondaryTextColor = ColorLocalization.Get(ColorLocalKeys.UI_TEXT_GOOD);
            line.SecondaryText = "KILL        {0} {1} {2}".F(targetname, blankvalue, scorevalue);
            break;
          case EntryType.OBJECTIVE_MET:
            line.SecondaryTextColor = ColorLocalization.Get(ColorLocalKeys.UI_TEXT_GOOD);
            line.SecondaryText = "OBJ MET     {0} {1} {2}".F(targetname, blankvalue, scorevalue);
            break;
          case EntryType.OBJECTIVE_FAILED:
            line.SecondaryTextColor = ColorLocalization.Get(ColorLocalKeys.UI_TEXT_BAD);
            line.SecondaryText = "OBJ LOSS    {0} {1} {2}".F(targetname, blankvalue, scorevalue);
            break;
          case EntryType.FRIENDLY_HIT:
            line.SecondaryTextColor = ColorLocalization.Get(ColorLocalKeys.UI_TEXT_BAD);
            line.SecondaryText = "FRIEND HIT  {0} {1} {2}".F(targetname, damagevalue, scorevalue);
            break;
          case EntryType.FRIENDLY_KILL:
            line.SecondaryTextColor = ColorLocalization.Get(ColorLocalKeys.UI_TEXT_BAD);
            line.SecondaryText = "FRIEND KILL {0} {1} {2}".F(targetname, blankvalue, scorevalue);
            break;
          case EntryType.DAMAGE_TAKEN:
            line.SecondaryTextColor = ColorLocalization.Get(ColorLocalKeys.UI_TEXT);
            line.SecondaryText = "DAMAGED BY  {0} {1} {2}".F(targetname, damagevalue, scorevalue);
            break;
          case EntryType.DEATH:
            line.SecondaryTextColor = ColorLocalization.Get(ColorLocalKeys.UI_TEXT_BAD);
            line.SecondaryText = "KILLED BY   {0} {1} {2}".F(targetname, blankvalue, scorevalue);
            break;
          case EntryType.MESSAGE_LOG:
            line.SecondaryTextColor = new COLOR(entry.ScoreValue);
            line.SecondaryText = "MESSAGE     {0}".F(targetname);
            break;
          case EntryType.MESSAGE_LOG_SPLIT:
            line.Text = string.Empty;
            line.SecondaryTextColor = new COLOR(entry.ScoreValue);
            line.SecondaryText = "            {0}".F(targetname);
            break;
          default:
            line.SecondaryTextColor = ColorLocalization.Get(ColorLocalKeys.UI_TEXT);
            line.SecondaryText = "{0}  {1}".F(entry.Type, targetname);
            break;
        }
      }
    }

    private void UpdatePage()
    {
      Actors.Components.ScoreInfo score = Engine.PlayerInfo.Score;
      int killsPage = 1 + score.KillsByName.Count / LineLimit;
      int damagesPage = 1 + score.DamageDealtByName.Count / LineLimit;
      int frienddamagesPage = 1 + score.FriendlyDamageDealtByName.Count / LineLimit;
      int friendkillsPage = 1 + score.FriendlyKillsByName.Count / LineLimit;
      int damagedByPage = 1 + score.DamageTakenByName.Count / LineLimit;
      int killedByPage = 1 + score.KilledByName.Count / LineLimit;
      int eventsPage = 1 + score.Entries.Count / 32;
      int killsPageStart = 1;
      int damagesPageStart = killsPageStart + killsPage;
      int frienddamagesPageStart = damagesPageStart + damagesPage;
      int friendkillsPageStart = frienddamagesPageStart + frienddamagesPage;
      int damagedByPageStart = friendkillsPageStart + friendkillsPage;
      int killedByPageStart = damagedByPageStart + damagedByPage;
      int eventsPageStart = killedByPageStart + killedByPage;
      int pageTotal = eventsPageStart + eventsPage;
      Page = (Page + pageTotal) % (pageTotal);

      for (int i = 0; i < EventTexts.Length; i++)
      {
        EventTexts[i].Text = string.Empty;
        EventTexts[i].SecondaryText = string.Empty;
      }

      sb.Clear();
      sb.AppendLine(GameScenarioManager.Scenario.Info.Name.ToUpper() + " (" + GameScenarioManager.Scenario.State.Difficulty.ToUpper() + ")");
      sb.AppendLine("Page {0} of {1}".F(Page + 1, pageTotal));
      sb.AppendLine("-------------------------------------------------------------");
      if (Page == 0)
      {
        ReportText.TextColor = ColorLocalization.Get(ColorLocalKeys.UI_TEXT);
        sb.Append("Score                                               ");
        sb.AppendLine(score.Score.ToString(" 00000000").PadLeft(9));
        sb.AppendLine();

        sb.Append("Total Hits on Enemy                                 ");
        sb.AppendLine(score.Hits.ToString(" #######0").PadLeft(9));
        sb.Append("Damage Inflicted on Enemy                           ");
        sb.AppendLine(score.DamageDealt.ToString(" ####0.00").PadLeft(9));
        sb.Append("Kills                                               ");
        sb.Append(score.Kills.ToString(" #######0").PadLeft(9));
        sb.AppendLine();

        sb.Append("Total Hits on Friendlies                            ");
        sb.AppendLine(score.FriendlyHits.ToString(" #######0").PadLeft(9));
        sb.Append("Damage Inflicted on Friendlies                      ");
        sb.AppendLine(score.FriendlyDamageDealt.ToString(" ####0.00").PadLeft(9));
        sb.Append("Kills                                               ");
        sb.Append(score.FriendlyKills.ToString(" #######0").PadLeft(9));
        sb.AppendLine();

        sb.Append("Damage Taken                                        ");
        sb.AppendLine(score.DamageTaken.ToString(" ####0.00").PadLeft(9));
        sb.Append("Deaths                                              ");
        sb.AppendLine(score.Deaths.ToString(" #######0").PadLeft(9));
        sb.AppendLine();

        sb.AppendLine();
      }
      else if (Page >= killsPageStart && Page < killsPageStart + killsPage)
      {
        ReportText.TextColor = ColorLocalization.Get((score.Kills <= 0) ? ColorLocalKeys.UI_TEXT : ColorLocalKeys.UI_TEXT_GOOD);
        UpdatePagePart(Page - killsPageStart, killsPage, LineLimit, "Personal Kills", score.Kills, " #######0", score.KillsByName, "x#######0");
      }
      else if (Page >= damagesPageStart && Page < damagesPageStart + damagesPage)
      {
        ReportText.TextColor = ColorLocalization.Get((score.DamageDealt <= 0) ? ColorLocalKeys.UI_TEXT : ColorLocalKeys.UI_TEXT_GOOD);
        UpdatePagePart(Page - damagesPageStart, damagesPage, LineLimit, "Damage Inflicted on Enemy", score.DamageDealt, " ####0.00", score.DamageDealtByName, " ####0.00");
      }
      else if (Page >= frienddamagesPageStart && Page < frienddamagesPageStart + frienddamagesPage)
      {
        ReportText.TextColor = ColorLocalization.Get((score.FriendlyDamageDealt <= 0) ? ColorLocalKeys.UI_TEXT_GOOD : ColorLocalKeys.UI_TEXT_BAD);
        UpdatePagePart(Page - frienddamagesPageStart, frienddamagesPage, LineLimit, "Damage Inflicted on Friendlies", score.FriendlyDamageDealt, " ####0.00", score.FriendlyDamageDealtByName, " ####0.00");
      }
      else if (Page >= friendkillsPageStart && Page < friendkillsPageStart + friendkillsPage)
      {
        ReportText.TextColor = ColorLocalization.Get((score.FriendlyKills <= 0) ? ColorLocalKeys.UI_TEXT_GOOD : ColorLocalKeys.UI_TEXT_BAD);
        UpdatePagePart(Page - friendkillsPageStart, friendkillsPage, LineLimit, "Friendly Loss by Your Actions", score.FriendlyKills, " #######0", score.FriendlyKillsByName, " #######0");
      }
      else if (Page >= damagedByPageStart && Page < damagedByPageStart + damagedByPage)
      {
        ReportText.TextColor = ColorLocalization.Get((score.DamageTaken <= 0) ? ColorLocalKeys.UI_TEXT_GOOD : ColorLocalKeys.UI_TEXT);
        UpdatePagePart(Page - damagedByPageStart, damagedByPage, LineLimit, "Damage Report", score.DamageTaken, " ####0.00", score.DamageTakenByName, " ####0.00");
      }
      else if (Page >= killedByPageStart && Page < killedByPageStart + killedByPage)
      {
        ReportText.TextColor = ColorLocalization.Get((score.Deaths <= 0) ? ColorLocalKeys.UI_TEXT_GOOD : ColorLocalKeys.UI_TEXT_BAD);
        UpdatePagePart(Page - killedByPageStart, killedByPage, LineLimit, "Deaths", score.Deaths, " #######0", score.KilledByName, "x#######0");
      }
      else if (Page >= eventsPageStart && Page < eventsPageStart + eventsPage)
      {
        ReportText.TextColor = ColorLocalization.Get(ColorLocalKeys.UI_TEXT);
        UpdatePageEvents(Page - eventsPageStart, eventsPage, 32, "Events                                         Damage    Score", score.Entries);
      }


      ReportText.Text = sb.ToString();
    }
  }
}
