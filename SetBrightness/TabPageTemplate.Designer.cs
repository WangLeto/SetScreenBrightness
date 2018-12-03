namespace SetBrightness
{
    partial class TabPageTemplate
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.brightTrackbar = new System.Windows.Forms.TrackBar();
            this.contrastTrackbar = new System.Windows.Forms.TrackBar();
            this.contrastLabel = new System.Windows.Forms.Label();
            this.brightLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.brightTrackbar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.contrastTrackbar)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "亮度";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "对比度";
            // 
            // brightTrackbar
            // 
            this.brightTrackbar.AutoSize = false;
            this.brightTrackbar.Location = new System.Drawing.Point(0, 24);
            this.brightTrackbar.Maximum = 100;
            this.brightTrackbar.Name = "brightTrackbar";
            this.brightTrackbar.Size = new System.Drawing.Size(370, 30);
            this.brightTrackbar.TabIndex = 1;
            this.brightTrackbar.TabStop = false;
            this.brightTrackbar.TickFrequency = 5;
            // 
            // contrastTrackbar
            // 
            this.contrastTrackbar.AutoSize = false;
            this.contrastTrackbar.Location = new System.Drawing.Point(0, 83);
            this.contrastTrackbar.Maximum = 100;
            this.contrastTrackbar.Name = "contrastTrackbar";
            this.contrastTrackbar.Size = new System.Drawing.Size(370, 30);
            this.contrastTrackbar.TabIndex = 1;
            this.contrastTrackbar.TabStop = false;
            this.contrastTrackbar.TickFrequency = 5;
            // 
            // contrastLabel
            // 
            this.contrastLabel.AutoSize = true;
            this.contrastLabel.Location = new System.Drawing.Point(317, 63);
            this.contrastLabel.Name = "contrastLabel";
            this.contrastLabel.Size = new System.Drawing.Size(43, 17);
            this.contrastLabel.TabIndex = 2;
            this.contrastLabel.Text = "label3";
            // 
            // brightLabel
            // 
            this.brightLabel.AutoSize = true;
            this.brightLabel.Location = new System.Drawing.Point(317, 4);
            this.brightLabel.Name = "brightLabel";
            this.brightLabel.Size = new System.Drawing.Size(43, 17);
            this.brightLabel.TabIndex = 2;
            this.brightLabel.Text = "label3";
            // 
            // TabPageTemplate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.brightLabel);
            this.Controls.Add(this.contrastLabel);
            this.Controls.Add(this.contrastTrackbar);
            this.Controls.Add(this.brightTrackbar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "TabPageTemplate";
            this.Size = new System.Drawing.Size(380, 120);
            ((System.ComponentModel.ISupportInitialize)(this.brightTrackbar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.contrastTrackbar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar brightTrackbar;
        private System.Windows.Forms.TrackBar contrastTrackbar;
        private System.Windows.Forms.Label contrastLabel;
        private System.Windows.Forms.Label brightLabel;
    }
}
