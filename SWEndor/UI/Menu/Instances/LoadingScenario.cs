using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.Primitives;
using SWEndor.Scenarios;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SWEndor.UI.Menu.Pages
{
  public class LoadingScenario : Page
  {
    SelectionElement Cover = new SelectionElement();
    SelectionElement LoadingText = new SelectionElement();
    ThreadSafeDictionary<SelectionElement, float> Squares = new ThreadSafeDictionary<SelectionElement, float>();
    TV_COLOR SquareColor1 = new TV_COLOR(0, 0, 0, 0.7f);
    TV_COLOR SquareColor2 = new TV_COLOR(0.8f, 0.8f, 0, 1);

    GameScenarioBase SelectedScenario = null;
    ActorTypeInfo SelectedActorTypeInfo = null;
    string SelectedDifficulty = null;
    bool Loaded = false;
    Exception LoadException = null;

    public LoadingScenario(GameScenarioBase selectedScenario, ActorTypeInfo selectedActorTypeInfo, string selectedDifficulty)
    {
      SelectedScenario = selectedScenario;
      SelectedActorTypeInfo = selectedActorTypeInfo;
      SelectedDifficulty = selectedDifficulty;

      Cover.HighlightBoxPosition = new TV_2DVECTOR();
      Cover.HighlightBoxWidth = Globals.Engine.ScreenWidth;
      Cover.HighlightBoxHeight = Globals.Engine.ScreenHeight;
      Cover.UnHighlightBoxPositionColor = new TV_COLOR(0, 0, 0, 0.3f);

      Globals.Engine.Screen2D.LoadingTextLines = new List<string> { "" };
      LoadingText.Text = PrintLoadingText();
      LoadingText.TextPosition = new TV_2DVECTOR(Globals.Engine.ScreenWidth * 0.2f, Globals.Engine.ScreenHeight * 0.35f);

      float di = 0.04f;
      float dj = 0.03f;
      for (float i = 0.2f; i < 0.8f; i += di)
        for (float j = 0.6f; j < 0.7f; j += dj)
        {
          SelectionElement sqi = new SelectionElement();
          sqi.HighlightBoxPosition = new TV_2DVECTOR(Globals.Engine.ScreenWidth * (i + 0.005f), Globals.Engine.ScreenHeight * (j + 0.005f));
          sqi.HighlightBoxWidth = Globals.Engine.ScreenWidth * (di - 0.01f);
          sqi.HighlightBoxHeight = Globals.Engine.ScreenHeight * (dj - 0.01f);
          sqi.UnHighlightBoxPositionColor = SquareColor1;
          Squares.Put(sqi, (float)Globals.Engine.Random.NextDouble());
        }

      Elements.Add(Cover);
      Elements.Add(LoadingText);
      foreach (SelectionElement sq in Squares.GetKeys())
        Elements.Add(sq);

      StartLoad();
    }

    private string PrintLoadingText()
    {
      return "Loading Scenario...\n" + string.Join("\n", Globals.Engine.Screen2D.LoadingTextLines.ToArray());
    }

    private void StartLoad()
    {
      //Globals.Engine.Game.IsPaused = true;

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

    public override void Tick()
    {
      base.Tick();
      if (Loaded)
        ScenarioLoaded();
      else if (LoadException != null)
        Globals.Engine.Screen2D.CurrentPage = new FatalError(LoadException);
      else
      {
        LoadingText.Text = PrintLoadingText();

        foreach (SelectionElement sq in Squares.GetKeys())
        {
          Squares[sq] -= Globals.Engine.Game.TimeSinceRender;
          if (Squares[sq] < 0)
          {
            if (sq.UnHighlightBoxPositionColor.GetIntColor() == SquareColor1.GetIntColor())
              sq.UnHighlightBoxPositionColor = SquareColor2;
            else
              sq.UnHighlightBoxPositionColor = SquareColor1;

            Squares[sq] += (float)Globals.Engine.Random.NextDouble();
          }
        }
      }
    }

    private void ScenarioLoaded()
    {
      Globals.Engine.Game.IsPaused = true;
      Globals.Engine.GameScenarioManager.Reset();

      Globals.Engine.SoundManager.SetSound("r23");
      Thread.Sleep(1200);

      SelectedScenario.Launch();
      Globals.Engine.Game.IsPaused = false;
      Globals.Engine.Screen2D.CurrentPage = null;
      Globals.Engine.Screen2D.ShowPage = false;
    }
  }
}
