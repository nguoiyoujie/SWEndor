namespace SWEndor.Game.UI.Forms
{
  partial class StatForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StatForm));
      this.tmTick = new System.Windows.Forms.Timer(this.components);
      this.lblFPS = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.lblGameTime = new System.Windows.Forms.Label();
      this.label8 = new System.Windows.Forms.Label();
      this.lblRenderTime = new System.Windows.Forms.Label();
      this.label10 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.tpObjects = new System.Windows.Forms.TabPage();
      this.ucStatObjects1 = new SWEndor.Game.UI.Forms.UIControls.ucStatObjects();
      this.tpSound = new System.Windows.Forms.TabPage();
      this.ucSound1 = new SWEndor.Game.UI.Forms.UIControls.ucSound();
      this.tpPool = new System.Windows.Forms.TabPage();
      this.ucPoolCache1 = new SWEndor.Game.UI.Forms.UIControls.ucPoolCache();
      this.tpInfo = new System.Windows.Forms.TabPage();
      this.lblAIFrame = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.lblCollisionFrame = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.lblGameFrame = new System.Windows.Forms.Label();
      this.label9 = new System.Windows.Forms.Label();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.ucInfo1 = new SWEndor.Game.UI.Forms.UIControls.ucInfo();
      this.tabControl1.SuspendLayout();
      this.tpObjects.SuspendLayout();
      this.tpSound.SuspendLayout();
      this.tpPool.SuspendLayout();
      this.tpInfo.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.SuspendLayout();
      // 
      // tmTick
      // 
      this.tmTick.Enabled = true;
      this.tmTick.Interval = 50;
      this.tmTick.Tick += new System.EventHandler(this.tmTick_Tick);
      // 
      // lblFPS
      // 
      this.lblFPS.Location = new System.Drawing.Point(226, 28);
      this.lblFPS.Name = "lblFPS";
      this.lblFPS.Size = new System.Drawing.Size(50, 15);
      this.lblFPS.TabIndex = 11;
      this.lblFPS.Text = "0";
      // 
      // label7
      // 
      this.label7.Location = new System.Drawing.Point(40, 28);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(180, 15);
      this.label7.TabIndex = 10;
      this.label7.Text = "FPS";
      // 
      // lblGameTime
      // 
      this.lblGameTime.Location = new System.Drawing.Point(226, 43);
      this.lblGameTime.Name = "lblGameTime";
      this.lblGameTime.Size = new System.Drawing.Size(50, 15);
      this.lblGameTime.TabIndex = 13;
      this.lblGameTime.Text = "0";
      // 
      // label8
      // 
      this.label8.Location = new System.Drawing.Point(40, 43);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(180, 15);
      this.label8.TabIndex = 12;
      this.label8.Text = "Game Time";
      // 
      // lblRenderTime
      // 
      this.lblRenderTime.Location = new System.Drawing.Point(226, 58);
      this.lblRenderTime.Name = "lblRenderTime";
      this.lblRenderTime.Size = new System.Drawing.Size(50, 15);
      this.lblRenderTime.TabIndex = 15;
      this.lblRenderTime.Text = "0";
      // 
      // label10
      // 
      this.label10.Location = new System.Drawing.Point(40, 58);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(180, 15);
      this.label10.TabIndex = 14;
      this.label10.Text = "Time since Render";
      // 
      // label6
      // 
      this.label6.Location = new System.Drawing.Point(6, 10);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(180, 15);
      this.label6.TabIndex = 18;
      this.label6.Text = "General";
      // 
      // tabControl1
      // 
      this.tabControl1.Controls.Add(this.tpObjects);
      this.tabControl1.Controls.Add(this.tpSound);
      this.tabControl1.Controls.Add(this.tpPool);
      this.tabControl1.Controls.Add(this.tpInfo);
      this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl1.Location = new System.Drawing.Point(0, 0);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(628, 426);
      this.tabControl1.TabIndex = 67;
      // 
      // tpObjects
      // 
      this.tpObjects.Controls.Add(this.ucStatObjects1);
      this.tpObjects.Location = new System.Drawing.Point(4, 22);
      this.tpObjects.Name = "tpObjects";
      this.tpObjects.Padding = new System.Windows.Forms.Padding(3);
      this.tpObjects.Size = new System.Drawing.Size(620, 400);
      this.tpObjects.TabIndex = 0;
      this.tpObjects.Text = "Objects";
      this.tpObjects.UseVisualStyleBackColor = true;
      // 
      // ucStatObjects1
      // 
      this.ucStatObjects1.AutoScroll = true;
      this.ucStatObjects1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ucStatObjects1.Location = new System.Drawing.Point(3, 3);
      this.ucStatObjects1.Name = "ucStatObjects1";
      this.ucStatObjects1.Size = new System.Drawing.Size(614, 394);
      this.ucStatObjects1.TabIndex = 0;
      // 
      // tpSound
      // 
      this.tpSound.Controls.Add(this.ucSound1);
      this.tpSound.Location = new System.Drawing.Point(4, 22);
      this.tpSound.Name = "tpSound";
      this.tpSound.Padding = new System.Windows.Forms.Padding(3);
      this.tpSound.Size = new System.Drawing.Size(620, 400);
      this.tpSound.TabIndex = 1;
      this.tpSound.Text = "Sounds";
      this.tpSound.UseVisualStyleBackColor = true;
      // 
      // ucSound1
      // 
      this.ucSound1.AutoScroll = true;
      this.ucSound1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ucSound1.Location = new System.Drawing.Point(3, 3);
      this.ucSound1.Name = "ucSound1";
      this.ucSound1.Size = new System.Drawing.Size(614, 394);
      this.ucSound1.TabIndex = 0;
      // 
      // tpPool
      // 
      this.tpPool.Controls.Add(this.ucPoolCache1);
      this.tpPool.Location = new System.Drawing.Point(4, 22);
      this.tpPool.Name = "tpPool";
      this.tpPool.Padding = new System.Windows.Forms.Padding(3);
      this.tpPool.Size = new System.Drawing.Size(620, 400);
      this.tpPool.TabIndex = 2;
      this.tpPool.Text = "Pool / Cache";
      this.tpPool.UseVisualStyleBackColor = true;
      // 
      // ucPoolCache1
      // 
      this.ucPoolCache1.AutoScroll = true;
      this.ucPoolCache1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ucPoolCache1.Location = new System.Drawing.Point(3, 3);
      this.ucPoolCache1.Name = "ucPoolCache1";
      this.ucPoolCache1.Size = new System.Drawing.Size(614, 394);
      this.ucPoolCache1.TabIndex = 0;
      // 
      // tpInfo
      // 
      this.tpInfo.Controls.Add(this.ucInfo1);
      this.tpInfo.Location = new System.Drawing.Point(4, 22);
      this.tpInfo.Name = "tpInfo";
      this.tpInfo.Padding = new System.Windows.Forms.Padding(3);
      this.tpInfo.Size = new System.Drawing.Size(620, 400);
      this.tpInfo.TabIndex = 3;
      this.tpInfo.Text = "Version Info";
      this.tpInfo.UseVisualStyleBackColor = true;
      // 
      // lblAIFrame
      // 
      this.lblAIFrame.Location = new System.Drawing.Point(468, 58);
      this.lblAIFrame.Name = "lblAIFrame";
      this.lblAIFrame.Size = new System.Drawing.Size(50, 15);
      this.lblAIFrame.TabIndex = 73;
      this.lblAIFrame.Text = "0";
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(282, 58);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(180, 15);
      this.label2.TabIndex = 72;
      this.label2.Text = "AI Frame";
      // 
      // lblCollisionFrame
      // 
      this.lblCollisionFrame.Location = new System.Drawing.Point(468, 43);
      this.lblCollisionFrame.Name = "lblCollisionFrame";
      this.lblCollisionFrame.Size = new System.Drawing.Size(50, 15);
      this.lblCollisionFrame.TabIndex = 71;
      this.lblCollisionFrame.Text = "0";
      // 
      // label4
      // 
      this.label4.Location = new System.Drawing.Point(282, 43);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(180, 15);
      this.label4.TabIndex = 70;
      this.label4.Text = "Collision Frame";
      // 
      // lblGameFrame
      // 
      this.lblGameFrame.Location = new System.Drawing.Point(468, 28);
      this.lblGameFrame.Name = "lblGameFrame";
      this.lblGameFrame.Size = new System.Drawing.Size(50, 15);
      this.lblGameFrame.TabIndex = 69;
      this.lblGameFrame.Text = "0";
      // 
      // label9
      // 
      this.label9.Location = new System.Drawing.Point(282, 28);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(180, 15);
      this.label9.TabIndex = 68;
      this.label9.Text = "Game Frame";
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
      this.splitContainer1.Panel1.Controls.Add(this.label6);
      this.splitContainer1.Panel1.Controls.Add(this.lblAIFrame);
      this.splitContainer1.Panel1.Controls.Add(this.label7);
      this.splitContainer1.Panel1.Controls.Add(this.label2);
      this.splitContainer1.Panel1.Controls.Add(this.lblFPS);
      this.splitContainer1.Panel1.Controls.Add(this.lblCollisionFrame);
      this.splitContainer1.Panel1.Controls.Add(this.label8);
      this.splitContainer1.Panel1.Controls.Add(this.label4);
      this.splitContainer1.Panel1.Controls.Add(this.lblGameTime);
      this.splitContainer1.Panel1.Controls.Add(this.lblGameFrame);
      this.splitContainer1.Panel1.Controls.Add(this.label10);
      this.splitContainer1.Panel1.Controls.Add(this.label9);
      this.splitContainer1.Panel1.Controls.Add(this.lblRenderTime);
      this.splitContainer1.Panel1MinSize = 80;
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
      this.splitContainer1.Size = new System.Drawing.Size(628, 510);
      this.splitContainer1.SplitterDistance = 80;
      this.splitContainer1.TabIndex = 4;
      // 
      // ucInfo1
      // 
      this.ucInfo1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ucInfo1.Location = new System.Drawing.Point(3, 3);
      this.ucInfo1.Name = "ucInfo1";
      this.ucInfo1.Size = new System.Drawing.Size(614, 394);
      this.ucInfo1.TabIndex = 0;
      // 
      // StatForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(628, 510);
      this.Controls.Add(this.splitContainer1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "StatForm";
      this.Text = "Statistics";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.StatForm_FormClosing);
      this.Load += new System.EventHandler(this.StatForm_Load);
      this.tabControl1.ResumeLayout(false);
      this.tpObjects.ResumeLayout(false);
      this.tpSound.ResumeLayout(false);
      this.tpPool.ResumeLayout(false);
      this.tpInfo.ResumeLayout(false);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.Timer tmTick;
    private System.Windows.Forms.Label lblFPS;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Label lblGameTime;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.Label lblRenderTime;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tpObjects;
    private System.Windows.Forms.TabPage tpSound;
    private System.Windows.Forms.TabPage tpPool;
    private UIControls.ucStatObjects ucStatObjects1;
    private UIControls.ucSound ucSound1;
    private UIControls.ucPoolCache ucPoolCache1;
    private System.Windows.Forms.Label lblAIFrame;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label lblCollisionFrame;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label lblGameFrame;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.TabPage tpInfo;
    private UIControls.ucInfo ucInfo1;
  }
}

