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
            this.ConfigFileButton = new System.Windows.Forms.Button();
            this.githubLink = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SSHLowPingCheck = new System.Windows.Forms.CheckBox();
            this.SSHLoginCheck = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.TimeIntervalTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.WindowsPositionComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.DeleteRDPCheck = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
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
            // 
            // ConfigFileButton
            // 
            this.ConfigFileButton.Location = new System.Drawing.Point(75, 285);
            this.ConfigFileButton.Name = "ConfigFileButton";
            this.ConfigFileButton.Size = new System.Drawing.Size(140, 23);
            this.ConfigFileButton.TabIndex = 3;
            this.ConfigFileButton.Text = "打开默认密码配置";
            this.ConfigFileButton.UseVisualStyleBackColor = true;
            this.ConfigFileButton.Click += new System.EventHandler(this.ConfigFileButton_Click);
            // 
            // githubLink
            // 
            this.githubLink.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.githubLink.Location = new System.Drawing.Point(75, 314);
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
            this.SSHLoginCheck.Location = new System.Drawing.Point(16, 20);
            this.SSHLoginCheck.Name = "SSHLoginCheck";
            this.SSHLoginCheck.Size = new System.Drawing.Size(66, 16);
            this.SSHLoginCheck.TabIndex = 8;
            this.SSHLoginCheck.Text = "SSH登录";
            this.SSHLoginCheck.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(52, 314);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "By";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.DeleteRDPCheck);
            this.groupBox3.Controls.Add(this.TimeIntervalTextBox);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.WindowsPositionComboBox);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Location = new System.Drawing.Point(13, 150);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(275, 109);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "RDP 选项";
            // 
            // TimeIntervalTextBox
            // 
            this.TimeIntervalTextBox.Location = new System.Drawing.Point(104, 49);
            this.TimeIntervalTextBox.Name = "TimeIntervalTextBox";
            this.TimeIntervalTextBox.Size = new System.Drawing.Size(121, 21);
            this.TimeIntervalTextBox.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "端口Ping间隔：";
            // 
            // WindowsPositionComboBox
            // 
            this.WindowsPositionComboBox.FormattingEnabled = true;
            this.WindowsPositionComboBox.Items.AddRange(new object[] {
            "左显示器",
            "右显示器"});
            this.WindowsPositionComboBox.Location = new System.Drawing.Point(104, 18);
            this.WindowsPositionComboBox.Name = "WindowsPositionComboBox";
            this.WindowsPositionComboBox.Size = new System.Drawing.Size(121, 20);
            this.WindowsPositionComboBox.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "窗口打开位置：";
            // 
            // DeleteRDPCheck
            // 
            this.DeleteRDPCheck.AutoSize = true;
            this.DeleteRDPCheck.Location = new System.Drawing.Point(17, 79);
            this.DeleteRDPCheck.Name = "DeleteRDPCheck";
            this.DeleteRDPCheck.Size = new System.Drawing.Size(132, 16);
            this.DeleteRDPCheck.TabIndex = 5;
            this.DeleteRDPCheck.Text = "退出时清空历史列表";
            this.DeleteRDPCheck.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(42, 262);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(245, 12);
            this.label5.TabIndex = 7;
            this.label5.Text = "Ctrl + Alt + F  主窗体可快捷键缩小至托盘";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(192, 73);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 12);
            this.label6.TabIndex = 6;
            this.label6.Text = "最少1000毫秒";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(230, 55);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 12);
            this.label7.TabIndex = 7;
            this.label7.Text = "毫秒";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 334);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.githubLink);
            this.Controls.Add(this.ConfigFileButton);
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
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton SSHByXshellCheck;
        private System.Windows.Forms.RadioButton SSHByPuttyCheck;
        private System.Windows.Forms.Button ConfigFileButton;
        private System.Windows.Forms.TextBox githubLink;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox SSHLowPingCheck;
        private System.Windows.Forms.CheckBox SSHLoginCheck;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox WindowsPositionComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TimeIntervalTextBox;
        private System.Windows.Forms.CheckBox DeleteRDPCheck;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
    }
}