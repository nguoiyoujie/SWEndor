namespace SWEndor.Game.UI.Forms.UIControls
{
  partial class ucShaderCache
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
      this.lblInUse = new System.Windows.Forms.Label();
      this.lblPool = new System.Windows.Forms.Label();
      this.label12 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // lblInUse
      // 
      this.lblInUse.Location = new System.Drawing.Point(296, 10);
      this.lblInUse.Name = "lblInUse";
      this.lblInUse.Size = new System.Drawing.Size(60, 15);
      this.lblInUse.TabIndex = 105;
      this.lblInUse.Text = "In Use";
      // 
      // lblPool
      // 
      this.lblPool.Location = new System.Drawing.Point(230, 10);
      this.lblPool.Name = "lblPool";
      this.lblPool.Size = new System.Drawing.Size(60, 15);
      this.lblPool.TabIndex = 97;
      this.lblPool.Text = "Pool";
      // 
      // label12
      // 
      this.label12.Location = new System.Drawing.Point(10, 10);
      this.label12.Name = "label12";
      this.label12.Size = new System.Drawing.Size(180, 15);
      this.label12.TabIndex = 69;
      this.label12.Text = "Shader Utilization";
      // 
      // ucShaderCache
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.lblInUse);
      this.Controls.Add(this.lblPool);
      this.Controls.Add(this.label12);
      this.Name = "ucShaderCache";
      this.Size = new System.Drawing.Size(569, 359);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label lblInUse;
    private System.Windows.Forms.Label lblPool;
    private System.Windows.Forms.Label label12;
  }
}
