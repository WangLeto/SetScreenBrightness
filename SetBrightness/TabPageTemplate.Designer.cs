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
            if (disposing)
            {
                components?.Dispose();
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
            this.brightnessNameLabel = new System.Windows.Forms.Label();
            this.contrastNameLabel = new System.Windows.Forms.Label();
            this.brightTrackbar = new System.Windows.Forms.TrackBar();
            this.contrastTrackbar = new System.Windows.Forms.TrackBar();
            this.contrastLabel = new System.Windows.Forms.Label();
            this.brightLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.brightTrackbar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.contrastTrackbar)).BeginInit();
            this.SuspendLayout();
            // 
            // brightnessNameLabel
            // 
            this.brightnessNameLabel.AutoSize = true;
            this.brightnessNameLabel.Location = new System.Drawing.Point(332, 4);
            this.brightnessNameLabel.Name = "brightnessNameLabel";
            this.brightnessNameLabel.Size = new System.Drawing.Size(32, 17);
            this.brightnessNameLabel.TabIndex = 0;
            this.brightnessNameLabel.Text = "亮度";
            this.brightnessNameLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // contrastNameLabel
            // 
            this.contrastNameLabel.AutoSize = true;
            this.contrastNameLabel.Location = new System.Drawing.Point(320, 57);
            this.contrastNameLabel.Name = "contrastNameLabel";
            this.contrastNameLabel.Size = new System.Drawing.Size(44, 17);
            this.contrastNameLabel.TabIndex = 0;
            this.contrastNameLabel.Text = "对比度";
            this.contrastNameLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
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
            this.brightTrackbar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.brightTrackbar_MouseDown);
            // 
            // contrastTrackbar
            // 
            this.contrastTrackbar.AutoSize = false;
            this.contrastTrackbar.Location = new System.Drawing.Point(0, 77);
            this.contrastTrackbar.Maximum = 100;
            this.contrastTrackbar.Name = "contrastTrackbar";
            this.contrastTrackbar.Size = new System.Drawing.Size(370, 30);
            this.contrastTrackbar.TabIndex = 1;
            this.contrastTrackbar.TabStop = false;
            this.contrastTrackbar.TickFrequency = 5;
            this.contrastTrackbar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.contrastTrackbar_MouseDown);
            // 
            // contrastLabel
            // 
            this.contrastLabel.AutoSize = true;
            this.contrastLabel.Location = new System.Drawing.Point(10, 57);
            this.contrastLabel.Name = "contrastLabel";
            this.contrastLabel.Size = new System.Drawing.Size(43, 17);
            this.contrastLabel.TabIndex = 2;
            this.contrastLabel.Text = "label3";
            // 
            // brightLabel
            // 
            this.brightLabel.AutoSize = true;
            this.brightLabel.Location = new System.Drawing.Point(10, 4);
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
            this.Controls.Add(this.contrastNameLabel);
            this.Controls.Add(this.brightnessNameLabel);
            this.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "TabPageTemplate";
            this.Size = new System.Drawing.Size(380, 110);
            ((System.ComponentModel.ISupportInitialize)(this.brightTrackbar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.contrastTrackbar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label brightnessNameLabel;
        private System.Windows.Forms.Label contrastNameLabel;
        private System.Windows.Forms.TrackBar brightTrackbar;
        private System.Windows.Forms.TrackBar contrastTrackbar;
        private System.Windows.Forms.Label contrastLabel;
        private System.Windows.Forms.Label brightLabel;
    }
}
