using MTV3D65;
using System.Collections.Generic;

namespace SWEndor.UI
{
  public class UIPage
  {
    public UIPage()
    {

    }

    public List<UISelectionElement> Elements = new List<UISelectionElement>();
    public UIPage Parent = null;
    public int SelectedElementID = -1;

    public virtual void Show()
    {
      UISelectionElement SelectedElement = null;
      if (SelectedElementID >= 0 && SelectedElementID < Elements.Count)
        SelectedElement = Elements[SelectedElementID];

      foreach (UISelectionElement se in Elements)
      {
        se.Show((se == SelectedElement));
      }
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

        UISelectionElement newSelectedElement = Elements[newSelectedElementID];
         
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

        UISelectionElement newSelectedElement = Elements[newSelectedElementID];

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
        Screen2D.Instance().CurrentPage = Parent;
        return true;
      }
      return false;
    }

    public virtual void EnterPage(UIPage page)
    {
      page.Parent = this;
      Screen2D.Instance().CurrentPage = page;
    }

    public virtual bool OnKeyPress(CONST_TV_KEY key)
    {
      UISelectionElement SelectedElement = null;
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

      if (SelectedElement.AfterKeyPress != null)
        SelectedElement.AfterKeyPress(key);

      return ret;
    }
  }
}
