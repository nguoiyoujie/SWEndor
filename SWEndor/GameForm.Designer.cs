namespace SWEndor
{
  partial class GameForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameForm));
      this.pbGame = new System.Windows.Forms.PictureBox();
      this.lblVersion = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.pbGame)).BeginInit();
      this.SuspendLayout();
      // 
      // pbGame
      // 
      this.pbGame.BackColor = System.Drawing.Color.Black;
      this.pbGame.Location = new System.Drawing.Point(0, 0);
      this.pbGame.Name = "pbGame";
      this.pbGame.Size = new System.Drawing.Size(100, 100);
      this.pbGame.TabIndex = 0;
      this.pbGame.TabStop = false;
      this.pbGame.MouseEnter += new System.EventHandler(this.pbGame_MouseEnter);
      this.pbGame.MouseLeave += new System.EventHandler(this.pbGame_MouseLeave);
      // 
      // lblVersion
      // 
      this.lblVersion.AutoSize = true;
      this.lblVersion.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.lblVersion.Font = new System.Drawing.Font("Consolas", 10F);
      this.lblVersion.ForeColor = System.Drawing.Color.White;
      this.lblVersion.Location = new System.Drawing.Point(0, 544);
      this.lblVersion.Name = "lblVersion";
      this.lblVersion.Size = new System.Drawing.Size(72, 17);
      this.lblVersion.TabIndex = 1;
      this.lblVersion.Text = "v0.0.0.0";
      // 
      // GameForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(5)))), ((int)(((byte)(5)))));
      this.ClientSize = new System.Drawing.Size(584, 561);
      this.Controls.Add(this.lblVersion);
      this.Controls.Add(this.pbGame);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "GameForm";
      this.Text = "Game";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GameForm_FormClosing);
      this.Load += new System.EventHandler(this.GameForm_Load);
      ((System.ComponentModel.ISupportInitialize)(this.pbGame)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    internal System.Windows.Forms.PictureBox pbGame;
    private System.Windows.Forms.Label lblVersion;
  }
}

