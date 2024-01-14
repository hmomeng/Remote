using Remote.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Remote
{
    public partial class MainForm : Form
    {
        // 定时器
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        const string IniFileName = "config.ini";    // 默认账密配置文件
        const string RDPFileName = ".RDPconfig";    // RDP临时文件夹
        public string ConfigFolderPath;             // RDP全路径

        // 读取系统属性
        public static bool deleteFlag = Properties.Settings.Default.deleteFlag;             // 关闭删除RDP文件
        public static bool tcpPingFlag = Properties.Settings.Default.tcpPingFlag;           // 端口Ping
        public static bool fullScreenFlag = Properties.Settings.Default.fullScreenFlag;     // 全屏，配合screenMode
        public static bool topFlag = Properties.Settings.Default.topFlag;                   // 置顶
        public static bool SSHFlag = Properties.Settings.Default.SSHFlag;                   // 右侧两窗口是否支持SSH
        public static string SSHMethod = Properties.Settings.Default.SSHMethodFlag;         // SSH客户端
        public static bool SSHLowPingFlag = Properties.Settings.Default.SSHLowPingFlag;     // SSH端口低Ping
        public static int TimeInterval = Properties.Settings.Default.TimeInterval;          // 端口Ping间隔、超时间隔
        public static int width  = Properties.Settings.Default.width;                       // 分辨率宽度
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
            TcpPingCheck.Checked = tcpPingFlag;
            FullScreenCheck.Checked = fullScreenFlag;
            TopMostCheck.Checked = topFlag;
            ResolutionComboBox.Text = width.ToString() + "x" + height.ToString();
            //DeleteRDPCheck.Checked = deleteFlag;
            //SSHLoginCheck.Checked =SSHFlag;
            //SSHMethodCheck.Checked = SSHMethod;
            //SSHLowPingCheck.Checked = SSHLowPingFlag;
        }


        // 通过[名称]读取ini默认账密
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int GetPrivateProfileString(string section, string key, string defaultValue, StringBuilder returnedString, int size, string filePath);





        public MainForm()
        {

            InitializeComponent();
            // 快捷键
            HotKey.RegisterHotKey(this.Handle, 100, HotKey.KeyModifiers.Ctrl | HotKey.KeyModifiers.Alt, Keys.F);

            // 初始化托盘图标
            InitializeNotifyIcon();

            // 获取执行文件所在目录下的 .config 文件夹路径
            ConfigFolderPath = Path.Combine(Application.StartupPath, RDPFileName);

            // 定时器间隔
            timer.Interval = TimeInterval; 
            timer.Tick += Timer_Tick;
        }
        

        // 窗口关闭事件
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (e.CloseReason == CloseReason.UserClosing)
            {
                syncSystemVariables();
                if(deleteFlag){deleteRDPList();}
                deleteRegList();
            }
        }

        // 程序初始化事件
        private void Form1_Load(object sender, EventArgs e)
        {
            // 同步单选框状态
            syncVariables();
            syncWindowsOption();
            flashHistoryList();
            // 检查默认密码文件是否存在
            PublicMethod.CreateINIfile();
            LoadDefaultPassNameToComboBox(DefaultPassConnComboBox, IniFileName);
            // 启动定时器
            if (tcpPingFlag){timer.Start();} 
        }

        // 添加一个变量来跟踪上一次 ping 是否成功
        private bool lastPingSuccess1 = false;
        private bool lastPingSuccess2 = false;
        //TCPPing
        private async Task<bool> HandleTextBoxAsync(TextBox textBox, Label label, bool PingFlag,string defaultText = null)
        {
            if (!string.IsNullOrEmpty(textBox.Text))
            {
                string addr, ip, port, username, password;
                defaultText = defaultText == "SSH" ? "22" : "3389";

                ParseTextBoxContent(textBox.Text, out ip, out port, out username, out password, defaultText);
                addr = ip + ":" + port;

                // 如果上次没有Ping通
                if (!PingFlag)
                {
                    int delay = await MeasureTcpPingExecutionTime(addr, timeoutMilliseconds: TimeInterval);
                    if (delay < TimeInterval && delay >= 0)
                    {
                        label.Text = $"{addr}   {delay}ms";
                        return PingFlag = true; // 如果 ping 成功，设置标志为 true
                    }
                    else
                    {
                        label.Text = $"{addr}  close";
                        return PingFlag = false; // 如果 ping 失败，设置标志为 false
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                label.Text = defaultText ?? textBox.Name;
            }
            return false;
        }

        // 计时器定时执行代码，持续TcpPing
        private async void Timer_Tick(object sender, EventArgs e)
        {
            _ = await HandleTextBoxAsync(RDPtextBox1, RDPLabel1, false, "1");
            _ = await HandleTextBoxAsync(RDPtextBox2, RDPLabel2, false, "2");
            _ = await HandleTextBoxAsync(RDPtextBox3, RDPLabel3, false, "3");
            _ = await HandleTextBoxAsync(RDPtextBox4, RDPLabel4, false, "4");
            lastPingSuccess1 = await HandleTextBoxAsync(SSHtextBox1, SSHLabel1, lastPingSuccess1, SSHFlag ? "SSH" : "RDP");
            lastPingSuccess2 = await HandleTextBoxAsync(SSHtextBox2, SSHLabel2, lastPingSuccess2, SSHFlag ? "SSH" : "RDP");
            if(!SSHLowPingFlag)
            {
                lastPingSuccess1 = false;
                lastPingSuccess2 = false;

            }
        }

        // 去除中文字符和空行
        private static string RemoveChineseAndTrim(string input)
        {
            // 保留英文字符和数字，去除中文字符和符号
            return Regex.Replace(input, @"[^\x00-\x7F]+", "").Trim();
        }

        // 判断是否是空行
        public static bool IsNullOrEmptyOrWhitespace(string value)
        {
            return string.IsNullOrEmpty(value) || value.Trim().Length == 0;
        }

        public static bool TryExtractInfoFromSingleLine(string line, out string ip, out string port, out string username, out string password, string defaultPort = "3389")
        {
            ip = "";
            port = "";
            string host = string.Empty;
            username = string.Empty;
            password = string.Empty;

            // 去除空行
            line = line.Replace("\r", "").Replace("\n", "");
            
            // 判断是否只有一行
            if (line.Contains("\n"))
            {
                return false;
            }
            // 删除固定格式的标识
            line = line.Replace("服务器IP：", "").Replace("登录账号：", "\n").Replace("登录密码：", "\n");

            // 根据换行分隔
            string[] parts = line.Split('\n');

            if (parts.Length >= 3)
            {
                host = parts[0].Trim();
                string[] addr = host.Split(':');
                if (addr.Length == 1)
                {
                    ip = addr[0].Trim();
                    port = defaultPort;
                }
                else
                {
                    ip = addr[0].Trim();
                    port = addr[1].Trim();
                }

                username = parts[1].Trim();
                password = parts[2].Trim();
                //Console.WriteLine($"Host: {host}, Username: {username}, Password: {password}");

                return true;
            }

            // 如果匹配不成功，返回标识
            return false;
        }
       
        public  void ParseResolution(string Resolution,out  int width,out  int height)
        {
            width = Properties.Settings.Default.width;
            height = Properties.Settings.Default.height;
            try
            {
                // 使用 Split 方法按 "x" 分割字符串
                string[] dimensions = Resolution.Split('x');
                width = int.Parse(dimensions[0]);
                height = int.Parse(dimensions[1]);
            }
            catch
            {
                MessageBox.Show($"分辨率参数有误", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000);
            }
        }
        // 解析字符串
        public static void ParseTextBoxContent(string textBoxContent, out string ip, out string port, out string username, out string password, string defaultPort = "3389")
        {
            username = "administrator";
            password = string.Empty;
            string oneLineText = textBoxContent;
            ip = "";
            port = "";

            // 判断并去除前后的双引号
            if (textBoxContent.StartsWith("\"") && textBoxContent.EndsWith("\""))
            {
                // 使用 TrimStart 和 TrimEnd 方法去除前后的双引号
                textBoxContent = textBoxContent.Substring(1,textBoxContent.Length-2);
                oneLineText = textBoxContent;
            }

            // 将字符串按照换行符切割分别传入lines数组中
            string[] lines = textBoxContent.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            //Console.WriteLine(lines.Length);
            if(lines.Length == 1)
            {
                
                //Console.WriteLine("单行模式");
                // 尝试从单行提取信息
                if (TryExtractInfoFromSingleLine(oneLineText, out ip,out port, out username, out password,  defaultPort))
                {
                    TryExtractInfoFromSingleLine(oneLineText, out ip, out port, out username, out password, defaultPort);
                    //匹配成功，可以使用 host、username 和 password
                    return;
                }
            }

            // 从第一行开始处理
            foreach (var line in lines)
            {
                //Console.WriteLine("多行模式");
                // 清除空行和中文字符和符号
                string cleanedLine = RemoveChineseAndTrim(line);

                // 如果该行包含IP信息
                Match ipPortMatch = Regex.Match(cleanedLine, @"(\d+\.\d+\.\d+\.\d+)(?::(\d+))?");
                if (ipPortMatch.Success)
                {
                   ip = ipPortMatch.Groups[1].Value;
                   port = ipPortMatch.Groups[2].Success ? ipPortMatch.Groups[2].Value : defaultPort;


                    // 寻找下一行，如果存在则为用户名行
                    int nextLineIndex = Array.IndexOf(lines, line) + 1;
                    if (nextLineIndex < lines.Length)
                    {
                        username = RemoveChineseAndTrim(lines[nextLineIndex]);

                        // 寻找下一行，如果存在则为密码行
                        int passwordLineIndex = nextLineIndex + 1;
                        if (passwordLineIndex < lines.Length)
                        {
                            password = RemoveChineseAndTrim(lines[passwordLineIndex]);
                        }

                        // 结束匹配
                        break;
                    }
                }

            }
            //Console.WriteLine($"Host: {ip}:{port}, Username: {username}, Password: {password}");
        }

        // 执行CMD命令
        public static string ExecuteCommand(string command)
        {
            string output = string.Empty;

            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/c " + command;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            // 读取进程的输出流
            using (System.IO.StreamReader reader = process.StandardOutput)
            {
                output = reader.ReadToEnd();
            }
            process.WaitForExit();
            return output;
        }

        // 加密密码
        public static string ConvertFromSecureString(string passwd)
        {
            string key = "passwd IsNullOrEmpty";
            string command = $"powershell.exe -command \"($securePwd = ConvertTo-SecureString -AsPlainText -Force '{passwd}') | ConvertFrom-SecureString\"";
            string encryptedString = ExecuteCommand(command).Trim();
            if (!string.IsNullOrEmpty(encryptedString))
            {
                key = "51:b:" + encryptedString.Trim();
            }
            return key;
        }

        // 生成RDP文件
        public static void GenerateRdpFile(string fullAddress, string username, string password)
        {
            // 创建 .config 文件夹（如果不存在）
            string configFolderPath = Path.Combine(Environment.CurrentDirectory, RDPFileName);
            try
            {
                // 创建文件夹
                Directory.CreateDirectory(configFolderPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误：{ex.Message}");
            }
            
            // 创建 RDP 文件路径
            string rdpFilePath = Path.Combine(configFolderPath, fullAddress.Split(':')[0] + ".rdp");
            string screenMode = fullScreenFlag ? "0" : "1";
            string windowspositionText = windowsposition ? "winposstr:s:0,0,0,0,3840,2160" : "winposstr:s:1920,0,1920,1080,3840,2160";
            // winposstr:s:1920,0,1920,1080,3840,2160
            // winposstr:s:0,0,0,0,3840,2160
            // 设置 RDP 文件的内容
            string rdpFileContent = $@"domain:s:
alternate shell:s: 
shell working directory:s:
compression:i:1
authentication level:i:0
desktopwidth:i:{width}
desktopheight:i:{height}
session bpp:i:32
{windowspositionText}
bitmapcachepersistenable:i:1
autoreconnection enabled:i:1
keyboardhook:i:2
displayconnectionbar:i:1
disable wallpaper:i:1
disable menu anims:i:1
disable themes:i:0
disable cursor setting:i:0
disable full window drag:i:1
audiomode:i:0
redirectdrives:i:0
redirectprinters:i:1
redirectcomports:i:0
redirectsmartcards:i:0
screen mode id:i:{screenMode}
full address:s:{fullAddress}
username:s:{username}
password {password}";

            /*
             * domain:s:                          指定登录远程计算机的用户帐户所在域的名称
             * alternate shell:s:                 不清楚用途，官网说明是指定要在远程会话中作为 shell（而不是资源管理器）自动启动的程序。
             * shell working directory:s:         指定备用 shell 时要使用的远程计算机上的工作目录。
             * compression:i:1                    通过 RDP 传输到本地计算机时是否启用批量压缩。 1 压缩  0 不压缩
             * authentication level:i:0           是否验证CA证书  0 不验证
             * 
             * desktopwidth:i:                    水平分辨率
             * desktopheight:i:                   垂直分辨率
             * session bpp:i:32                   颜色深度为32
             * winposstr:s:0,1,2000,0,3840,795    指定客户端计算机上会话窗口的位置和尺寸
             * bitmapcachepersistenable:i:1       确定位图是否缓存在本地计算机上（基于磁盘的缓存）。 位图缓存可以提高远程会话的性能。  1 缓存
             * displayconnectionbar:i:1           全屏模式下是否显示顶部连接栏  0 不显示 1显示
             * disable wallpaper:i:1              禁用壁纸
             * disable menu anims:i:1             禁用菜单动画       
             * disable themes:i:0                 禁用主题           
             * disable cursor setting             禁用光标 
             * disable full window drag:i:1       拖动时显示窗口的轮廓   0 显示窗口的内容
             * audiomode:i:0                      本地或远程计算机是否播放音频   0 本地机器播放声音 1 远程机器上播放声音 2 不播放声音  
             * 
             * redirectdrives:i:0                 是否将本地磁盘映射到远程机器
             * redirectprinters:i:1               打印机重定向
             * redirectcomports:i:0               串口重定向
             * redirectsmartcards:i:0             智能卡重定向
             * 
             * autoreconnection enabled:i:1       连接端口时是否尝试重新连接
             * keyboardhook:i:2                   是否支持Win、Alt、Tab 组合键   0 不支持 1 处于焦点时  2 仅全屏时 3 仅RemoteAPP支持
             * 
             * full address:s:                    是否全屏 
             * username:s:                        远程用户名
             * password                           远程用户密码
             */


            // 写入内容到 RDP 文件
            File.WriteAllText(rdpFilePath, rdpFileContent);
        }

        // 执行RDP文件
        public static void ExecuteRdpFile(string rdpFilePath)
        {
            // 执行 RDP 文件
            Process process = new Process();
            process.StartInfo.FileName = "mstsc.exe";
            process.StartInfo.Arguments = rdpFilePath;

            process.Start();

            // 等待远程桌面连接工具关闭
            //process.WaitForExit();
        }

        private void HandleSSHLogin(TextBox textBox)
        {
            if (textBox.TextLength > 0)
            {
                if (SSHFlag)
                {
                    string textBoxContent = textBox.Text;
                    ParseTextBoxContent(textBoxContent, out string ip, out string port, out string username, out string password,"22");

                    if (!string.IsNullOrEmpty(ip) && !string.IsNullOrEmpty(port))
                    {
                        // 创建一个线程，将 StartPutty 方法作为委托传递给它
                        Thread puttyThread = new Thread(() => StartSSH(ip, port, username, password));

                        // 启动线程
                        puttyThread.Start();
                    }
                    else
                    {
                        MessageBox.Show($"SSH信息错误", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000);
                        
                    }
                }
                else
                {
                    GenerateAndExecuteRdpFileAsync(textBox);
                }
            }
        }
       
        private void StartSSH(string ip, string port, string username, string password)
        {
            SSHMethod = Properties.Settings.Default.SSHMethodFlag;
            string SSHClientPath = string.Empty;
            string SSHClientArgs = string.Empty;
            if (SSHMethod == "Putty")
            {
               SSHClientPath = "putty";
               SSHClientArgs = $"-ssh -l {username} -pw \"{password}\" -P {port} {ip}";
            }
            else if (SSHMethod == "Xshell")
            {
               SSHClientPath = "xshell";
               SSHClientArgs = $"ssh {username}:{password}@{ip}:{port}";
            }
            ProcessStartInfo SSHStartInfo = new ProcessStartInfo
            {
                FileName = SSHClientPath,
                Arguments = SSHClientArgs,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = false
            };
            try
            {
                // 启动 PuTTY
                Process puttyProcess = new Process
                {
                    StartInfo = SSHStartInfo
                };
                puttyProcess.Start();

                puttyProcess.WaitForExit();
                puttyProcess.Close();
            }
            catch (Exception)
            {

                MessageBox.Show($"未安装 {SSHMethod}，请在设置中选择其他客户端", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000);
            }
        }

        private Thread rdpThread;
        public void GenerateAndExecuteRdpFileAsync(TextBox textBox)
        {
            if (rdpThread == null || !rdpThread.IsAlive)
            {
                rdpThread = new Thread(() => GenerateAndExecuteRdpFile(textBox.Text));
                rdpThread.Start();
                
            }
        }
        private void GenerateAndExecuteRdpFile(string textBoxContent)
        {
            string ip,port;
            string addr;
            string username;
            string password;
            // 解析文本返回远程地址、用户名、密码
            ParseTextBoxContent(textBoxContent, out ip,out port, out username, out password);
            addr = ip + ":" + port;
            //Console.WriteLine($"Host: {addr}, Username: {username}, Password: {password}");

            string encryptedPassword = ConvertFromSecureString(password);
            try
            {
                // 生成 RDP 文件
                GenerateRdpFile(addr, username, encryptedPassword);
                
                // 执行 RDP 文件
                string rdpFilePath = Path.Combine(Path.Combine(Environment.CurrentDirectory, RDPFileName), ip + ".rdp");
                ExecuteRdpFile(rdpFilePath);
            }
            catch (Exception)
            {
                
                MessageBox.Show($"运行RDP文件出错，请检查输入格式是否正确", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000);
            }
        }

        // 计算TCPPing延迟
        static async Task<int> MeasureTcpPingExecutionTime(string host, int timeoutMilliseconds)
        {
            Stopwatch stopwatch = new Stopwatch();

            // 启动计时器
            stopwatch.Start();

            // 执行TcpPing方法，并获取延迟时间
            bool delay = await TcpPing(host, timeoutMilliseconds);
            
            // 停止计时器
            stopwatch.Stop();
            if (delay) {
                // 输出TcpPing方法执行时间
                //Console.WriteLine($"TcpPing方法执行时间: {stopwatch.ElapsedMilliseconds} 毫秒");
                // 返回延迟时间
                return (int)stopwatch.ElapsedMilliseconds;
            }
            else
            {
                return timeoutMilliseconds + 100;
            }
            
        }
        static async Task<bool> TcpPing(string host, int timeoutMilliseconds)
        {
            string pattern = @"^\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}:\d{1,5}\b$";
            Regex regex = new Regex(pattern);
            if (!regex.IsMatch(host))
            {
                return false;
            }


            string[] parts = host.Split(':');
            string ipaddr = parts[0];
            int port = int.Parse(parts[1]);

            using (TcpClient tcpClient = new TcpClient())
            {
                var connectTask = tcpClient.ConnectAsync(ipaddr, port);

                var delayTask = Task.Delay(timeoutMilliseconds);

                // 使用 Task.WhenAny 来等待首先完成的任务（连接成功或超时）
                var completedTask = await Task.WhenAny(connectTask, delayTask);

                // 如果连接任务完成，则返回连接状态；否则，表示超时，返回连接失败
                if (completedTask == connectTask && tcpClient.Connected)
                {
                    return true; // 连接成功，端口开放
                }
                else
                {
                    return false; // 连接失败，端口关闭或主机不可达
                }
            }
        }

        private void RDPButton1_Click(object sender, EventArgs e)
        {
            if (RDPtextBox1.Text.Length > 0){GenerateAndExecuteRdpFileAsync(RDPtextBox1);}
            flashHistoryList();

        }
        private void RDPButton2_Click(object sender, EventArgs e)
        {
            if (RDPtextBox2.Text.Length > 0) { GenerateAndExecuteRdpFileAsync(RDPtextBox2); }
            flashHistoryList();
        }

        private void RDPButton3_Click(object sender, EventArgs e)
        {
            if (RDPtextBox3.Text.Length > 0) { GenerateAndExecuteRdpFileAsync(RDPtextBox3); }
            flashHistoryList();
        }

        private void RDPButton4_Click(object sender, EventArgs e)
        {
            if (RDPtextBox4.Text.Length > 0) { GenerateAndExecuteRdpFileAsync(RDPtextBox4); }
            flashHistoryList();
        }

        private void SSHButton1_Click(object sender, EventArgs e)
        {

            HandleSSHLogin(SSHtextBox1);
        }

        private void SSHButton2_Click(object sender, EventArgs e)
        {

            HandleSSHLogin(SSHtextBox2);
        }

        // 获取默认账密名
        private void LoadDefaultPassNameToComboBox(ComboBox comboBox, string filePath)
        {
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("[") && line.EndsWith("]"))
                        {
                            // 去除括号，将section添加到ComboBox的Items集合中
                            string sectionName = line.Trim('[', ']');
                            comboBox.Items.Add(sectionName);
                        }
                    }
                    if (DefaultPassConnComboBox.Items.Count > 0) { DefaultPassConnComboBox.Text = (string)DefaultPassConnComboBox.Items[0]; }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void HistoryConnButton_Click(object sender, EventArgs e)
        {
            if (HistoryComboBox.Text.Length > 0)
            {
                string trimmedString = HistoryComboBox.Text.Trim();
                string rdpFilePath = Path.Combine(Path.Combine(Environment.CurrentDirectory, RDPFileName), trimmedString + ".rdp");
                UpdateScreenModeId(rdpFilePath, FullScreenCheck.Checked);
                ExecuteRdpFile(rdpFilePath);
                // flashFileList();
            }
            // 执行 RDP 文件

        }
        // 获取历史列表
        private void flashHistoryList()
        {
            List<string> fileList = GetConfigFilesSortedByDate();
            // 将文件列表绑定到 ListBox
            HistoryComboBox.DataSource = fileList;
            if(fileList.Count > 0) {
                HistoryComboBox.Text= fileList[0];
            }
        }
        // 遍历目录下的RDP文件
        public List<string> GetConfigFilesSortedByDate()
        {
            try
            {
                // 检查 .config 文件夹是否存在，如果不存在则创建
                if (!Directory.Exists(ConfigFolderPath))
                {
                    Directory.CreateDirectory(ConfigFolderPath);
                }

                // 获取 .config 文件夹中的文件列表
                string[] fileList = Directory.GetFiles(ConfigFolderPath, "*.rdp");

                // 创建一个列表来存储文件名和对应的修改日期或创建日期
                List<KeyValuePair<string, DateTime>> fileDates = new List<KeyValuePair<string, DateTime>>();

                // 获取每个文件的修改日期或创建日期并存储在 fileDates 列表中
                foreach (string filePath in fileList)
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    DateTime date = fileInfo.LastWriteTime; // 获取修改日期
                                                            // 若要获取创建日期，可以使用 fileInfo.CreationTime 属性
                                                            // DateTime date = fileInfo.CreationTime;

                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                    if (!string.IsNullOrEmpty(fileNameWithoutExtension) && fileNameWithoutExtension != filePath)
                    {
                        // 使用不包含扩展名的文件名和对应的日期添加到列表中
                        fileDates.Add(new KeyValuePair<string, DateTime>(fileNameWithoutExtension, date));
                    }
                }

                // 根据日期排序文件名列表
                fileDates.Sort((x, y) => y.Value.CompareTo(x.Value));

                // 选择只包含文件名的部分
                List<string> sortedFileList = new List<string>();
                foreach (var kvp in fileDates)
                {
                    sortedFileList.Add(kvp.Key);
                }

                return sortedFileList;
            }
            catch (Exception)
            {
                // 处理异常情况
                MessageBox.Show($"获取文件列表时发生错误", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000);
                return new List<string>(); // 返回空数组
            }
        }

        // 修改历史连接中的全屏选项
        public void UpdateScreenModeId(string filePath, bool isCheckboxChecked)
        {
            try
            {
                // 读取RDP文件的所有文本内容
                string[] lines = File.ReadAllLines(filePath);

                // 遍历每一行文本，查找并替换screen mode id:i:的值
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith("screen mode id:i:"))
                    {
                        // 根据复选框状态进行替换
                        if (isCheckboxChecked)
                        {
                            // 如果复选框未选中，则将screen mode id:i:1改为screen mode id:i:0
                            lines[i] = "screen mode id:i:0";
                        }
                        else
                        {
                            
                            // 如果复选框选中，则将screen mode id:i:0改为screen mode id:i:1
                            lines[i] = "screen mode id:i:1";
                        }

                        // 停止循环，因为我们已经找到并替换了所需的值
                        break;
                    }
                }

                // 将修改后的文本内容写回到RDP文件中
                File.WriteAllLines(filePath, lines);

                Console.WriteLine("RDP文件已更新。");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误：{ex.Message}");
            }
        }

        // 默认账密连接
        private void DefaultPassConnButton_Click(object sender, EventArgs e)
        {
            DefaultPassConnIPTextBox.Text = RemoveChineseAndTrim(DefaultPassConnIPTextBox.Text);
            string addr = "";
            string username = "";
            string password = "";
            string iniFilePath = System.IO.Path.Combine(Application.StartupPath, IniFileName);

            StringBuilder result = new StringBuilder(255); // 分配缓冲区大小
            int size = GetPrivateProfileString(DefaultPassConnComboBox.Text, "端口", "", result, result.Capacity, iniFilePath);
            string port = result.ToString().Trim();
            addr = DefaultPassConnIPTextBox.Text + ":" + port;
            //Console.WriteLine(addr);

            result.Clear();
            size = GetPrivateProfileString(DefaultPassConnComboBox.Text, "账号", "", result, result.Capacity, iniFilePath);
            username = result.ToString().Trim();
            //Console.WriteLine(username);

            result.Clear();
            size = GetPrivateProfileString(DefaultPassConnComboBox.Text, "密码", "", result, result.Capacity, iniFilePath);
            password = result.ToString().Trim();
            //Console.WriteLine(password);

            string encryptedPassword = ConvertFromSecureString(password);
            // 生成 RDP 文件
            GenerateRdpFile(addr, username, encryptedPassword);

            // 执行 RDP 文件
            string rdpFilePath = Path.Combine(Path.Combine(Environment.CurrentDirectory, RDPFileName), addr.Split(':')[0] + ".rdp");
            ExecuteRdpFile(rdpFilePath);
        }

        private void OpenRDPListButton_Click(object sender, EventArgs e)
        {
            // 指定文件夹的路径
            string folderPath = RDPFileName;

            try
            {
                // 使用Process.Start方法打开文件夹
                Process.Start(folderPath);
            }
            catch (Exception ex)
            {
                // 处理异常
                Console.WriteLine("Error: " + ex.Message);
            }
        }


        private void SSHtextBox2_TextChanged(object sender, EventArgs e)
        {
            lastPingSuccess2 = false;
        }


        private void SSHtextBox1_TextChanged(object sender, EventArgs e)
        {
            lastPingSuccess1 = false;
        }

        private void ClearTextBoxtButton_Click(object sender, EventArgs e)
        {
            RDPtextBox1.Text= string.Empty;
            RDPtextBox2.Text= string.Empty;
            RDPtextBox3.Text= string.Empty;
            RDPtextBox4.Text= string.Empty;
            SSHtextBox1.Text= string.Empty;
            SSHtextBox2.Text= string.Empty;
        }

        private void SettingButton_Click(object sender, EventArgs e)
        {
            syncSystemVariables();
            OpenSettingsForm();
        }
        void deleteRDPList()
        {
            string folderPath = RDPFileName; // 指定文件夹路径

            // 检查文件夹是否存在
            if (Directory.Exists(folderPath))
            {
                // 获取文件夹中所有的RDP文件
                string[] rdpFiles = Directory.GetFiles(folderPath, "*.rdp");

                // 遍历并删除所有RDP文件
                foreach (string rdpFilePath in rdpFiles)
                {
                    try
                    {
                        // 删除文件
                        File.Delete(rdpFilePath);
                        Console.WriteLine($"Deleted: {rdpFilePath}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to delete {rdpFilePath}: {ex.Message}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Folder does not exist.");
            }

            Console.WriteLine("Operation completed.");
        }
        private void deleteRegList()
        {
            string folderPath = RDPFileName; // 指定文件夹路径

            // 检查文件夹是否存在
            if (Directory.Exists(folderPath))
            {
                // 获取文件夹中所有的RDP文件
                string[] rdpFiles = Directory.GetFiles(folderPath, "*.reg");

                // 遍历并删除所有RDP文件
                foreach (string rdpFilePath in rdpFiles)
                {
                    try
                    {
                        // 删除文件
                        File.Delete(rdpFilePath);
                        Console.WriteLine($"Deleted: {rdpFilePath}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to delete {rdpFilePath}: {ex.Message}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Folder does not exist.");
            }

            Console.WriteLine("Operation completed.");
        }
        private void TcpPingCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (TcpPingCheck.Checked)
            {
                timer.Enabled = true;
                lastPingSuccess1 = false;
                lastPingSuccess2 = false;

            }
            else
            {
                timer.Enabled = false;
                RDPLabel1.Text = "1";
                RDPLabel2.Text = "2";
                RDPLabel3.Text = "3";
                RDPLabel4.Text = "4";
                if (SSHFlag)
                {
                    SSHLabel1.Text = "SSH";
                    SSHLabel2.Text = "SSH";
                }
                else
                {
                    SSHLabel1.Text = "RDP";
                    SSHLabel2.Text = "RDP";
                }
            }
            tcpPingFlag = TcpPingCheck.Checked;
        }

        private void FullScreenCheck_CheckedChanged(object sender, EventArgs e)
        {
            fullScreenFlag = FullScreenCheck.Checked;
        }
        private void TopMostCheck_CheckedChanged(object sender, EventArgs e)
        {
            topFlag = TopMostCheck.Checked;
            this.TopMost = topFlag;
            
        }


        // 当SettingsForm关闭时MainForm触发该事件
        private void SettingForm_FormClosedEvent(object sender, EventArgs e)
        {
            syncVariables();
            syncWindowsOption();
        }

        // 打开SettingsForm
        private void OpenSettingsForm()
        {
            SettingsForm settingsForm = new SettingsForm();
            // 订阅 SettingForm 的窗体关闭事件
            settingsForm.FormClosedEvent += SettingForm_FormClosedEvent;
            settingsForm.TopMost = this.TopMost;
            settingsForm.ShowDialog();
            
        }

        private AutoLoginForm autoLogin;
        private void OpenAutoLogin()
        {

            // 如果窗体已经实例化且处于显示状态，则隐藏
            if (autoLogin != null && !autoLogin.IsDisposed && autoLogin.Visible)
            {
                autoLogin.Hide();
            }
            else
            {
                // 如果窗体未实例化或者已被销毁，则创建新的窗体实例
                if (autoLogin == null || autoLogin.IsDisposed)
                {
                    autoLogin = new AutoLoginForm();
                }
                // 显示窗体
                autoLogin.TopMost = this.TopMost;
                autoLogin.Show();
            }
        }


        private void ResolutionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ParseResolution(ResolutionComboBox.Text, out width, out height);
        }

        private void ResolutionComboBox_Leave(object sender, EventArgs e)
        {
            ParseResolution(ResolutionComboBox.Text, out width, out height);
        }
        private void AutoLoginbutton_Click(object sender, EventArgs e)
        {
            OpenAutoLogin();
        }


        // ----------------------------------------------------------------------------------------------------------
        // 快捷键
        private NotifyIcon notifyIcon;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem openMenuItem;
        private ToolStripMenuItem aboutMenuItem;
        class HotKey
        {
            // 如果函数执行成功，返回值不为0。
            // 如果函数执行失败，返回值为0。要得到扩展错误信息，调用GetLastError。
            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool RegisterHotKey(
                IntPtr hWnd,                    // 要定义热键的窗口的句柄
                int id,                          // 定义热键ID（不能与其它ID重复）
                KeyModifiers fsModifiers,       // 标识热键是否在按Alt、Ctrl、Shift、Windows等键时才会生效
                Keys vk                         // 定义热键的内容
            );

            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool UnregisterHotKey(
                IntPtr hWnd,                    // 要取消热键的窗口的句柄
                int id                          // 要取消热键的ID
            );

            // 定义了辅助键的名称（将数字转变为字符以便于记忆，也可去除此枚举而直接使用数值）
            [Flags()]
            public enum KeyModifiers
            {
                None = 0,
                Alt = 1,
                Ctrl = 2,
                Shift = 4,
                WindowsKey = 8
            }
        }
        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;

            switch (m.Msg)
            {
                case WM_HOTKEY:
                    switch (m.WParam.ToInt32())
                    {
                        case 100:
                            if (this.Visible)
                            {
                                this.Hide();
                                timer.Stop();
                                notifyIcon.Visible = true;
                            }
                            else
                            {
                                this.Show();
                                if (TcpPingCheck.Checked) { 
                                    timer.Start(); 
                                }
                                this.WindowState = FormWindowState.Normal;
                                notifyIcon.Visible = false;
                            }
                            break;
                    }
                    break;
            }
            base.WndProc(ref m);
        }
        private void InitializeNotifyIcon()
        {
            if (Properties.Resources.mini != null)
            {
                // 使用嵌入的图标
                Icon customIcon = Properties.Resources.mini;

                notifyIcon = new NotifyIcon();
                notifyIcon.Icon = customIcon;

                // 初始化ContextMenuStrip
                contextMenuStrip = new ContextMenuStrip();

                // 添加打开菜单项
                openMenuItem = new ToolStripMenuItem("打开");
                openMenuItem.Click += OpenMenuItem_Click;
                contextMenuStrip.Items.Add(openMenuItem);


                // 为NotifyIcon设置右键选择菜单
                notifyIcon.ContextMenuStrip = contextMenuStrip;

                notifyIcon.Visible = false;
                notifyIcon.Click += NotifyIcon_Click;
            }
            else
            {
                MessageBox.Show($"无法加载嵌入的图标资源", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000);
            }


        }
        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            // 处理打开菜单项的点击事件，可以在这里添加显示主窗口的逻辑
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

        private void aboutMenuItem_Click(object sender, EventArgs e)
        {
            // 处理退出菜单项的点击事件，可以在这里添加退出应用程序的逻辑
            MessageBox.Show($"Auther by hmomeng@gmail.com", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000);
        }
        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            // 在托盘图标上的点击事件，你可以添加相应的处理逻辑
            // 这里简单地显示窗体
            if (((MouseEventArgs)e).Button == MouseButtons.Left)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                notifyIcon.Visible = false;
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 在窗体关闭时释放托盘图标资源
            notifyIcon.Dispose();
        }
        // 快捷键
        // ----------------------------------------------------------------------------------------------------------



    }
}
