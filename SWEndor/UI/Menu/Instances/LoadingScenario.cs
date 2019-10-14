using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.Scenarios;
using SWEndor.Sound;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SWEndor.UI.Menu.Pages
{
  public class LoadingScenario : Page
  {
    SelectionElement Cover = new SelectionElement();
    SelectionElement LoadingText = new SelectionElement();
    List<float> Vals = new List<float>();
    List<SelectionElement> Squares = new List<SelectionElement>();
    int SquareColor1 = ColorLocalization.Get(ColorLocalKeys.GAME_LOAD_DARK);
    int SquareColor2 = ColorLocalization.Get(ColorLocalKeys.GAME_LOAD_LIGHT);

    GameScenarioBase SelectedScenario = null;
    ActorTypeInfo SelectedActorTypeInfo = null;
    string SelectedDifficulty = null;
    bool Loaded = false;
    Exception LoadException = null;

    public LoadingScenario(Screen2D owner, GameScenarioBase selectedScenario, ActorTypeInfo selectedActorTypeInfo, string selectedDifficulty) : base(owner)
    {
      SelectedScenario = selectedScenario;
      SelectedActorTypeInfo = selectedActorTypeInfo;
      SelectedDifficulty = selectedDifficulty;

      Cover.HighlightBoxPosition = new TV_2DVECTOR();
      Cover.HighlightBoxWidth = owner.ScreenSize.x;
      Cover.HighlightBoxHeight = owner.ScreenSize.y;
      Cover.UnHighlightBoxColor = ColorLocalization.Get(ColorLocalKeys.UI_UNHIGHLIGHT_BACKGROUND);

      Engine.Screen2D.LoadingTextLines = new List<string> { "" };
      LoadingText.Text = PrintLoadingText();
      LoadingText.TextPosition = new TV_2DVECTOR(owner.ScreenSize.x * 0.2f, owner.ScreenSize.y * 0.35f);

      float di = 0.04f;
      float dj = 0.03f;
      for (float i = 0.2f; i < 0.8f; i += di)
        for (float j = 0.6f; j < 0.7f; j += dj)
        {
          SelectionElement sqi = new SelectionElement();
          sqi.HighlightBoxPosition = new TV_2DVECTOR(owner.ScreenSize.x * (i + 0.005f), owner.ScreenSize.y * (j + 0.005f));
          sqi.HighlightBoxWidth = owner.ScreenSize.x * (di - 0.01f);
          sqi.HighlightBoxHeight = owner.ScreenSize.y * (dj - 0.01f);
          sqi.UnHighlightBoxColor = SquareColor1;
          Squares.Add(sqi);
          Vals.Add((float)Engine.Random.NextDouble());
        }

      Elements.Add(Cover);
      Elements.Add(LoadingText);
      foreach (SelectionElement sq in Squares)
        Elements.Add(sq);

      StartLoad();
    }

    private string PrintLoadingText()
    {
      return "Loading Scenario...\n" + string.Join("\n", Engine.Screen2D.LoadingTextLines.ToArray());
    }

    private void StartLoad()
    {
      //Engine.Game.IsPaused = true;

      Thread th_Load = new Thread(Load);
      th_Load.Start();
    }

    private void Load()
    {
      try
      {
        SelectedScenario.Load(SelectedActorTypeInfo, SelectedDifficulty);
        Thread.Sleep(500);
        Loaded = true;
      }
      catch (Exception ex)
      {
        LoadException = ex;
      }
    }

    public override void ProcessTick()
    {
      base.ProcessTick();
      if (Loaded)
        ScenarioLoaded();
      else if (LoadException != null)
        Engine.Screen2D.CurrentPage = new FatalError(Engine.Screen2D, LoadException);
      else
      {
        LoadingText.Text = PrintLoadingText();

        for (int i = 0; i < Squares.Count; i++)
        {
          Vals[i] -= Engine.Game.TimeSinceRender;
          if (Vals[i] < 0)
          {
            if (Squares[i].UnHighlightBoxColor == SquareColor1)
              Squares[i].UnHighlightBoxColor = SquareColor2;
            else
              Squares[i].UnHighlightBoxColor = SquareColor1;

            Vals[i] += (float)Engine.Random.NextDouble();
          }
        }
      }
    }

    private void ScenarioLoaded()
    {
      Engine.Game.IsPaused = true;
      GameScenarioManager.Reset();

      Engine.SoundManager.SetSound(SoundGlobals.Ready);
      Thread.Sleep(1200);

      SelectedScenario.Launch();
      Engine.Game.IsPaused = false;
      Engine.Screen2D.CurrentPage = null;
      Engine.Screen2D.ShowPage = false;
    }
  }
}
