//Source: https://www.c-sharpcorner.com/article/part-i-simple-color-syntax-code-editor-for-php-written-in-c/

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SWEndor.ScenarioEditor
{
  public partial class FlickerFreeRichEditTextBox : RichTextBox
  {
    const short WM_PAINT = 0x00f;
    public FlickerFreeRichEditTextBox()
    {
      InitializeComponent();
    }

    public bool _ignore = false;
    public bool _Paint = true;
    protected override void WndProc(ref System.Windows.Forms.Message m)
    {
      // Code courtesy of Mark Mihevc  
      // sometimes we want to eat the paint message so we don't have to see all the  
      // flicker from when we select the text to change the color.  
      if (m.Msg == WM_PAINT)
      {
        if (_Paint)
          base.WndProc(ref m); // if we decided to paint this control, just call the RichTextBox WndProc  
        else
          m.Result = IntPtr.Zero; // not painting, must set this to IntPtr.Zero if not painting therwise serious problems.  
      }
      else
        base.WndProc(ref m); // message other than WM_PAINT, jsut do what you normally do.  
    }

    [DllImport("user32.dll")]
    static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, Int32 wParam, Int32 lParam);
    const int WM_USER = 0x400;
    const int EM_HIDESELECTION = WM_USER + 63;
    public new void HideSelection(bool hide)
    {
      SendMessage(Handle, EM_HIDESELECTION, hide ? 1 : 0, 0);
    }

    private void tb_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Z)
      {
        _ignore = true;
        while (UndoActionName == "Unknown")
          Undo();

        Undo();
        e.Handled = true;
        _ignore = false;
      }
    }
  }
}
