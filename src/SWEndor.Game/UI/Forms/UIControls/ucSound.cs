using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using SWEndor.Game.Core;
using static SWEndor.Game.Sound.SoundManager;

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
      string currMusic = engine.SoundManager.CurrMusic;
      lblCurrMusic.Text = currMusic;
      lblInterruptMusic.Text = engine.SoundManager.IntrMusic;
      lblPrevDynMusic.Text = engine.SoundManager.PrevDynMusic;

      if (_cacheCurrMusic != currMusic)
      {
        _nextsb.Clear();
        _next.Clear();
        Piece p = Piece.Factory.Get(currMusic);
        if (p != null)
        {
          foreach (var mt in p.MoodTransitions)
          {
            foreach (string s in mt.Value)
            {
              if (_next.ContainsKey(s))
              {
                if (!_next[s].Contains(mt.Key))
                {
                  _next[s].Add(mt.Key);
                }
              }
              else
              {
                _next.Add(s, new List<int>() { mt.Key });
              }
            }
          }

          foreach (var nxt in _next)
          {
            _nextsb.Append(nxt.Key + " (");
            _nextsb.Append(string.Join(",", nxt.Value));
            _nextsb.AppendLine(")");
          }

          lblNext.Text = _nextsb.ToString();
        }
        else
        {
          lblNext.Text = "NA";
        }
      }
    }

    private string _cacheCurrMusic = null;
    private StringBuilder _nextsb = new StringBuilder();
    private Dictionary<string, List<int>> _next = new Dictionary<string, List<int>>();
  }
}
