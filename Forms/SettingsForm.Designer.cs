namespace Remote
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.SSHByXshellCheck = new System.Windows.Forms.RadioButton();
            this.SSHByPuttyCheck = new System.Windows.Forms.RadioButton();
            this.AutoLoginInfoButton = new System.Windows.Forms.Button();
            this.githubLink = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SSHLowPingCheck = new System.Windows.Forms.CheckBox();
            this.SSHLoginCheck = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.SSHByXshellCheck);
            this.groupBox1.Controls.Add(this.SSHByPuttyCheck);
            this.groupBox1.Location = new System.Drawing.Point(12, 100);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(275, 44);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SSH 连接方式";
            // 
            // SSHByXshellCheck
            // 
            this.SSHByXshellCheck.AutoSize = true;
            this.SSHByXshellCheck.Location = new System.Drawing.Point(118, 19);
            this.SSHByXshellCheck.Name = "SSHByXshellCheck";
            this.SSHByXshellCheck.Size = new System.Drawing.Size(119, 16);
            this.SSHByXshellCheck.TabIndex = 4;
            this.SSHByXshellCheck.TabStop = true;
            this.SSHByXshellCheck.Text = "Xshell(部分版本)";
            this.SSHByXshellCheck.UseVisualStyleBackColor = true;
            this.SSHByXshellCheck.Click += new System.EventHandler(this.SSHByXshellCheck_Click);
            // 
            // SSHByPuttyCheck
            // 
            this.SSHByPuttyCheck.AutoSize = true;
            this.SSHByPuttyCheck.Location = new System.Drawing.Point(16, 20);
            this.SSHByPuttyCheck.Name = "SSHByPuttyCheck";
            this.SSHByPuttyCheck.Size = new System.Drawing.Size(53, 16);
            this.SSHByPuttyCheck.TabIndex = 3;
            this.SSHByPuttyCheck.TabStop = true;
            this.SSHByPuttyCheck.Text = "Putty";
            this.SSHByPuttyCheck.UseVisualStyleBackColor = true;
            this.SSHByPuttyCheck.CheckedChanged += new System.EventHandler(this.SSHByPuttyCheck_CheckedChanged);
            this.SSHByPuttyCheck.Click += new System.EventHandler(this.SSHByPuttyCheck_Click);
            // 
            // AutoLoginInfoButton
            // 
            this.AutoLoginInfoButton.Location = new System.Drawing.Point(12, 150);
            this.AutoLoginInfoButton.Name = "AutoLoginInfoButton";
            this.AutoLoginInfoButton.Size = new System.Drawing.Size(140, 23);
            this.AutoLoginInfoButton.TabIndex = 3;
            this.AutoLoginInfoButton.Text = "打开默认密码配置";
            this.AutoLoginInfoButton.UseVisualStyleBackColor = true;
            this.AutoLoginInfoButton.Click += new System.EventHandler(this.AutoLoginInfoButton_Click);
            // 
            // githubLink
            // 
            this.githubLink.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.githubLink.Location = new System.Drawing.Point(76, 196);
            this.githubLink.Name = "githubLink";
            this.githubLink.ReadOnly = true;
            this.githubLink.Size = new System.Drawing.Size(173, 14);
            this.githubLink.TabIndex = 4;
            this.githubLink.Text = "github.com/hmomeng/remote";
            this.githubLink.Click += new System.EventHandler(this.githubLink_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.SSHLowPingCheck);
            this.groupBox2.Controls.Add(this.SSHLoginCheck);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(275, 82);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "SSH 连接(右侧俩窗口)";
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // label1
            // 
            this.label1.AutoEllipsis = true;
            this.label1.Location = new System.Drawing.Point(103, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(172, 40);
            this.label1.TabIndex = 10;
            this.label1.Text = "Ping通后停止，长PingSSH端口会记录登陆日志，修改文本后刷新";
            // 
            // SSHLowPingCheck
            // 
            this.SSHLowPingCheck.AutoSize = true;
            this.SSHLowPingCheck.Checked = true;
            this.SSHLowPingCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SSHLowPingCheck.Location = new System.Drawing.Point(103, 20);
            this.SSHLowPingCheck.Name = "SSHLowPingCheck";
            this.SSHLowPingCheck.Size = new System.Drawing.Size(78, 16);
            this.SSHLowPingCheck.TabIndex = 9;
            this.SSHLowPingCheck.Text = "SSH低Ping";
            this.SSHLowPingCheck.UseVisualStyleBackColor = true;
            // 
            // SSHLoginCheck
            // 
            this.SSHLoginCheck.AutoSize = true;
            this.SSHLoginCheck.Checked = true;
            this.SSHLoginCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SSHLoginCheck.Location = new System.Drawing.Point(16, 20);
            this.SSHLoginCheck.Name = "SSHLoginCheck";
            this.SSHLoginCheck.Size = new System.Drawing.Size(66, 16);
            this.SSHLoginCheck.TabIndex = 8;
            this.SSHLoginCheck.Text = "SSH登录";
            this.SSHLoginCheck.UseVisualStyleBackColor = true;
            this.SSHLoginCheck.CheckedChanged += new System.EventHandler(this.SSHLoginCheck_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(53, 196);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "By";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(299, 222);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.githubLink);
            this.Controls.Add(this.AutoLoginInfoButton);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "设置";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsForm_FormClosing);
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton SSHByXshellCheck;
        private System.Windows.Forms.RadioButton SSHByPuttyCheck;
        private System.Windows.Forms.Button AutoLoginInfoButton;
        private System.Windows.Forms.TextBox githubLink;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox SSHLowPingCheck;
        private System.Windows.Forms.CheckBox SSHLoginCheck;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}