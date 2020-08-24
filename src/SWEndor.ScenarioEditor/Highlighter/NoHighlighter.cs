using System.Drawing;

namespace SWEndor.ScenarioEditor
{
  public class NoHighlighter : IHighlighter
  {
    public static NoHighlighter Instance = new NoHighlighter();

    private NoHighlighter() { }

    public void Highlight(FlickerFreeRichEditTextBox box)
    {
      box._ignore = true;
      box._Paint = false;
      box.SuspendLayout();

      int p0 = box.SelectionStart;
      int p1 = box.SelectionLength;

      box.SelectAll();
      box.SelectionColor = Color.Black;

      box.SelectionStart = p0;
      box.SelectionLength = p1;

      box._Paint = true;
      box.ResumeLayout();
      box._ignore = false;
    }

    public void Highlight(FlickerFreeRichEditTextBox box, int line) { }
  }
}
