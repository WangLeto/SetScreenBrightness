namespace SetBrightness
{
    partial class Form1
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.trackBar_brightness = new System.Windows.Forms.TrackBar();
            this.label_brightness = new System.Windows.Forms.Label();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Contast_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AutoStart_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.退出程序ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button1 = new System.Windows.Forms.Button();
            this.label_contrast = new System.Windows.Forms.Label();
            this.trackBar_contrast = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_brightness)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_contrast)).BeginInit();
            this.SuspendLayout();
            // 
            // trackBar_brightness
            // 
            this.trackBar_brightness.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.trackBar_brightness.Location = new System.Drawing.Point(33, 18);
            this.trackBar_brightness.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.trackBar_brightness.Maximum = 100;
            this.trackBar_brightness.Name = "trackBar_brightness";
            this.trackBar_brightness.Size = new System.Drawing.Size(303, 45);
            this.trackBar_brightness.TabIndex = 0;
            this.trackBar_brightness.TabStop = false;
            this.trackBar_brightness.TickFrequency = 5;
            this.trackBar_brightness.Value = 20;
            // 
            // label_brightness
            // 
            this.label_brightness.AutoSize = true;
            this.label_brightness.Location = new System.Drawing.Point(7, 22);
            this.label_brightness.Name = "label_brightness";
            this.label_brightness.Size = new System.Drawing.Size(15, 17);
            this.label_brightness.TabIndex = 1;
            this.label_brightness.Text = "0";
            this.label_brightness.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);
            this.notifyIcon.MouseMove += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseMove);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Contast_ToolStripMenuItem,
            this.AutoStart_ToolStripMenuItem,
            this.退出程序ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(137, 70);
            this.contextMenuStrip1.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this.contextMenuStrip1_Closing);
            // 
            // Contast_ToolStripMenuItem
            // 
            this.Contast_ToolStripMenuItem.CheckOnClick = true;
            this.Contast_ToolStripMenuItem.Name = "Contast_ToolStripMenuItem";
            this.Contast_ToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.Contast_ToolStripMenuItem.Text = "启用对比度";
            this.Contast_ToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.Contast_ToolStripMenuItem_CheckStateChanged);
            this.Contast_ToolStripMenuItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Contast_ToolStripMenuItem_MouseDown);
            // 
            // AutoStart_ToolStripMenuItem
            // 
            this.AutoStart_ToolStripMenuItem.CheckOnClick = true;
            this.AutoStart_ToolStripMenuItem.Name = "AutoStart_ToolStripMenuItem";
            this.AutoStart_ToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.AutoStart_ToolStripMenuItem.Text = "随开机启动";
            this.AutoStart_ToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.AutoStart_ToolStripMenuItem_CheckStateChanged);
            this.AutoStart_ToolStripMenuItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.AutoStart_ToolStripMenuItem_MouseDown);
            // 
            // 退出程序ToolStripMenuItem
            // 
            this.退出程序ToolStripMenuItem.Name = "退出程序ToolStripMenuItem";
            this.退出程序ToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.退出程序ToolStripMenuItem.Text = "退出";
            this.退出程序ToolStripMenuItem.Click += new System.EventHandler(this.Exit_ToolStripMenuItem_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.White;
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gainsboro;
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(337, 18);
            this.button1.Margin = new System.Windows.Forms.Padding(0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(25, 25);
            this.button1.TabIndex = 2;
            this.button1.TabStop = false;
            this.button1.Text = "×";
            this.button1.UseCompatibleTextRendering = true;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label_contrast
            // 
            this.label_contrast.AutoSize = true;
            this.label_contrast.Location = new System.Drawing.Point(48, 67);
            this.label_contrast.Name = "label_contrast";
            this.label_contrast.Size = new System.Drawing.Size(15, 17);
            this.label_contrast.TabIndex = 3;
            this.label_contrast.Text = "0";
            this.label_contrast.Visible = false;
            // 
            // trackBar_contrast
            // 
            this.trackBar_contrast.Location = new System.Drawing.Point(67, 65);
            this.trackBar_contrast.Maximum = 100;
            this.trackBar_contrast.Name = "trackBar_contrast";
            this.trackBar_contrast.Size = new System.Drawing.Size(293, 45);
            this.trackBar_contrast.TabIndex = 4;
            this.trackBar_contrast.TabStop = false;
            this.trackBar_contrast.TickFrequency = 5;
            this.trackBar_contrast.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "对比度";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(376, 66);
            this.Controls.Add(this.trackBar_contrast);
            this.Controls.Add(this.label_contrast);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label_brightness);
            this.Controls.Add(this.trackBar_brightness);
            this.Controls.Add(this.label2);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Deactivate += new System.EventHandler(this.Form1_Deactivate);
            this.VisibleChanged += new System.EventHandler(this.Form1_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_brightness)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_contrast)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar trackBar_brightness;
        private System.Windows.Forms.Label label_brightness;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 退出程序ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AutoStart_ToolStripMenuItem;
        private System.Windows.Forms.Button button1;
        public System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ToolStripMenuItem Contast_ToolStripMenuItem;
        private System.Windows.Forms.Label label_contrast;
        private System.Windows.Forms.TrackBar trackBar_contrast;
        private System.Windows.Forms.Label label2;
    }
}

