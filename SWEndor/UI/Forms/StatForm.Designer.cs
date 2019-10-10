namespace SWEndor.UI.Forms
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
      this.tpSound = new System.Windows.Forms.TabPage();
      this.tpPool = new System.Windows.Forms.TabPage();
      this.ucStatObjects1 = new SWEndor.UI.Forms.UIControls.ucStatObjects();
      this.ucSound1 = new SWEndor.UI.Forms.UIControls.ucSound();
      this.ucPoolCache1 = new SWEndor.UI.Forms.UIControls.ucPoolCache();
      this.tabControl1.SuspendLayout();
      this.tpObjects.SuspendLayout();
      this.tpSound.SuspendLayout();
      this.tpPool.SuspendLayout();
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
      this.lblFPS.Location = new System.Drawing.Point(232, 27);
      this.lblFPS.Name = "lblFPS";
      this.lblFPS.Size = new System.Drawing.Size(50, 15);
      this.lblFPS.TabIndex = 11;
      this.lblFPS.Text = "0";
      // 
      // label7
      // 
      this.label7.Location = new System.Drawing.Point(46, 27);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(180, 15);
      this.label7.TabIndex = 10;
      this.label7.Text = "FPS";
      // 
      // lblGameTime
      // 
      this.lblGameTime.Location = new System.Drawing.Point(232, 42);
      this.lblGameTime.Name = "lblGameTime";
      this.lblGameTime.Size = new System.Drawing.Size(50, 15);
      this.lblGameTime.TabIndex = 13;
      this.lblGameTime.Text = "0";
      // 
      // label8
      // 
      this.label8.Location = new System.Drawing.Point(46, 42);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(180, 15);
      this.label8.TabIndex = 12;
      this.label8.Text = "Game Time";
      // 
      // lblRenderTime
      // 
      this.lblRenderTime.Location = new System.Drawing.Point(232, 57);
      this.lblRenderTime.Name = "lblRenderTime";
      this.lblRenderTime.Size = new System.Drawing.Size(50, 15);
      this.lblRenderTime.TabIndex = 15;
      this.lblRenderTime.Text = "0";
      // 
      // label10
      // 
      this.label10.Location = new System.Drawing.Point(46, 57);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(180, 15);
      this.label10.TabIndex = 14;
      this.label10.Text = "Time since Render";
      // 
      // label6
      // 
      this.label6.Location = new System.Drawing.Point(12, 9);
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
      this.tabControl1.Location = new System.Drawing.Point(12, 75);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(600, 400);
      this.tabControl1.TabIndex = 67;
      // 
      // tpObjects
      // 
      this.tpObjects.Controls.Add(this.ucStatObjects1);
      this.tpObjects.Location = new System.Drawing.Point(4, 22);
      this.tpObjects.Name = "tpObjects";
      this.tpObjects.Padding = new System.Windows.Forms.Padding(3);
      this.tpObjects.Size = new System.Drawing.Size(592, 374);
      this.tpObjects.TabIndex = 0;
      this.tpObjects.Text = "Objects";
      this.tpObjects.UseVisualStyleBackColor = true;
      // 
      // tpSound
      // 
      this.tpSound.Controls.Add(this.ucSound1);
      this.tpSound.Location = new System.Drawing.Point(4, 22);
      this.tpSound.Name = "tpSound";
      this.tpSound.Padding = new System.Windows.Forms.Padding(3);
      this.tpSound.Size = new System.Drawing.Size(592, 374);
      this.tpSound.TabIndex = 1;
      this.tpSound.Text = "Sounds";
      this.tpSound.UseVisualStyleBackColor = true;
      // 
      // tpPool
      // 
      this.tpPool.Controls.Add(this.ucPoolCache1);
      this.tpPool.Location = new System.Drawing.Point(4, 22);
      this.tpPool.Name = "tpPool";
      this.tpPool.Padding = new System.Windows.Forms.Padding(3);
      this.tpPool.Size = new System.Drawing.Size(592, 374);
      this.tpPool.TabIndex = 2;
      this.tpPool.Text = "Pool / Cache";
      this.tpPool.UseVisualStyleBackColor = true;
      // 
      // ucStatObjects1
      // 
      this.ucStatObjects1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ucStatObjects1.Location = new System.Drawing.Point(3, 3);
      this.ucStatObjects1.Name = "ucStatObjects1";
      this.ucStatObjects1.Size = new System.Drawing.Size(586, 368);
      this.ucStatObjects1.TabIndex = 0;
      // 
      // ucSound1
      // 
      this.ucSound1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ucSound1.Location = new System.Drawing.Point(3, 3);
      this.ucSound1.Name = "ucSound1";
      this.ucSound1.Size = new System.Drawing.Size(586, 368);
      this.ucSound1.TabIndex = 0;
      // 
      // ucPoolCache1
      // 
      this.ucPoolCache1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ucPoolCache1.Location = new System.Drawing.Point(3, 3);
      this.ucPoolCache1.Name = "ucPoolCache1";
      this.ucPoolCache1.Size = new System.Drawing.Size(586, 368);
      this.ucPoolCache1.TabIndex = 0;
      // 
      // StatForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(628, 510);
      this.Controls.Add(this.tabControl1);
      this.Controls.Add(this.label6);
      this.Controls.Add(this.lblRenderTime);
      this.Controls.Add(this.label10);
      this.Controls.Add(this.lblGameTime);
      this.Controls.Add(this.label8);
      this.Controls.Add(this.lblFPS);
      this.Controls.Add(this.label7);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "StatForm";
      this.Text = "Statistics";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.StatForm_FormClosing);
      this.tabControl1.ResumeLayout(false);
      this.tpObjects.ResumeLayout(false);
      this.tpSound.ResumeLayout(false);
      this.tpPool.ResumeLayout(false);
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
  }
}

