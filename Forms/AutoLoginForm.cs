using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Remote.Forms
{
    public partial class AutoLoginForm : Form
    {

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
        public static string screenMode = fullScreenFlag ? "0" : "1";  // 全屏=0，窗口=1
        // 读取RDP文件
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int GetPrivateProfileString(string section, string key, string defaultValue, StringBuilder returnedString, int size, string filePath);


        public AutoLoginForm()
        {
            InitializeComponent();
        }

        // 去除中文字符和空行
        private static string RemoveChineseAndTrim(string input)
        {
            // 保留英文字符和数字，去除中文字符和符号
            return Regex.Replace(input, @"[^\x00-\x7F]+", "").Trim();
        }
        // 获取默认账密
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


        // 默认账密连接


        private void AutoLoginForm_Load(object sender, EventArgs e)
        {
            LoadSectionsToComboBox(DefaultPassConnComboBox, IniFileName);
            if (DefaultPassConnComboBox.Items.Count > 0)
            {
                DefaultPassConnComboBox.Text = (string)DefaultPassConnComboBox.Items[0];
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
            screenMode = fullScreenFlag ? "0" : "1";
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

        
        private void LoginButton_Click(object sender, EventArgs e)
        {
            List<string> ipList = new List<string>();
            // 获取 TextBox 中的文本
            string[] lines = autoIP.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            // 解析每行的 IP，并存储起来
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim(); // 去除可能的空格
                if (!string.IsNullOrEmpty(trimmedLine))
                {
                    ipList.Add(trimmedLine);
                }
            }

            // 可以在这里对解析后的 IP 进行进一步处理，如输出到控制台或执行其他操作
            Console.WriteLine("解析到的 IP 列表:");
            foreach (string ip in ipList)
            {
                Console.WriteLine(ip);
            }
            BatchCreateRdpFiles(ipList);
            string[] ipArray = ipList.ToArray();
            string regFilePath = ".RDPconfig/CreateReg.reg";
            GenerateRegFile(regFilePath, ipArray);
            ExecRegFile(regFilePath);

            List<Task> tasks = new List<Task>();
            foreach (string ip in ipList)
            {
                string rdpFileName = $".RDPconfig/{ip}.rdp";
                tasks.Add(Task.Run(() => ExecuteRdpFile(rdpFileName)));
            }

        }
        private void ExecRegFile(string regFile)
        {
            // 构建 regedit.exe 命令
            string regeditCommand = $"regedit.exe /s {regFile}";

            // 调用 ExecuteCommand 方法执行命令
            string commandOutput = ExecuteCommand(regeditCommand);

            // 打印命令执行结果
            Console.WriteLine($"Command Output:\n{commandOutput}");
        }

        private void BatchCreateRdpFiles(List<string> ipList)
        {
            foreach (string ip in ipList)
            {
                DefaultPassConnComboBox.Text = RemoveChineseAndTrim(DefaultPassConnComboBox.Text);
                string addr = ip;
                string username = "";
                string password = "";
                string iniFilePath = System.IO.Path.Combine(Application.StartupPath, IniFileName);

                StringBuilder result = new StringBuilder(255); // 分配缓冲区大小
                int size = GetPrivateProfileString(DefaultPassConnComboBox.Text, "端口", "", result, result.Capacity, iniFilePath);
                string port = result.ToString().Trim();
                addr = ip + ":" + port;
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

                // 如果需要执行 RDP 文件，可以在这里添加执行的代码
                //string rdpFilePath = Path.Combine(Path.Combine(Environment.CurrentDirectory, RDPFileName), addr.Split(':')[0] + ".rdp");
                //ExecuteRdpFile(rdpFilePath);
            }
        }

        private void DefaultPassConnButton_Click(object sender, EventArgs e)
        {
            DefaultPassConnComboBox.Text = RemoveChineseAndTrim(DefaultPassConnComboBox.Text);
            string addr = "";
            string username = "";
            string password = "";
            string iniFilePath = System.IO.Path.Combine(Application.StartupPath, IniFileName);

            StringBuilder result = new StringBuilder(255); // 分配缓冲区大小
            int size = GetPrivateProfileString(DefaultPassConnComboBox.Text, "端口", "", result, result.Capacity, iniFilePath);
            string port = result.ToString().Trim();
            addr = DefaultPassConnComboBox.Text + ":" + port;
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
        public static void GenerateRegFile(string filePath, string[] ipList)
        {
            try
            {
                // 创建一个StringBuilder来构建reg文件内容
                StringBuilder regContent = new StringBuilder();

                // 添加reg文件头部信息
                regContent.AppendLine("Windows Registry Editor Version 5.00");

                // 添加注册表项路径
                regContent.AppendLine("[HKEY_CURRENT_USER\\Software\\Microsoft\\Terminal Server Client\\LocalDevices]");

                // 遍历IP列表生成注册表项
                foreach (string ip in ipList)
                {
                    string registryEntry = $"\"{ip}\"=dword:0000004c";
                    regContent.AppendLine(registryEntry);
                }

                // 将StringBuilder内容写入文件
                File.WriteAllText(filePath, regContent.ToString());

                Console.WriteLine($"Reg file '{filePath}' generated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public static void ExecuteRdpFile(string rdpFileName)
        {
            // 构建 mstsc.exe 命令
            string mstscCommand = $"mstsc.exe {rdpFileName}";
            
            Console.WriteLine($"Error executing command: {mstscCommand}");
            // 调用 ExecuteCommand 方法执行命令


            try
            {
                ExecuteCommand(mstscCommand);
                
            }
            catch (Exception ex)
            {
                // 处理异常，例如打印错误消息或记录日志
                Console.WriteLine($"Error executing command: {ex.Message}");
            }
        }

       

        private void ClearRegButton_Click(object sender, EventArgs e)
        {
            string regFilePath = "./.RDPconfig/DeleteReg.reg";
            GenerateDeleteRegFile(regFilePath);
            ExecRegFile(regFilePath);
        }

        public static void GenerateDeleteRegFile(string filePath)
        {
           
            try
            {
                // 创建一个StringBuilder来构建reg文件内容
                StringBuilder regContent = new StringBuilder();

                // 添加reg文件头部信息
                regContent.AppendLine("Windows Registry Editor Version 5.00");

                // 添加注册表项路径
                regContent.AppendLine("[-HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Terminal Server Client\\LocalDevices]");
              
                // 将StringBuilder内容写入文件
                File.WriteAllText(filePath, regContent.ToString());

                Console.WriteLine($"Reg file '{filePath}' generated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

}
