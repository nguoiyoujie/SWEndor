using System.Windows.Forms;
using SWEndor.Game.Core;

namespace SWEndor.Game.UI.Forms.UIControls
{
  public partial class ucSound : UserControl
  {
    public ucSound()
    {
      InitializeComponent();
    }

    public void Update(Engine engine)
    {
      lblDynPiece.Text = Sound.SoundManager.Piece.Factory.Count.ToString();
      lblMood.Text = engine.SoundManager.GetMood().ToString();
      lblCurrMusic.Text = engine.SoundManager.CurrMusic;
      lblInterruptMusic.Text = engine.SoundManager.IntrMusic;
      lblPrevDynMusic.Text = engine.SoundManager.PrevDynMusic;
    }
  }
}
