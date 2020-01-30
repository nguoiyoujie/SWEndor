namespace SWEndor.ScenarioEditor
{
  partial class ucMultipleItemBox
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.lboxList = new System.Windows.Forms.ListBox();
      this.bAdd = new System.Windows.Forms.Button();
      this.bRemove = new System.Windows.Forms.Button();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.SuspendLayout();
      // 
      // lboxList
      // 
      this.lboxList.Dock = System.Windows.Forms.DockStyle.Fill;
      this.lboxList.FormattingEnabled = true;
      this.lboxList.Location = new System.Drawing.Point(0, 0);
      this.lboxList.Name = "lboxList";
      this.lboxList.Size = new System.Drawing.Size(100, 113);
      this.lboxList.TabIndex = 0;
      this.lboxList.SelectedIndexChanged += new System.EventHandler(this.lboxList_SelectedIndexChanged);
      // 
      // bAdd
      // 
      this.bAdd.Dock = System.Windows.Forms.DockStyle.Right;
      this.bAdd.Location = new System.Drawing.Point(34, 0);
      this.bAdd.Name = "bAdd";
      this.bAdd.Size = new System.Drawing.Size(33, 33);
      this.bAdd.TabIndex = 1;
      this.bAdd.Text = "+";
      this.bAdd.UseVisualStyleBackColor = true;
      this.bAdd.Click += new System.EventHandler(this.bAdd_Click);
      // 
      // bRemove
      // 
      this.bRemove.Dock = System.Windows.Forms.DockStyle.Right;
      this.bRemove.Enabled = false;
      this.bRemove.Location = new System.Drawing.Point(67, 0);
      this.bRemove.Name = "bRemove";
      this.bRemove.Size = new System.Drawing.Size(33, 33);
      this.bRemove.TabIndex = 2;
      this.bRemove.Text = "-";
      this.bRemove.UseVisualStyleBackColor = true;
      this.bRemove.Click += new System.EventHandler(this.bRemove_Click);
      // 
      // splitContainer1
      // 
      this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer1.IsSplitterFixed = true;
      this.splitContainer1.Location = new System.Drawing.Point(0, 0);
      this.splitContainer1.Name = "splitContainer1";
      this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.lboxList);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.bAdd);
      this.splitContainer1.Panel2.Controls.Add(this.bRemove);
      this.splitContainer1.Panel2MinSize = 33;
      this.splitContainer1.Size = new System.Drawing.Size(100, 150);
      this.splitContainer1.SplitterDistance = 113;
      this.splitContainer1.TabIndex = 3;
      // 
      // ucMultipleItemBox
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.splitContainer1);
      this.Name = "ucMultipleItemBox";
      this.Size = new System.Drawing.Size(100, 150);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ListBox lboxList;
    private System.Windows.Forms.Button bAdd;
    private System.Windows.Forms.Button bRemove;
    private System.Windows.Forms.SplitContainer splitContainer1;
  }
}
