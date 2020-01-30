namespace SWEndor.ScenarioEditor
{
  partial class tpEditor
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
      this.splitC = new System.Windows.Forms.SplitContainer();
      this.rtb = new SWEndor.ScenarioEditor.FlickerFreeRichEditTextBox();
      this.pLineNum = new System.Windows.Forms.PictureBox();
      this.rtb_output = new SWEndor.ScenarioEditor.FlickerFreeRichEditTextBox();
      this.ofd = new System.Windows.Forms.OpenFileDialog();
      this.sfd = new System.Windows.Forms.SaveFileDialog();
      ((System.ComponentModel.ISupportInitialize)(this.splitC)).BeginInit();
      this.splitC.Panel1.SuspendLayout();
      this.splitC.Panel2.SuspendLayout();
      this.splitC.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pLineNum)).BeginInit();
      this.SuspendLayout();
      // 
      // splitC
      // 
      this.splitC.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitC.Location = new System.Drawing.Point(0, 0);
      this.splitC.Name = "splitC";
      this.splitC.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitC.Panel1
      // 
      this.splitC.Panel1.Controls.Add(this.rtb);
      this.splitC.Panel1.Controls.Add(this.pLineNum);
      // 
      // splitC.Panel2
      // 
      this.splitC.Panel2.Controls.Add(this.rtb_output);
      this.splitC.Size = new System.Drawing.Size(200, 100);
      this.splitC.SplitterDistance = 71;
      this.splitC.TabIndex = 3;
      // 
      // rtb
      // 
      this.rtb.AcceptsTab = true;
      this.rtb.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.rtb.Dock = System.Windows.Forms.DockStyle.Fill;
      this.rtb.EnableAutoDragDrop = true;
      this.rtb.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.rtb.Location = new System.Drawing.Point(36, 0);
      this.rtb.Name = "rtb";
      this.rtb.Size = new System.Drawing.Size(164, 71);
      this.rtb.TabIndex = 0;
      this.rtb.Text = "";
      this.rtb.WordWrap = false;
      this.rtb.SelectionChanged += new System.EventHandler(this.rtb_SelectionChanged);
      this.rtb.VScroll += new System.EventHandler(this.rtb_VScroll);
      this.rtb.SizeChanged += new System.EventHandler(this.rtb_SizeChanged);
      this.rtb.TextChanged += new System.EventHandler(this.rtb_TextChanged);
      // 
      // pLineNum
      // 
      this.pLineNum.BackColor = System.Drawing.SystemColors.Window;
      this.pLineNum.Dock = System.Windows.Forms.DockStyle.Left;
      this.pLineNum.Location = new System.Drawing.Point(0, 0);
      this.pLineNum.Name = "pLineNum";
      this.pLineNum.Size = new System.Drawing.Size(36, 71);
      this.pLineNum.TabIndex = 1;
      this.pLineNum.TabStop = false;
      this.pLineNum.Paint += new System.Windows.Forms.PaintEventHandler(this.pLineNum_Paint);
      // 
      // rtb_output
      // 
      this.rtb_output.AcceptsTab = true;
      this.rtb_output.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.rtb_output.Dock = System.Windows.Forms.DockStyle.Fill;
      this.rtb_output.EnableAutoDragDrop = true;
      this.rtb_output.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.rtb_output.Location = new System.Drawing.Point(0, 0);
      this.rtb_output.Name = "rtb_output";
      this.rtb_output.ReadOnly = true;
      this.rtb_output.Size = new System.Drawing.Size(200, 25);
      this.rtb_output.TabIndex = 2;
      this.rtb_output.Text = "";
      this.rtb_output.WordWrap = false;
      // 
      // ofd
      // 
      this.ofd.Filter = "Script files|*.sw|Scenario files|*.scen|INI files|*.ini|All files|*.*";
      // 
      // sfd
      // 
      this.sfd.Filter = "Script files|*.sw|Scenario files|*.scen|INI files|*.ini|All files|*.*";
      // 
      // tpEditor
      // 
      this.Controls.Add(this.splitC);
      this.splitC.Panel1.ResumeLayout(false);
      this.splitC.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitC)).EndInit();
      this.splitC.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.pLineNum)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.SplitContainer splitC;
    private FlickerFreeRichEditTextBox rtb;
    private System.Windows.Forms.PictureBox pLineNum;
    private FlickerFreeRichEditTextBox rtb_output;
    private System.Windows.Forms.OpenFileDialog ofd;
    private System.Windows.Forms.SaveFileDialog sfd;
  }
}
