using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Remote
{
    public partial class SettingsForm : Form
    {

        // 读取系统属性
        public static bool deleteFlag = Properties.Settings.Default.deleteFlag;
        public static bool tcpPingFlag = Properties.Settings.Default.tcpPingFlag;
        public static bool fullScreenFlag = Properties.Settings.Default.fullScreenFlag;
        public static bool topFlag = Properties.Settings.Default.topFlag;
        public static bool SSHFlag = Properties.Settings.Default.SSHFlag;
        public static string SSHMethod = Properties.Settings.Default.SSHMethodFlag;
        public static string screenMode = "1";  // 全屏=0，窗口=1

        public SettingsForm()
        {
            InitializeComponent();
            
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            // 读取属性
            SSHLoginCheck.Checked = Properties.Settings.Default.SSHFlag;
            SSHLowPingCheck.Checked = Properties.Settings.Default.SSHLowPingFlag;
            SSHByPuttyCheck.Checked = (Properties.Settings.Default.SSHMethodFlag == "Putty");
            SSHByXshellCheck.Checked = (Properties.Settings.Default.SSHMethodFlag == "Xshell");



        }

        private void SSHByPuttyCheck_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void SSHByPuttyCheck_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.SSHMethodFlag = "Putty";
            Properties.Settings.Default.Save();
        }

        private void SSHByXshellCheck_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.SSHMethodFlag = "Xshell";
            Properties.Settings.Default.Save();
        }

        private void AutoLoginInfoButton_Click(object sender, EventArgs e)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string configFilePath = Path.Combine(currentDirectory, "config.ini");

            // 检查文件是否存在
            if (File.Exists(configFilePath))
            {
                // 使用默认的文本编辑器打开文件
                Process.Start(configFilePath);
            }
            else
            {
                Console.WriteLine("config.ini 文件不存在。");
            }
        }

        private void SSHLoginCheck_CheckedChanged(object sender, EventArgs e)
        {

        }


        // 主窗体订阅 SettingForm 的窗体关闭事件
        public event EventHandler FormClosedEvent;
        private void OnFormClosedEvent()
        {
            FormClosedEvent?.Invoke(this, EventArgs.Empty);
        }
        private void SettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Console.WriteLine("SettringsForm close");

            Properties.Settings.Default.SSHFlag = SSHLoginCheck.Checked;
            Properties.Settings.Default.SSHLowPingFlag = SSHLowPingCheck.Checked;
            if (SSHByPuttyCheck.Checked)
            {
                Properties.Settings.Default.SSHMethodFlag = "Putty";
            }else if (SSHByXshellCheck.Checked)
            {
                Properties.Settings.Default.SSHMethodFlag = "Xshell";
            }
            Properties.Settings.Default.Save();
            // 在窗体关闭前触发事件
            OnFormClosedEvent();
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }



        private void githubLink_Click(object sender, EventArgs e)
        {
            string url = $"https://" + githubLink.Text;
            // 创建一个新的进程启动默认的 Web 浏览器
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
    }
}
