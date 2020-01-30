using System;
using System.Windows.Forms;
using SWEndor.ScenarioEditor.Controls.Forms;

namespace SWEndor.ScenarioEditor
{
  public partial class ucMultipleItemBox : UserControl
  {
    public ucMultipleItemBox()
    {
      InitializeComponent();
      bRemove.Enabled = false;
    }

    private void bAdd_Click(object sender, EventArgs e)
    {
      AddItemForm aif = new AddItemForm();
      if (aif.ShowDialog() == DialogResult.OK)
      {
        lboxList.Items.Add(aif.Item);
      }
    }

    private void bRemove_Click(object sender, EventArgs e)
    {
      if (lboxList.SelectedIndex > -1)
      {
        lboxList.Items.Remove(lboxList.SelectedItem);
        lboxList.SelectedIndex = -1;
      }
    }

    private void lboxList_SelectedIndexChanged(object sender, EventArgs e)
    {
      bRemove.Enabled = lboxList.SelectedIndex > -1;
    }
  }
}
