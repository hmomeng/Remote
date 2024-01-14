namespace Remote.Forms
{
    partial class AutoLoginForm
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
            this.autoIP = new System.Windows.Forms.TextBox();
            this.LoginButton = new System.Windows.Forms.Button();
            this.DefaultPassConnComboBox = new System.Windows.Forms.ComboBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.ClearRegButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // autoIP
            // 
            this.autoIP.Location = new System.Drawing.Point(12, 12);
            this.autoIP.Multiline = true;
            this.autoIP.Name = "autoIP";
            this.autoIP.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.autoIP.Size = new System.Drawing.Size(223, 368);
            this.autoIP.TabIndex = 0;
            // 
            // LoginButton
            // 
            this.LoginButton.Location = new System.Drawing.Point(254, 337);
            this.LoginButton.Name = "LoginButton";
            this.LoginButton.Size = new System.Drawing.Size(112, 43);
            this.LoginButton.TabIndex = 1;
            this.LoginButton.Text = "批量登陆";
            this.LoginButton.UseVisualStyleBackColor = true;
            this.LoginButton.Click += new System.EventHandler(this.LoginButton_Click);
            // 
            // DefaultPassConnComboBox
            // 
            this.DefaultPassConnComboBox.FormattingEnabled = true;
            this.DefaultPassConnComboBox.Location = new System.Drawing.Point(254, 297);
            this.DefaultPassConnComboBox.Name = "DefaultPassConnComboBox";
            this.DefaultPassConnComboBox.Size = new System.Drawing.Size(98, 20);
            this.DefaultPassConnComboBox.TabIndex = 4;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(250, 12);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(193, 58);
            this.textBox1.TabIndex = 6;
            this.textBox1.Text = "计算机\\HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Terminal Server Client\\LocalDevices";
            // 
            // ClearRegButton
            // 
            this.ClearRegButton.Location = new System.Drawing.Point(250, 72);
            this.ClearRegButton.Name = "ClearRegButton";
            this.ClearRegButton.Size = new System.Drawing.Size(102, 35);
            this.ClearRegButton.TabIndex = 7;
            this.ClearRegButton.Text = "清除注册表";
            this.ClearRegButton.UseVisualStyleBackColor = true;
            this.ClearRegButton.Click += new System.EventHandler(this.ClearRegButton_Click);
            // 
            // AutoLoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(438, 392);
            this.Controls.Add(this.ClearRegButton);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.DefaultPassConnComboBox);
            this.Controls.Add(this.LoginButton);
            this.Controls.Add(this.autoIP);
            this.Name = "AutoLoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "批量登陆";
            this.Load += new System.EventHandler(this.AutoLoginForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox autoIP;
        private System.Windows.Forms.Button LoginButton;
        private System.Windows.Forms.ComboBox DefaultPassConnComboBox;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button ClearRegButton;
    }
}