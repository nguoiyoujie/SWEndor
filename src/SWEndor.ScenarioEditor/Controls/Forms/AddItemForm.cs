using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SWEndor.ScenarioEditor.Controls.Forms
{
  public partial class AddItemForm : Form
  {
    public AddItemForm()
    {
      InitializeComponent();
    }

    public string Item;

    private void bAdd_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.OK;
    }

    private void bCancel_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.Cancel;
    }

    private void tbItem_TextChanged(object sender, EventArgs e)
    {
      bAdd.Enabled = tbItem.Text.Length > 0;
    }
  }
}
