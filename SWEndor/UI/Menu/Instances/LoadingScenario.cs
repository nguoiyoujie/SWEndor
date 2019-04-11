using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.Primitives;
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
      Cover.HighlightBoxWidth = Engine.Instance().ScreenWidth;
      Cover.HighlightBoxHeight = Engine.Instance().ScreenHeight;
      Cover.UnHighlightBoxPositionColor = new TV_COLOR(0, 0, 0, 0.3f);

      Screen2D.Instance().LoadingTextLines = new List<string> { "" };
      LoadingText.Text = PrintLoadingText();
      LoadingText.TextPosition = new TV_2DVECTOR(Engine.Instance().ScreenWidth * 0.2f, Engine.Instance().ScreenHeight * 0.35f);

      float di = 0.04f;
      float dj = 0.03f;
      for (float i = 0.2f; i < 0.8f; i += di)
        for (float j = 0.6f; j < 0.7f; j += dj)
        {
          SelectionElement sqi = new SelectionElement();
          sqi.HighlightBoxPosition = new TV_2DVECTOR(Engine.Instance().ScreenWidth * (i + 0.005f), Engine.Instance().ScreenHeight * (j + 0.005f));
          sqi.HighlightBoxWidth = Engine.Instance().ScreenWidth * (di - 0.01f);
          sqi.HighlightBoxHeight = Engine.Instance().ScreenHeight * (dj - 0.01f);
          sqi.UnHighlightBoxPositionColor = SquareColor1;
          Squares.Put(sqi, (float)Engine.Instance().Random.NextDouble());
        }

      Elements.Add(Cover);
      Elements.Add(LoadingText);
      foreach (SelectionElement sq in Squares.GetKeys())
        Elements.Add(sq);

      StartLoad();
    }

    private string PrintLoadingText()
    {
      return "Loading Scenario...\n" + string.Join("\n", Screen2D.Instance().LoadingTextLines.ToArray());
    }

    private void StartLoad()
    {
      //Game.Instance().IsPaused = true;

      Thread th_Load = new Thread(Load);
      th_Load.Start();
    }

    private void Load()
    {
      try
      {
        SelectedScenario.Load(SelectedActorTypeInfo, SelectedDifficulty);
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
        Screen2D.Instance().CurrentPage = new FatalError(LoadException);
      else
      {
        LoadingText.Text = PrintLoadingText();

        foreach (SelectionElement sq in Squares.GetKeys())
        {
          Squares[sq] -= Game.Instance().TimeSinceRender;
          if (Squares[sq] < 0)
          {
            if (sq.UnHighlightBoxPositionColor.GetIntColor() == SquareColor1.GetIntColor())
              sq.UnHighlightBoxPositionColor = SquareColor2;
            else
              sq.UnHighlightBoxPositionColor = SquareColor1;

            Squares[sq] += (float)Engine.Instance().Random.NextDouble();
          }
        }
      }
    }

    private void ScenarioLoaded()
    {
      GameScenarioManager.Instance().Reset();

      SoundManager.Instance().SetSound("r23");
      Thread.Sleep(1200);

      SelectedScenario.Launch();
      Game.Instance().IsPaused = false;
      Screen2D.Instance().CurrentPage = null;
      Screen2D.Instance().ShowPage = false;
    }
  }
}
