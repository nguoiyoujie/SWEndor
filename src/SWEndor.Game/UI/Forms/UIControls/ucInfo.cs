using System.Windows.Forms;
using Primrose.Primitives.Extensions;
using SWEndor.Game.Core;

namespace SWEndor.Game.UI.Forms.UIControls
{
  public partial class ucInfo : UserControl
  {
    public ucInfo()
    {
      InitializeComponent();

      lblGameVer.Text = Globals.Version;
      lblPrimroseVer.Text = "{0}.{1}".F(Primrose.Build.BuildDate, Primrose.Build.Revision);
      lblPrimExprVer.Text = "{0}.{1}".F(Primrose.Expressions.Build.BuildDate, Primrose.Expressions.Build.Revision);
    }

    public void Update(Engine engine)
    {
      lblFmodVer.Text = engine.SoundManager.Version;
    }
  }
}
