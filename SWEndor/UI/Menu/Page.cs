using MTV3D65;
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

    public virtual void Show()
    {
      SelectionElement SelectedElement = null;
      if (SelectedElementID >= 0 && SelectedElementID < Elements.Count)
        SelectedElement = Elements[SelectedElementID];

      foreach (SelectionElement se in Elements)
        se.Show((se == SelectedElement));
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
        Globals.Engine.Screen2D.CurrentPage = Parent;
        return true;
      }
      return false;
    }

    public virtual void EnterPage(Page page)
    {
      page.Parent = this;
      Globals.Engine.Screen2D.CurrentPage = page;
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
