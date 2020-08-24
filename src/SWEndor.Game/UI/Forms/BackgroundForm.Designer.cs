namespace SWEndor.Game.UI.Forms
{
  partial class BackgroundForm
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
      this.components = new System.ComponentModel.Container();
      this.lblVersion = new System.Windows.Forms.Label();
      this.gameTimer = new System.Windows.Forms.Timer(this.components);
      this.SuspendLayout();
      // 
      // lblVersion
      // 
      this.lblVersion.AutoSize = true;
      this.lblVersion.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.lblVersion.Font = new System.Drawing.Font("Consolas", 10F);
      this.lblVersion.ForeColor = System.Drawing.Color.White;
      this.lblVersion.Location = new System.Drawing.Point(0, 439);
      this.lblVersion.Name = "lblVersion";
      this.lblVersion.Size = new System.Drawing.Size(72, 17);
      this.lblVersion.TabIndex = 2;
      this.lblVersion.Text = "v0.0.0.0";
      // 
      // gameTimer
      // 
      this.gameTimer.Enabled = true;
      this.gameTimer.Interval = 500;
      this.gameTimer.Tick += new System.EventHandler(this.gameTimer_Tick);
      // 
      // BackgroundForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(5)))), ((int)(((byte)(5)))));
      this.ClientSize = new System.Drawing.Size(603, 456);
      this.ControlBox = false;
      this.Controls.Add(this.lblVersion);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.Name = "BackgroundForm";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Game";
      this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
      this.Load += new System.EventHandler(this.BackgroundForm_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lblVersion;
    private System.Windows.Forms.Timer gameTimer;
  }
}