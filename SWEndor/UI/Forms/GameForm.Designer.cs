namespace SWEndor.UI.Forms
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
      ((System.ComponentModel.ISupportInitialize)(this.pbGame)).BeginInit();
      this.SuspendLayout();
      // 
      // pbGame
      // 
      this.pbGame.BackColor = System.Drawing.Color.Black;
      this.pbGame.Dock = System.Windows.Forms.DockStyle.Fill;
      this.pbGame.Location = new System.Drawing.Point(0, 0);
      this.pbGame.Name = "pbGame";
      this.pbGame.Size = new System.Drawing.Size(584, 561);
      this.pbGame.TabIndex = 0;
      this.pbGame.TabStop = false;
      this.pbGame.MouseEnter += new System.EventHandler(this.pbGame_MouseEnter);
      this.pbGame.MouseLeave += new System.EventHandler(this.pbGame_MouseLeave);
      // 
      // GameForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(5)))), ((int)(((byte)(5)))));
      this.ClientSize = new System.Drawing.Size(584, 561);
      this.Controls.Add(this.pbGame);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "GameForm";
      this.Text = "Game";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GameForm_FormClosing);
      this.Load += new System.EventHandler(this.GameForm_Load);
      ((System.ComponentModel.ISupportInitialize)(this.pbGame)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    internal System.Windows.Forms.PictureBox pbGame;
  }
}

