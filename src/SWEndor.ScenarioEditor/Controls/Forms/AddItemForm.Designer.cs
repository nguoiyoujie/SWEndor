namespace SWEndor.ScenarioEditor.Controls.Forms
{
  partial class AddItemForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.tbItem = new System.Windows.Forms.TextBox();
      this.bAdd = new System.Windows.Forms.Button();
      this.bCancel = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // tbItem
      // 
      this.tbItem.Location = new System.Drawing.Point(12, 12);
      this.tbItem.Name = "tbItem";
      this.tbItem.Size = new System.Drawing.Size(160, 20);
      this.tbItem.TabIndex = 0;
      this.tbItem.TextChanged += new System.EventHandler(this.tbItem_TextChanged);
      // 
      // bAdd
      // 
      this.bAdd.Enabled = false;
      this.bAdd.Location = new System.Drawing.Point(30, 38);
      this.bAdd.Name = "bAdd";
      this.bAdd.Size = new System.Drawing.Size(68, 29);
      this.bAdd.TabIndex = 1;
      this.bAdd.Text = "Add";
      this.bAdd.UseVisualStyleBackColor = true;
      this.bAdd.Click += new System.EventHandler(this.bAdd_Click);
      // 
      // bCancel
      // 
      this.bCancel.Location = new System.Drawing.Point(104, 38);
      this.bCancel.Name = "bCancel";
      this.bCancel.Size = new System.Drawing.Size(68, 29);
      this.bCancel.TabIndex = 2;
      this.bCancel.Text = "Cancel";
      this.bCancel.UseVisualStyleBackColor = true;
      this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
      // 
      // AddItemForm
      // 
      this.AcceptButton = this.bAdd;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.bCancel;
      this.ClientSize = new System.Drawing.Size(184, 73);
      this.Controls.Add(this.bCancel);
      this.Controls.Add(this.bAdd);
      this.Controls.Add(this.tbItem);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.Name = "AddItemForm";
      this.Text = "Add Item";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox tbItem;
    private System.Windows.Forms.Button bAdd;
    private System.Windows.Forms.Button bCancel;
  }
}