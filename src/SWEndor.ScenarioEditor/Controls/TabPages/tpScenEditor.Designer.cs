namespace SWEndor.ScenarioEditor
{
  partial class tpScenEditor
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
      this.ofd = new System.Windows.Forms.OpenFileDialog();
      this.sfd = new System.Windows.Forms.SaveFileDialog();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.tb_General_Desc = new System.Windows.Forms.TextBox();
      this.mboxGeneral_Difficulty = new SWEndor.ScenarioEditor.ucMultipleItemBox();
      this.mboxGeneral_Wings = new SWEndor.ScenarioEditor.ucMultipleItemBox();
      this.tbGeneral_PlayerName = new System.Windows.Forms.TextBox();
      this.tbGeneral_Name = new System.Windows.Forms.TextBox();
      this.label5 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.tbAudio_Win = new System.Windows.Forms.TextBox();
      this.tbAudio_Lose = new System.Windows.Forms.TextBox();
      this.label9 = new System.Windows.Forms.Label();
      this.label10 = new System.Windows.Forms.Label();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.label7 = new System.Windows.Forms.Label();
      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.groupBox3.SuspendLayout();
      this.SuspendLayout();
      // 
      // ofd
      // 
      this.ofd.Filter = "Script files|*.sw|Scenario files|*.scen|INI files|*.ini|All files|*.*";
      // 
      // sfd
      // 
      this.sfd.Filter = "Script files|*.sw|Scenario files|*.scen|INI files|*.ini|All files|*.*";
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.tb_General_Desc);
      this.groupBox1.Controls.Add(this.mboxGeneral_Difficulty);
      this.groupBox1.Controls.Add(this.mboxGeneral_Wings);
      this.groupBox1.Controls.Add(this.tbGeneral_PlayerName);
      this.groupBox1.Controls.Add(this.tbGeneral_Name);
      this.groupBox1.Controls.Add(this.label5);
      this.groupBox1.Controls.Add(this.label4);
      this.groupBox1.Controls.Add(this.label3);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Location = new System.Drawing.Point(15, 15);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(500, 317);
      this.groupBox1.TabIndex = 1;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "General";
      // 
      // tb_General_Desc
      // 
      this.tb_General_Desc.AcceptsReturn = true;
      this.tb_General_Desc.AcceptsTab = true;
      this.tb_General_Desc.Location = new System.Drawing.Point(96, 201);
      this.tb_General_Desc.Multiline = true;
      this.tb_General_Desc.Name = "tb_General_Desc";
      this.tb_General_Desc.Size = new System.Drawing.Size(300, 97);
      this.tb_General_Desc.TabIndex = 9;
      // 
      // mboxGeneral_Difficulty
      // 
      this.mboxGeneral_Difficulty.Location = new System.Drawing.Point(296, 75);
      this.mboxGeneral_Difficulty.Name = "mboxGeneral_Difficulty";
      this.mboxGeneral_Difficulty.Size = new System.Drawing.Size(100, 120);
      this.mboxGeneral_Difficulty.TabIndex = 8;
      // 
      // mboxGeneral_Wings
      // 
      this.mboxGeneral_Wings.Location = new System.Drawing.Point(96, 75);
      this.mboxGeneral_Wings.Name = "mboxGeneral_Wings";
      this.mboxGeneral_Wings.Size = new System.Drawing.Size(100, 120);
      this.mboxGeneral_Wings.TabIndex = 7;
      // 
      // tbGeneral_PlayerName
      // 
      this.tbGeneral_PlayerName.Location = new System.Drawing.Point(96, 49);
      this.tbGeneral_PlayerName.Name = "tbGeneral_PlayerName";
      this.tbGeneral_PlayerName.Size = new System.Drawing.Size(100, 20);
      this.tbGeneral_PlayerName.TabIndex = 6;
      // 
      // tbGeneral_Name
      // 
      this.tbGeneral_Name.Location = new System.Drawing.Point(96, 26);
      this.tbGeneral_Name.Name = "tbGeneral_Name";
      this.tbGeneral_Name.Size = new System.Drawing.Size(300, 20);
      this.tbGeneral_Name.TabIndex = 5;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(18, 202);
      this.label5.Margin = new System.Windows.Forms.Padding(15, 5, 15, 5);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(60, 13);
      this.label5.TabIndex = 4;
      this.label5.Text = "Description";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(243, 75);
      this.label4.Margin = new System.Windows.Forms.Padding(15, 5, 15, 5);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(47, 13);
      this.label4.TabIndex = 3;
      this.label4.Text = "Difficulty";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(18, 75);
      this.label3.Margin = new System.Windows.Forms.Padding(15, 5, 15, 5);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(37, 13);
      this.label3.TabIndex = 2;
      this.label3.Text = "Wings";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(18, 52);
      this.label2.Margin = new System.Windows.Forms.Padding(15, 5, 15, 5);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(67, 13);
      this.label2.TabIndex = 1;
      this.label2.Text = "Player Name";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(18, 29);
      this.label1.Margin = new System.Windows.Forms.Padding(15, 5, 15, 5);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(35, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Name";
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.tbAudio_Win);
      this.groupBox2.Controls.Add(this.tbAudio_Lose);
      this.groupBox2.Controls.Add(this.label9);
      this.groupBox2.Controls.Add(this.label10);
      this.groupBox2.Location = new System.Drawing.Point(15, 338);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(500, 89);
      this.groupBox2.TabIndex = 2;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Audio";
      // 
      // tbAudio_Win
      // 
      this.tbAudio_Win.Location = new System.Drawing.Point(96, 49);
      this.tbAudio_Win.Name = "tbAudio_Win";
      this.tbAudio_Win.Size = new System.Drawing.Size(300, 20);
      this.tbAudio_Win.TabIndex = 6;
      // 
      // tbAudio_Lose
      // 
      this.tbAudio_Lose.Location = new System.Drawing.Point(96, 26);
      this.tbAudio_Lose.Name = "tbAudio_Lose";
      this.tbAudio_Lose.Size = new System.Drawing.Size(300, 20);
      this.tbAudio_Lose.TabIndex = 5;
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(18, 52);
      this.label9.Margin = new System.Windows.Forms.Padding(15, 5, 15, 5);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(26, 13);
      this.label9.TabIndex = 1;
      this.label9.Text = "Win";
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Location = new System.Drawing.Point(18, 29);
      this.label10.Margin = new System.Windows.Forms.Padding(15, 5, 15, 5);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(30, 13);
      this.label10.TabIndex = 0;
      this.label10.Text = "Lose";
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.label7);
      this.groupBox3.Location = new System.Drawing.Point(15, 433);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(500, 89);
      this.groupBox3.TabIndex = 3;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Scripts";
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(18, 29);
      this.label7.Margin = new System.Windows.Forms.Padding(15, 5, 15, 5);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(30, 13);
      this.label7.TabIndex = 0;
      this.label7.Text = "Lose";
      // 
      // tpScenEditor
      // 
      this.Controls.Add(this.groupBox3);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.groupBox1);
      this.Name = "tpScenEditor";
      this.Size = new System.Drawing.Size(684, 766);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.groupBox3.ResumeLayout(false);
      this.groupBox3.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.OpenFileDialog ofd;
    private System.Windows.Forms.SaveFileDialog sfd;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.TextBox tb_General_Desc;
    private ucMultipleItemBox mboxGeneral_Difficulty;
    private ucMultipleItemBox mboxGeneral_Wings;
    private System.Windows.Forms.TextBox tbGeneral_PlayerName;
    private System.Windows.Forms.TextBox tbGeneral_Name;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.TextBox tbAudio_Win;
    private System.Windows.Forms.TextBox tbAudio_Lose;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.Label label7;
  }
}
