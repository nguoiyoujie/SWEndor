namespace SWEndor.Game.UI.Forms.UIControls
{
  partial class ucPoolCache
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
      this.label15 = new System.Windows.Forms.Label();
      this.label12 = new System.Windows.Forms.Label();
      this.lblActorDistanceCache = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.lblActorDistancePool = new System.Windows.Forms.Label();
      this.lblActorDistanceInUse = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // lblInUse
      // 
      this.lblInUse.Location = new System.Drawing.Point(296, 55);
      this.lblInUse.Name = "lblInUse";
      this.lblInUse.Size = new System.Drawing.Size(60, 15);
      this.lblInUse.TabIndex = 105;
      this.lblInUse.Text = "In Use";
      // 
      // lblPool
      // 
      this.lblPool.Location = new System.Drawing.Point(230, 55);
      this.lblPool.Name = "lblPool";
      this.lblPool.Size = new System.Drawing.Size(60, 15);
      this.lblPool.TabIndex = 97;
      this.lblPool.Text = "Pool";
      // 
      // label15
      // 
      this.label15.Location = new System.Drawing.Point(10, 55);
      this.label15.Name = "label15";
      this.label15.Size = new System.Drawing.Size(180, 15);
      this.label15.TabIndex = 72;
      this.label15.Text = "Pool Utilization";
      // 
      // label12
      // 
      this.label12.Location = new System.Drawing.Point(10, 10);
      this.label12.Name = "label12";
      this.label12.Size = new System.Drawing.Size(180, 15);
      this.label12.TabIndex = 69;
      this.label12.Text = "Cache Utilization";
      // 
      // lblActorDistanceCache
      // 
      this.lblActorDistanceCache.Location = new System.Drawing.Point(230, 25);
      this.lblActorDistanceCache.Name = "lblActorDistanceCache";
      this.lblActorDistanceCache.Size = new System.Drawing.Size(60, 15);
      this.lblActorDistanceCache.TabIndex = 68;
      this.lblActorDistanceCache.Text = "0";
      // 
      // label5
      // 
      this.label5.Location = new System.Drawing.Point(40, 25);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(180, 15);
      this.label5.TabIndex = 67;
      this.label5.Text = "ActorDistanceInfo.Cache";
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(362, 10);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(60, 15);
      this.label1.TabIndex = 107;
      this.label1.Text = "In Use";
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(296, 10);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(60, 15);
      this.label2.TabIndex = 106;
      this.label2.Text = "Pool";
      // 
      // label3
      // 
      this.label3.Location = new System.Drawing.Point(230, 10);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(60, 15);
      this.label3.TabIndex = 108;
      this.label3.Text = "Cache";
      // 
      // lblActorDistancePool
      // 
      this.lblActorDistancePool.Location = new System.Drawing.Point(296, 25);
      this.lblActorDistancePool.Name = "lblActorDistancePool";
      this.lblActorDistancePool.Size = new System.Drawing.Size(60, 15);
      this.lblActorDistancePool.TabIndex = 109;
      this.lblActorDistancePool.Text = "0";
      // 
      // lblActorDistanceInUse
      // 
      this.lblActorDistanceInUse.Location = new System.Drawing.Point(362, 25);
      this.lblActorDistanceInUse.Name = "lblActorDistanceInUse";
      this.lblActorDistanceInUse.Size = new System.Drawing.Size(60, 15);
      this.lblActorDistanceInUse.TabIndex = 110;
      this.lblActorDistanceInUse.Text = "0";
      // 
      // ucPoolCache
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.lblActorDistanceInUse);
      this.Controls.Add(this.lblActorDistancePool);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.lblInUse);
      this.Controls.Add(this.lblPool);
      this.Controls.Add(this.label15);
      this.Controls.Add(this.label12);
      this.Controls.Add(this.lblActorDistanceCache);
      this.Controls.Add(this.label5);
      this.Name = "ucPoolCache";
      this.Size = new System.Drawing.Size(569, 359);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label lblInUse;
    private System.Windows.Forms.Label lblPool;
    private System.Windows.Forms.Label label15;
    private System.Windows.Forms.Label label12;
    private System.Windows.Forms.Label lblActorDistanceCache;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label lblActorDistancePool;
    private System.Windows.Forms.Label lblActorDistanceInUse;
  }
}
