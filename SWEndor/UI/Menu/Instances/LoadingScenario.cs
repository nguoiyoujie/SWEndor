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

    public LoadingScenario(Screen2D owner, GameScenarioBase selectedScenario, ActorTypeInfo selectedActorTypeInfo, string selectedDifficulty) : base(owner)
    {
      SelectedScenario = selectedScenario;
      SelectedActorTypeInfo = selectedActorTypeInfo;
      SelectedDifficulty = selectedDifficulty;

      Cover.HighlightBoxPosition = new TV_2DVECTOR();
      Cover.HighlightBoxWidth = this.GetEngine().ScreenWidth;
      Cover.HighlightBoxHeight = this.GetEngine().ScreenHeight;
      Cover.UnHighlightBoxPositionColor = new TV_COLOR(0, 0, 0, 0.3f);

      this.GetEngine().Screen2D.LoadingTextLines = new List<string> { "" };
      LoadingText.Text = PrintLoadingText();
      LoadingText.TextPosition = new TV_2DVECTOR(this.GetEngine().ScreenWidth * 0.2f, this.GetEngine().ScreenHeight * 0.35f);

      float di = 0.04f;
      float dj = 0.03f;
      for (float i = 0.2f; i < 0.8f; i += di)
        for (float j = 0.6f; j < 0.7f; j += dj)
        {
          SelectionElement sqi = new SelectionElement();
          sqi.HighlightBoxPosition = new TV_2DVECTOR(this.GetEngine().ScreenWidth * (i + 0.005f), this.GetEngine().ScreenHeight * (j + 0.005f));
          sqi.HighlightBoxWidth = this.GetEngine().ScreenWidth * (di - 0.01f);
          sqi.HighlightBoxHeight = this.GetEngine().ScreenHeight * (dj - 0.01f);
          sqi.UnHighlightBoxPositionColor = SquareColor1;
          Squares.Put(sqi, (float)this.GetEngine().Random.NextDouble());
        }

      Elements.Add(Cover);
      Elements.Add(LoadingText);
      foreach (SelectionElement sq in Squares.GetKeys())
        Elements.Add(sq);

      StartLoad();
    }

    private string PrintLoadingText()
    {
      return "Loading Scenario...\n" + string.Join("\n", this.GetEngine().Screen2D.LoadingTextLines.ToArray());
    }

    private void StartLoad()
    {
      //this.GetEngine().Game.IsPaused = true;

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
        this.GetEngine().Screen2D.CurrentPage = new FatalError(this.GetEngine().Screen2D, LoadException);
      else
      {
        LoadingText.Text = PrintLoadingText();

        foreach (SelectionElement sq in Squares.GetKeys())
        {
          Squares[sq] -= this.GetEngine().Game.TimeSinceRender;
          if (Squares[sq] < 0)
          {
            if (sq.UnHighlightBoxPositionColor.GetIntColor() == SquareColor1.GetIntColor())
              sq.UnHighlightBoxPositionColor = SquareColor2;
            else
              sq.UnHighlightBoxPositionColor = SquareColor1;

            Squares[sq] += (float)this.GetEngine().Random.NextDouble();
          }
        }
      }
    }

    private void ScenarioLoaded()
    {
      this.GetEngine().Game.IsPaused = true;
      this.GetEngine().GameScenarioManager.Reset();

      this.GetEngine().SoundManager.SetSound("r23");
      Thread.Sleep(1200);

      SelectedScenario.Launch();
      this.GetEngine().Game.IsPaused = false;
      this.GetEngine().Screen2D.CurrentPage = null;
      this.GetEngine().Screen2D.ShowPage = false;
    }
  }
}
