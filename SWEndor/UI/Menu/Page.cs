using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.AI;
using SWEndor.Player;
using SWEndor.Scenarios;
using SWEndor.Sound;
using System.Collections.Generic;

namespace SWEndor.UI.Menu
{
  public class Page
  {
    public List<SelectionElement> Elements = new List<SelectionElement>();
    public Page Parent = null;
    public int SelectedElementID = -1;

    public Page(Screen2D owner)
    {
      Owner = owner;
    }
    public readonly Screen2D Owner;
    public Engine Engine { get { return Owner.Engine; } }
    public Game Game { get { return Engine.Game; } }
    public GameScenarioManager GameScenarioManager { get { return Engine.GameScenarioManager; } }
    public TrueVision TrueVision { get { return Engine.TrueVision; } }
    public Font.Factory FontFactory { get { return Engine.FontFactory; } }
    public ActorInfo.Factory ActorFactory { get { return Engine.ActorFactory; } }
    public ActorTypeInfo.Factory ActorTypeFactory { get { return Engine.ActorTypeFactory; } }
    public ActionManager ActionManager { get { return Engine.ActionManager; } }
    public SoundManager SoundManager { get { return Engine.SoundManager; } }
    public LandInfo LandInfo { get { return Engine.LandInfo; } }
    public AtmosphereInfo AtmosphereInfo { get { return Engine.AtmosphereInfo; } }
    public PlayerInfo PlayerInfo { get { return Engine.PlayerInfo; } }
    public PlayerCameraInfo PlayerCameraInfo { get { return Engine.PlayerCameraInfo; } }
    public Screen2D Screen2D { get { return Engine.Screen2D; } }
    public Scenarios.Scripting.Expressions.Context ScriptContext { get { return Engine.ScriptContext; } }

    public TVScreen2DImmediate TVScreen2DImmediate { get { return Engine.TrueVision.TVScreen2DImmediate; } }
    public TVScreen2DText TVScreen2DText { get { return Engine.TrueVision.TVScreen2DText; } }

    public virtual void Show()
    {
      SelectionElement SelectedElement = null;
      if (SelectedElementID >= 0 && SelectedElementID < Elements.Count)
        SelectedElement = Elements[SelectedElementID];

      foreach (SelectionElement se in Elements)
        se.Show(Engine, (se == SelectedElement));
    }

    public virtual void Tick()
    {
    }

    public virtual void SelectNext()
    {
      int newSelectedElementID = SelectedElementID;
      int count = Elements.Count;
      bool found = false;

      while (!found && count > 0)
      {
        newSelectedElementID++;
        if (newSelectedElementID >= Elements.Count)
          newSelectedElementID = 0;

        SelectionElement newSelectedElement = Elements[newSelectedElementID];
         
        if (newSelectedElement.Selectable)
        {
          SelectedElementID = newSelectedElementID;
          found = true;
        }

        count--;
      }
    }

    public virtual void SelectPrev()
    {
      int newSelectedElementID = SelectedElementID;
      int count = Elements.Count;
      bool found = false;

      while (!found && count > 0)
      {
        newSelectedElementID--;
        if (newSelectedElementID < 0)
          newSelectedElementID = Elements.Count - 1;

        SelectionElement newSelectedElement = Elements[newSelectedElementID];

        if (newSelectedElement.Selectable)
        {
          SelectedElementID = newSelectedElementID;
          found = true;
        }

        count--;
      }
    }

    public virtual bool Back()
    {
      if (Parent != null)
      {
        Screen2D.CurrentPage = Parent;
        return true;
      }
      return false;
    }

    public virtual void EnterPage(Page page)
    {
      page.Parent = this;
      Screen2D.CurrentPage = page;
    }

    public virtual bool OnKeyPress(CONST_TV_KEY key)
    {
      SelectionElement SelectedElement = null;
      if (SelectedElementID > 0 && SelectedElementID < Elements.Count)
        SelectedElement = Elements[SelectedElementID];

      if (SelectedElement == null)
        return false;

      if (SelectedElement.OnKeyPress != null && SelectedElement.OnKeyPress(key))
        return true;

      bool ret = false;
      switch (key)
      {
        case CONST_TV_KEY.TV_KEY_UP:
          SelectPrev();
          ret = true;
          break;
        case CONST_TV_KEY.TV_KEY_DOWN:
          SelectNext();
          ret = true;
          break;
        case CONST_TV_KEY.TV_KEY_LEFT:
          ret = SelectedElement.DecrementToggleButtonNumber();
          break;
        case CONST_TV_KEY.TV_KEY_RIGHT:
          ret = SelectedElement.IncrementToggleButtonNumber();
          break;
        case CONST_TV_KEY.TV_KEY_ESCAPE:
          ret = Back();
          break;
      }

      SelectedElement.AfterKeyPress?.Invoke(key);

      return ret;
    }
  }
}
