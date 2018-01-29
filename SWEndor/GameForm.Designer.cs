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
      this.SuspendLayout();
      // 
      // GameForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(584, 561);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.Name = "GameForm";
      this.Text = "Game";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GameForm_FormClosing);
      this.Load += new System.EventHandler(this.GameForm_Load);
      this.MouseEnter += new System.EventHandler(this.GameForm_MouseEnter);
      this.MouseLeave += new System.EventHandler(this.GameForm_MouseLeave);
      this.ResumeLayout(false);

    }

    #endregion
  }
}

