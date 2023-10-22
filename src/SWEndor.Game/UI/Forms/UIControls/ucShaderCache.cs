using System.Windows.Forms;
using SWEndor.Game.Core;
using Primrose.Primitives.Factories;
using System.Collections.Generic;
using Primrose.Primitives.ValueTypes;

namespace SWEndor.Game.UI.Forms.UIControls
{
  public partial class ucShaderCache : UserControl
  {
    private class PairCollection<T, U> : List<Pair<T, U>>
    {
      public void Add(T t, U u)
      {
        Pair<T, U> item = new Pair<T, U>(t, u);
        Add(item);
      }
    }

    public ucShaderCache()
    {
      InitializeComponent();
    }

    public void Init(Engine engine)
    {
      PairCollection<string, IPool> pools = new PairCollection<string, IPool>();
      foreach (var kvp in engine.ShaderFactory)
      {
        if (kvp.Value.Pool != null)
        {
          pools.Add(kvp.Key, kvp.Value.Pool);
        }
      }

      int y = 30;
      foreach (Pair<string, IPool> kvp in pools)
      {
        Label txt = new Label();
        txt.Location = new System.Drawing.Point(44, y);
        txt.Size = new System.Drawing.Size(180, 15);
        txt.Text = kvp.t;
        //44, 232, 276

        Label count1 = new Label();
        count1.Location = new System.Drawing.Point(lblPool.Location.X, y);
        count1.Size = new System.Drawing.Size(lblPool.Size.Width, 15);
        count1.Text = "0";

        Label count2 = new Label();
        count2.Location = new System.Drawing.Point(lblInUse.Location.X, y);
        count2.Size = new System.Drawing.Size(lblInUse.Size.Width, 15);
        count2.Text = "0";

        Controls.Add(txt);
        Controls.Add(count1);
        Controls.Add(count2);
        counters.Add(new Pair<Label, Label>(count1, count2), kvp.u);

        y += 15;
      }
    }

    Dictionary<Pair<Label, Label>, IPool> counters = new Dictionary<Pair<Label, Label>, IPool>();

    public void Update(Engine engine)
    {
      foreach (KeyValuePair<Pair<Label, Label>, IPool> kvp in counters)
      {
        kvp.Key.t.Text = kvp.Value.Count.ToString();
        kvp.Key.u.Text = kvp.Value.UncollectedCount.ToString();
      }
    }
  }
}
