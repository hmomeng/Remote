using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
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

        const string IniFileName = "config.ini"; // 默认账密配置文件
        const string RDPFileName = ".RDPconfig";  // RDP临时文件夹

        // 读取系统属性
        public static bool deleteFlag = Properties.Settings.Default.deleteFlag;
        public static bool tcpPingFlag = Properties.Settings.Default.tcpPingFlag;
        public static bool fullScreenFlag = Properties.Settings.Default.fullScreenFlag;
        public static bool topFlag = Properties.Settings.Default.topFlag;
        public static bool SSHFlag = Properties.Settings.Default.SSHFlag;
        public static string SSHMethod = Properties.Settings.Default.SSHMethodFlag;
        public static bool SSHLowPingFlag = Properties.Settings.Default.SSHLowPingFlag;
        public static string screenMode = fullScreenFlag?"0":"1";  // 全屏=0，窗口=1


        // 读取RDP文件
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

            // 定时器间隔1000（毫秒）
            timer.Interval = 1000; // 
            timer.Tick += Timer_Tick;
        }


        // 覆盖OnFormClosing事件
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            // 检查窗体是否正在关闭
            if (e.CloseReason == CloseReason.UserClosing)
            {


                if (Properties.Settings.Default.deleteFlag == true)
                {
                    deleteRDPList();
                    
                }
                refreshCheckboxState();
                UpdateAttribute();
                
            }
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            // 同步单选框状态
            refreshCheckboxState();


            // 检查默认密码文件是否存在
            if (File.Exists(IniFileName))
            {
                Console.WriteLine("config.ini 文件已存在。");
            }
            else
            {
                // 文件不存在，创建文件
                try
                {
                    //using (StreamWriter sw = File.CreateText(IniFileName))
                    using (StreamWriter sw = new StreamWriter(IniFileName, false, System.Text.Encoding.GetEncoding("gb2312")))
                    {
                        sw.WriteLine($@"[Default]
端口=3389
账号=Administrator
密码=123qwe!@#
");
                    }
                    // 获取文件的文件属性
                    FileAttributes attributes = File.GetAttributes(IniFileName);
                    // 添加Hidden标志到文件属性中
                    attributes |= FileAttributes.Hidden;
                    // 设置文件的文件属性，将其隐藏
                    File.SetAttributes(IniFileName, attributes);
                    Console.WriteLine("config.ini 文件已创建。");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("创建文件时发生错误: " + ex.Message);
                }
            }


            flashFileList();
            LoadSectionsToComboBox(DefaultPassConnComboBox, IniFileName);
            if (DefaultPassConnComboBox.Items.Count > 0)
            {
                DefaultPassConnComboBox.Text = (string)DefaultPassConnComboBox.Items[0];
            }
            // textBox5.Text = GetFirstSshFileName();
            if (tcpPingFlag)
            {
                timer.Start();
            } // 启动定时器


        }

        private async Task HandleTextBoxAsync(TextBox textBox, Label label, string defaultText = null)
        {
            if (!string.IsNullOrEmpty(textBox.Text))
            {

                string addr, ip, port, username, password;
                defaultText = defaultText == "SSH" ? "22" : "3389";

                ParseTextBoxContent(textBox.Text, out ip, out port, out username, out password, defaultText);
                addr = ip + ":" + port;
                label.Text = addr;

                if (SSHFlag && defaultText == "SSH")
                {
                    int delay = await MeasureTcpPingExecutionTime(addr, timeoutMilliseconds: 1000);
                    if (delay < 500 && delay > 0)
                    {
                        label.Text = $"{addr}   {delay}ms";
                    }
                    else
                    {
                        label.Text = $"{addr}  close";
                    }
                    //Console.WriteLine("SSH Print");
                }
                else
                {
                    int delay = await MeasureTcpPingExecutionTime(addr, timeoutMilliseconds: 1000);
                    if (delay < 500 && delay > 0)
                    {
                        label.Text = $"{addr}   {delay}ms";
                    }
                    else
                    {
                        label.Text = $"{addr}  close";
                    }
                }


            }
            else
            {
                label.Text = defaultText ?? textBox.Name;
            }
            await Task.Delay(500);
        }

        // 添加一个变量来跟踪上一次 ping 是否成功
        private bool lastPingSuccess1 = false;
        private bool lastPingSuccess2 = false;

        private async Task<bool> HandleTextBoxAsyncBySSH(TextBox textBox, Label label, bool PingFlag,string defaultText = null)
        {
            if (!string.IsNullOrEmpty(textBox.Text))
            {
                string addr, ip, port, username, password;
                defaultText = defaultText == "SSH" ? "22" : "3389";

                ParseTextBoxContent(textBox.Text, out ip, out port, out username, out password, defaultText);
                addr = ip + ":" + port;
                label.Text = addr;

                if (SSHFlag &&  PingFlag)
                {
                    int delay = await MeasureTcpPingExecutionTime(addr, timeoutMilliseconds: 1000);
                    if (delay < 500 && delay > 0)
                    {
                        label.Text = $"{addr}   {delay}ms";
                        return PingFlag = true; // 如果 ping 成功，设置标志为 true
                    }
                    else
                    {
                        label.Text = $"{addr}  close";
                        return PingFlag = false; // 如果 ping 失败，设置标志为 false
                    }
                    //Console.WriteLine("SSH Print");
                }
                else if (!SSHFlag || defaultText != "SSH")
                {
                    int delay = await MeasureTcpPingExecutionTime(addr, timeoutMilliseconds: 1000);
                    if (delay < 500 && delay > 0)
                    {
                        label.Text = $"{addr}   {delay}ms";
                        return PingFlag = true; // 如果 ping 成功，设置标志为 true
                    }
                    else
                    {
                        label.Text = $"{addr}  close";
                         return  PingFlag = false; // 如果 ping 失败，设置标志为 false
                    }
                }
            }
            else
            {
                label.Text = defaultText ?? textBox.Name;
            }
            await Task.Delay(1000);

            return false;
        }

        // 计时器定时执行代码，持续TcpPing
        private async void Timer_Tick(object sender, EventArgs e)
        {
            await HandleTextBoxAsync(RDPtextBox1, RDPLabel1,"1");
            await HandleTextBoxAsync(RDPtextBox2, RDPLabel2,"2");
            await HandleTextBoxAsync(RDPtextBox3, RDPLabel3,"3");
            await HandleTextBoxAsync(RDPtextBox4, RDPLabel4,"4");
            if(SSHFlag && SSHLowPingFlag)
            {
                if (!lastPingSuccess1)
                {
                    lastPingSuccess1 = await HandleTextBoxAsyncBySSH(SSHtextBox2, SSHLabel2, lastPingSuccess1, SSHFlag ? "SSH" : "RDP");
                }
                if (!lastPingSuccess2) { 
                    lastPingSuccess2 = await HandleTextBoxAsyncBySSH(SSHtextBox1, SSHLabel1, lastPingSuccess2, SSHFlag ? "SSH" : "RDP"); 
                }

            }
            else
            {
                await HandleTextBoxAsync(SSHtextBox2, SSHLabel2, SSHFlag ? "SSH" : "RDP");
                await HandleTextBoxAsync(SSHtextBox1, SSHLabel1, SSHFlag ? "SSH" : "RDP");
            }
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
        // 解析字符串
        public static void ParseTextBoxContent(string textBoxContent, out string ip, out string port, out string username, out string password, string defaultPort = "3389")
        {
            username = "administrator";
            password = string.Empty;
            string oneLineText = textBoxContent;
            ip = "";
            port = "";
            

            // 将字符串按照换行符切割分别传入lines数组中
            string[] lines = textBoxContent.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            //Console.WriteLine(lines.Length);
            if(lines.Length == 1)
            {
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

            // 设置 RDP 文件的内容
            string rdpFileContent = $@"desktopwidth:i:1180
desktopheight:i:720
session bpp:i:32
winposstr:s:0,1,2000,0,3840,795
compression:i:1
keyboardhook:i:2
audiomode:i:0
authentication level:i:0
redirectdrives:i:0
redirectprinters:i:1
redirectcomports:i:0
redirectsmartcards:i:1
displayconnectionbar:i:1
autoreconnection enabled:i:1
domain:s:
alternate shell:s:
shell working directory:s:
disable wallpaper:i:1
disable full window drag:i:1
disable menu anims:i:1
disable themes:i:0
disable cursor setting:i:0
bitmapcachepersistenable:i:1
screen mode id:i:{screenMode}
full address:s:{fullAddress}
username:s:{username}
password {password}";

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

        private void deleteRDPList()
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
                        
                        MessageBox.Show("SSH信息错误", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    GenerateAndExecuteRdpFileAsync(textBox);
                }
            }
        }

        // 执行远程连接
        // private BackgroundWorker rdpWorker;
        // private Thread sshThread;
        private Thread rdpThread;

        public void GenerateAndExecuteRdpFileAsync(TextBox textBox)
        {
            if (rdpThread == null || !rdpThread.IsAlive)
            {
                rdpThread = new Thread(() => GenerateAndExecuteRdpFile(textBox.Text));
                rdpThread.Start();
            }
            flashFileList();
        }
        public static void ParseSSHText(string textBoxContent, out string ip, out string port, out string username, out string password)
        {
            ip = string.Empty;
            port = "22";
            string addr = string.Empty;
            username = "root";
            password = string.Empty;

            // 将字符串按照换行符切割分别传入lines数组中
            string[] lines = textBoxContent.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length == 1)
            {
                //string ipPortLine = RemoveChineseAndTrim(lines[0]);
                Match ipPortMatch = Regex.Match(lines[0], @"(\d+\.\d+\.\d+\.\d+):(\d+)");
                if (ipPortMatch.Success)
                {
                    ip = ipPortMatch.Groups[1].Value;
                    port = ipPortMatch.Groups[2].Value;
                    if (port == string.Empty)
                    {
                        port = "22";
                    }

                    addr = ip + ":" + port;


                }
                string[] passline = Regex.Split(lines[0], @"[：]");
                if (passline.Length > 1)
                {
                    password = passline[passline.Length - 1];
                }
                return;
            }
            // 如果小于3行，则退出
            if (lines.Length < 3 && addr == string.Empty)
            {
                return;
            }

            //清除中文和中文符号
            for (int i = 0; i < lines.Length; i++)
            {
                RemoveChineseAndTrim(lines[i]);
            }
            // 确认IP的行号
            int startIndex = 0;
            while (startIndex < lines.Length && IsNullOrEmptyOrWhitespace(lines[startIndex]))
            {
                startIndex++;
            }

            if (startIndex + 2 > lines.Length)
            {
                string ipPortLine = RemoveChineseAndTrim(lines[startIndex]);
                // addr = RemoveChineseAndTrim(lines[startIndex]);
                string usernameLine = "root";
                string passwordLine = "root";

                // 提取 IP 和端口

                Match ipPortMatch = Regex.Match(ipPortLine, @"(\d+\.\d+\.\d+\.\d+):(\d+)");
                if (ipPortMatch.Success)
                {

                    ip = ipPortMatch.Groups[1].Value;
                    port = ipPortMatch.Groups[2].Value;
                    if (port == string.Empty)
                    {
                        port = "22";
                    }

                    addr = ip + ":" + port;
                }
                else
                {
                    ipPortMatch = Regex.Match(ipPortLine, @"(\d+\.\d+\.\d+\.\d+)");
                    if (ipPortMatch.Success)
                    {

                        ip = ipPortMatch.Groups[1].Value;

                        addr = ip + ":" + port;
                    }

                }

                // 提取用户名和密码
                username = usernameLine;
                password = passwordLine;

            }
            else if (startIndex + 2 < lines.Length)
            {
                string ipPortLine = RemoveChineseAndTrim(lines[startIndex]);
                // addr = RemoveChineseAndTrim(lines[startIndex]);
                string usernameLine = RemoveChineseAndTrim(lines[startIndex + 1]);
                string passwordLine = RemoveChineseAndTrim(lines[startIndex + 2]);

                // 提取 IP 和端口

                Match ipPortMatch = Regex.Match(ipPortLine, @"(\d+\.\d+\.\d+\.\d+):(\d+)");
                if (ipPortMatch.Success)
                {

                    ip = ipPortMatch.Groups[1].Value;
                    port = ipPortMatch.Groups[2].Value;

                    addr = ip + ":" + port;
                }
                else
                {
                    ipPortMatch = Regex.Match(ipPortLine, @"(\d+\.\d+\.\d+\.\d+)");
                    if (ipPortMatch.Success)
                    {

                        ip = ipPortMatch.Groups[1].Value;

                        addr = ip + ":" + port;
                    }

                }

                // 提取用户名和密码
                username = usernameLine;
                password = passwordLine;
            }
        }
        private void StartSSH(string ip, string port, string username, string password)
        {
            SSHMethod = Properties.Settings.Default.SSHMethodFlag;
            if (SSHMethod == "Putty")
            {
                string puttyPath = "putty";
                string puttyArgs = $"-ssh -l {username} -pw \"{password}\" -P {port} {ip}";
                ProcessStartInfo puttyStartInfo = new ProcessStartInfo
                {
                    FileName = puttyPath,
                    Arguments = puttyArgs,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = false
                };
                try
                {
                    // 启动 PuTTY
                    Process puttyProcess = new Process
                    {
                        StartInfo = puttyStartInfo
                    };
                    puttyProcess.Start();

                    puttyProcess.WaitForExit();
                    puttyProcess.Close();
                }
                catch (Exception)
                {
                    
                    MessageBox.Show($"环境变量中未找到Putty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000);
                }
            }else if(SSHMethod == "Xshell")
            {
                string xshellPath = "xshell";
                string xshellArgs = $"ssh {username}:{password}@{ip}:{port}";
                ProcessStartInfo XshellStartInfo = new ProcessStartInfo
                {
                    FileName = xshellPath,
                    Arguments = xshellArgs,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = false
                };
                try
                {
                    // 启动 PuTTY
                    Process XshellProcess = new Process
                    {
                        StartInfo = XshellStartInfo
                    };
                    XshellProcess.Start();

                    XshellProcess.WaitForExit();
                    XshellProcess.Close();
                }
                catch (Exception)
                {
                    MessageBox.Show($"环境变量中未找到Xshell", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000);
                }
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
            // 生成 RDP 文件
            GenerateRdpFile(addr, username, encryptedPassword);

            // 执行 RDP 文件
            string rdpFilePath = Path.Combine(Path.Combine(Environment.CurrentDirectory, RDPFileName), ip + ".rdp");
            ExecuteRdpFile(rdpFilePath);
        }

        public string ConfigFolderPath { get; private set; }


        public List<string> GetConfigFiles()
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
                List<string> cleanedFileList = new List<string>();
                foreach (string filePath in fileList)
                {
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                    if (fileNameWithoutExtension != string.Empty && fileNameWithoutExtension != filePath)
                    {
                        // 使用不包含扩展名的文件名
                        cleanedFileList.Add(fileNameWithoutExtension);
                    }
                }
                //// 只保留文件名，去除路径和后缀名
                //for (int i = 0; i < fileList.Length; i++)
                //{
                //    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileList[i]);
                //    if (fileNameWithoutExtension == string.Empty || fileNameWithoutExtension == fileList[i])
                //    {
                //        // 文件名为空或者与原始文件路径相同，表示没有文件名，使用文件路径作为文件名
                //        fileList[i] = Path.GetFileName(fileList[i]);
                //    }
                //    else
                //    {
                //        // 使用不包含扩展名的文件名
                //        fileList[i] = fileNameWithoutExtension;
                //    }
                //}
                return cleanedFileList;
            }
            catch (Exception ex)
            {
                // 处理异常情况
                MessageBox.Show($"获取文件列表时发生错误：{ex.Message}");
                return new List<string>(); // 返回空数组
            }
        }
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
            catch (Exception ex)
            {
                // 处理异常情况
                MessageBox.Show($"获取文件列表时发生错误：{ex.Message}");
                return new List<string>(); // 返回空数组
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

        static async Task<int> MeasureTcpPingExecutionTime(string host, int timeoutMilliseconds)
        {
            Stopwatch stopwatch = new Stopwatch();

            // 启动计时器
            stopwatch.Start();

            // 执行TcpPing方法，并获取延迟时间
            bool delay = await TcpPing(host, timeoutMilliseconds);

            // 停止计时器
            stopwatch.Stop();

            // 输出TcpPing方法执行时间
            //Console.WriteLine($"TcpPing方法执行时间: {stopwatch.ElapsedMilliseconds} 毫秒");

            // 返回延迟时间
            return (int)stopwatch.ElapsedMilliseconds;
        }







        private void RDPButton1_Click(object sender, EventArgs e)
        {
            if (RDPtextBox1.Text.Length > 0)
            {
                GenerateAndExecuteRdpFileAsync(RDPtextBox1);
            }

        }
        private void RDPButton2_Click(object sender, EventArgs e)
        {
            if (RDPtextBox2.Text.Length > 0) { GenerateAndExecuteRdpFileAsync(RDPtextBox2); }


        }

        private void RDPButton3_Click(object sender, EventArgs e)
        {
            if (RDPtextBox3.Text.Length > 0) { GenerateAndExecuteRdpFileAsync(RDPtextBox3); }

        }

        private void RDPButton4_Click(object sender, EventArgs e)
        {
            if (RDPtextBox4.Text.Length > 0) { GenerateAndExecuteRdpFileAsync(RDPtextBox4); }


        }

        private void SSHButton1_Click(object sender, EventArgs e)
        {

            HandleSSHLogin(SSHtextBox1);
        }

        private void SSHButton2_Click(object sender, EventArgs e)
        {

            HandleSSHLogin(SSHtextBox2);
        }

        private void flashFileList()
        {
            // 获取执行文件目录下 .config 文件夹中的文件列表
            // List<string> fileList = GetConfigFiles();
            List<string> fileList = GetConfigFilesSortedByDate();
            // 将文件列表绑定到 ListBox
            HistoryComboBox.DataSource = fileList;
        }

        public static void ExecuteSshConnectionLinux(string ipAddress)
        {
            // 根据输入判断是否包含端口号
            int defaultPort = 22;
            string[] parts = ipAddress.Split(':');
            string ip = parts[0].Trim();
            int port = parts.Length > 1 ? int.Parse(parts[1].Trim()) : defaultPort;

            // 构造 SSH 命令
            string sshCommand = $"ssh root@{ip} -p {port} -o StrictHostKeyChecking=no";

            // 执行 SSH 命令
            Process process = new Process();
            process.StartInfo.FileName = "cmd"; // 终端程序的名称
            process.StartInfo.Arguments = "/c " + sshCommand; // 使用 -e 参数来执行 SSH 命令
            process.Start();

            // 等待远程连接工具关闭
            //process.WaitForExit();
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








        public static string GetFirstSshFileName()
        {
            string rdpConfigFolderPath = Path.Combine(Environment.CurrentDirectory, RDPFileName);

            // 检查 .RDPconfig 文件夹是否存在
            if (!Directory.Exists(rdpConfigFolderPath))
            {
                // 如果文件夹不存在，返回空字符串或者抛出异常，根据需求决定
                return string.Empty;
            }

            // 获取 .ssh 文件
            string[] files = Directory.GetFiles(rdpConfigFolderPath, "*.ssh");
            // 检查是否有文件
            if (files.Length == 0)
            {
                // 如果没有文件，返回空字符串或者抛出异常，根据需求决定
                return string.Empty;
            }


            // 返回第一个文件的名称（不包含路径部分）
            string fileName = Path.GetFileName(files[0]);
            string sshIP = fileName.Split('-')[0];
            string sshPort = fileName.Split('-')[1].Split('.')[0];

            //// 查找 ".ssh" 的位置
            //int indexOfDotSsh = fileName.IndexOf(".ssh");

            //// 如果找到 ".ssh"，截取从索引 0 到 indexOfDotSsh 的部分
            //// 如果没有找到 ".ssh"，截取整个字符串
            //string ipAddress = indexOfDotSsh >= 0 ? fileName.Substring(0, indexOfDotSsh) : fileName;

            return sshIP + ":" + sshPort;
        }
        // 生成RDP文件
        public static void GenerateSSHFile(string fullAddress)
        {
            // 创建 .config 文件夹（如果不存在）
            string configFolderPath = Path.Combine(Environment.CurrentDirectory, RDPFileName);
            Directory.CreateDirectory(configFolderPath);


            // 解析 IP 和端口号
            string[] addressParts = fullAddress.Split(':');
            string ipAddress = addressParts[0];
            string port = (addressParts.Length > 1) ? addressParts[1] : "22";

            // 创建 SSH 文件路径
            string sshFilePath = Path.Combine(configFolderPath, $"{ipAddress}-{port}.ssh");

            // 设置 SSH 文件的内容
            string sshFileContent = "";

            // 写入内容到 SSH 文件
            File.WriteAllText(sshFilePath, sshFileContent);
        }
        public static void DeleteSingleSshFile()
        {
            string rdpConfigFolderPath = Path.Combine(Environment.CurrentDirectory, RDPFileName);

            // 检查 .RDPconfig 文件夹是否存在
            if (!Directory.Exists(rdpConfigFolderPath))
            {
                // 如果文件夹不存在，则没有 .ssh 文件需要删除
                return;
            }

            // 获取 .ssh 文件
            string[] sshFiles = Directory.GetFiles(rdpConfigFolderPath, "*.ssh");

            // 检查是否只有一个 .ssh 文件
            if (sshFiles.Length == 1)
            {
                // 删除 .ssh 文件
                File.Delete(sshFiles[0]);
                Console.WriteLine("Deleted the single .ssh file.");
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



        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }





        static Dictionary<string, Dictionary<string, string>> ReadIniFile(string filePath)
        {
            Dictionary<string, Dictionary<string, string>> iniData = new Dictionary<string, Dictionary<string, string>>();
            string currentSection = "";
            Dictionary<string, string> currentSectionData = null;

            foreach (string line in File.ReadLines(filePath))
            {
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    currentSection = line.Trim('[', ']');
                    currentSectionData = new Dictionary<string, string>();
                    iniData[currentSection] = currentSectionData;
                }
                else if (currentSectionData != null && line.Contains("="))
                {
                    string[] parts = line.Split('=');
                    if (parts.Length == 2)
                    {
                        string key = parts[0].Trim();
                        string value = parts[1].Trim();
                        currentSectionData[key] = value;
                    }
                }
            }
            return iniData;
        }


        // 获取历史连接列表
        private void LoadSectionsToComboBox(ComboBox comboBox, string filePath)
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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }



        private void TcpPingCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (TcpPingCheck.Checked)
            {
                timer.Enabled = true;
                lastPingSuccess1 = false;
                lastPingSuccess2 = false;
                tcpPingFlag = TcpPingCheck.Checked;
                Properties.Settings.Default.tcpPingFlag = TcpPingCheck.Checked;
                Properties.Settings.Default.Save();
                
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
                    SSHLabel2.Text = "SSH";
                    SSHLabel1.Text = "SSH";
                }
                else
                {
                    SSHLabel2.Text = "RDP";
                    SSHLabel1.Text = "RDP";
                }
            }
            tcpPingFlag = TcpPingCheck.Checked;
            Properties.Settings.Default.tcpPingFlag = tcpPingFlag;
            Properties.Settings.Default.Save();
        }

        private void FullScreenCheck_CheckedChanged(object sender, EventArgs e)
        {
            fullScreenFlag = FullScreenCheck.Checked;
            screenMode = fullScreenFlag ? "0" : "1";
            Properties.Settings.Default.fullScreenFlag = fullScreenFlag;
        }
        private void DeleteRDPCheck_CheckedChanged(object sender, EventArgs e)
        {
            deleteFlag = DeleteRDPCheck.Checked;
            Properties.Settings.Default.deleteFlag = DeleteRDPCheck.Checked;
            Properties.Settings.Default.Save();
        }
        private void TopMostCheck_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = TopMostCheck.Checked;
            topFlag = TopMostCheck.Checked;
            Properties.Settings.Default.topFlag = topFlag;
        }

        private void OpenRDPListButton_Click(object sender, EventArgs e)
        {
            // 指定文件夹的路径
            string folderPath = @".\.RDPconfig";

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




  

        private void ExitButton_Click(object sender, EventArgs e)
        {
            // 在退出按钮的Click事件中调用Close方法来关闭窗体
            this.Close();
        }


        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            SSHLabel1.Text = SSHFlag ? "SSH" : "RDP";
            SSHLabel2.Text = SSHFlag ? "SSH" : "RDP";
        }




 

        private void button10_Click(object sender, EventArgs e)
        {

        }

        private void button10_Click_1(object sender, EventArgs e)
        {

        }

        private void SSHtextBox2_TextChanged(object sender, EventArgs e)
        {
            lastPingSuccess1 = false;
        }
        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }





        private void label3_Click(object sender, EventArgs e)
        {

        }


        private void SSHtextBox1_TextChanged(object sender, EventArgs e)
        {
            lastPingSuccess2 = false;
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
            UpdateAttribute();
            OpenSettingsForm();
        }

        // 更新系统属性
        private void UpdateAttribute()
        {
            Properties.Settings.Default.deleteFlag = deleteFlag;
            Properties.Settings.Default.tcpPingFlag = tcpPingFlag;
            Properties.Settings.Default.fullScreenFlag = fullScreenFlag;
            Properties.Settings.Default.topFlag = topFlag;
            //Properties.Settings.Default.SSHFlag = SSHFlag;
            //Properties.Settings.Default.SSHMethodFlag = SSHMethod;
            //Properties.Settings.Default.SSHLowPingFlag = SSHLowPingFlag;
            Properties.Settings.Default.Save();
        }

            // 读取属性和同步勾选框
            private void refreshCheckboxState()
        {
            // 读取系统属性
            deleteFlag = Properties.Settings.Default.deleteFlag;
            tcpPingFlag = Properties.Settings.Default.tcpPingFlag;
            fullScreenFlag = Properties.Settings.Default.fullScreenFlag;
            topFlag = Properties.Settings.Default.topFlag;
            SSHFlag = Properties.Settings.Default.SSHFlag;
            SSHMethod = Properties.Settings.Default.SSHMethodFlag;
            SSHLowPingFlag = Properties.Settings.Default.SSHLowPingFlag;
            screenMode = fullScreenFlag ? "0" : "1";  // 全屏=0，窗口=1

            // 同步Checkbox选择
            DeleteRDPCheck.Checked = Properties.Settings.Default.deleteFlag;
            TcpPingCheck.Checked = Properties.Settings.Default.tcpPingFlag;
            FullScreenCheck.Checked = Properties.Settings.Default.fullScreenFlag;
            TopMostCheck.Checked = Properties.Settings.Default.topFlag;
            //SSHLoginCheck.Checked = Properties.Settings.Default.SSHFlag;
            //SSHMethodCheck.Checked = Properties.Settings.Default.SSHMethodFlag;
            //SSHLowPingCheck.Checked = Properties.Settings.Default.SSHLowPingFlag;
        }

        // 当SettingsForm关闭时MainForm触发该事件
        private void SettingForm_FormClosedEvent(object sender, EventArgs e)
        {
            // Console.WriteLine("MainForm refreshCheckboxState()");
            refreshCheckboxState();
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
                                if (TcpPingCheck.Checked) { timer.Start(); }

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
                MessageBox.Show("无法加载嵌入的图标资源。");
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
            MessageBox.Show("Auther by hmomeng@gmail.com");
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
    }
}
