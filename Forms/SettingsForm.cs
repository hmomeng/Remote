using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Remote
{
    public partial class SettingsForm : Form
    {   
        // 主窗体订阅 SettingForm 的窗体关闭事件
        public event EventHandler FormClosedEvent;
        private void OnFormClosedEvent()
        {
            FormClosedEvent?.Invoke(this, EventArgs.Empty);
        }

        // 读取系统属性
        public static bool deleteFlag = Properties.Settings.Default.deleteFlag;             // 关闭删除RDP文件
        public static bool tcpPingFlag = Properties.Settings.Default.tcpPingFlag;           // 端口Ping
        public static bool fullScreenFlag = Properties.Settings.Default.fullScreenFlag;     // 全屏，配合screenMode
        public static bool topFlag = Properties.Settings.Default.topFlag;                   // 置顶
        public static bool SSHFlag = Properties.Settings.Default.SSHFlag;                   // 右侧两窗口是否支持SSH
        public static string SSHMethod = Properties.Settings.Default.SSHMethodFlag;         // SSH客户端
        public static bool SSHLowPingFlag = Properties.Settings.Default.SSHLowPingFlag;     // SSH端口低Ping
        public static int TimeInterval = Properties.Settings.Default.TimeInterval;          // 端口Ping间隔、超时间隔
        public static int width = Properties.Settings.Default.width;                        // 分辨率宽度
        public static int height = Properties.Settings.Default.height;                      // 分辨率高度
        public static bool windowsposition = Properties.Settings.Default.WindowsPosition;   // RDP窗口打开位置
        private void syncSystemVariables()
        {
            Properties.Settings.Default.deleteFlag = deleteFlag;
            Properties.Settings.Default.tcpPingFlag = tcpPingFlag;
            Properties.Settings.Default.fullScreenFlag = fullScreenFlag;
            Properties.Settings.Default.topFlag = topFlag;
            Properties.Settings.Default.SSHFlag = SSHFlag;
            Properties.Settings.Default.SSHMethodFlag = SSHMethod;
            Properties.Settings.Default.SSHLowPingFlag = SSHLowPingFlag;
            Properties.Settings.Default.TimeInterval = TimeInterval;
            Properties.Settings.Default.width = width;
            Properties.Settings.Default.height = height;
            Properties.Settings.Default.WindowsPosition = windowsposition;
            Properties.Settings.Default.Save();
        }
        private void syncVariables()
        {
            // 读取系统属性
            deleteFlag = Properties.Settings.Default.deleteFlag;
            tcpPingFlag = Properties.Settings.Default.tcpPingFlag;
            fullScreenFlag = Properties.Settings.Default.fullScreenFlag;
            topFlag = Properties.Settings.Default.topFlag;
            SSHFlag = Properties.Settings.Default.SSHFlag;
            SSHMethod = Properties.Settings.Default.SSHMethodFlag;
            SSHLowPingFlag = Properties.Settings.Default.SSHLowPingFlag;
            TimeInterval = Properties.Settings.Default.TimeInterval;
            width = Properties.Settings.Default.width;
            height = Properties.Settings.Default.height;
            windowsposition = Properties.Settings.Default.WindowsPosition;
        }
        private void syncWindowsOption()
        {
            // 同步Checkbox选择
            SSHLoginCheck.Checked = SSHFlag;
            SSHLowPingCheck.Checked = SSHLowPingFlag;
            SSHByPuttyCheck.Checked = SSHMethod == "Putty";
            SSHByXshellCheck.Checked = SSHMethod == "Xshell";
            WindowsPositionComboBox.Text = windowsposition ? WindowsPositionComboBox.Items[0].ToString() : WindowsPositionComboBox.Items[1].ToString();
            TimeIntervalTextBox.Text = TimeInterval.ToString();
            DeleteRDPCheck.Checked = deleteFlag;

        }

        private void UpdateVariables()
        {
            SSHFlag = SSHLoginCheck.Checked;
            SSHLowPingFlag = SSHLowPingCheck.Checked;
            SSHMethod = SSHByPuttyCheck.Checked ? "Putty" : "Xshell";
            windowsposition = WindowsPositionComboBox.Text == WindowsPositionComboBox.Items[0].ToString();
            UpdateTimeInterval();
            deleteFlag = DeleteRDPCheck.Checked;
        }

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            syncVariables();
            syncWindowsOption();
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            UpdateVariables();
            syncSystemVariables();
            // 在窗体关闭前触发事件
            OnFormClosedEvent();
        }


        private void UpdateTimeInterval()
        {
            try
            {
                int TimeIntervalByInt = int.Parse(TimeIntervalTextBox.Text);
                if (TimeIntervalByInt >= 1000)
                {
                    TimeInterval = TimeIntervalByInt;
                }
                else
                {
                    MessageBox.Show($"最少1000毫秒", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000);
                }
            }
            catch (Exception)
            {
                MessageBox.Show($"输入错误", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000);
            }
        }


        private void ConfigFileButton_Click(object sender, EventArgs e)
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
