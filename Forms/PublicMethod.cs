using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Remote.Forms
{
    internal class PublicMethod
    {

        const string IniFileName = "config.ini";
        public static void CreateINIfile()
        {
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
        }

        public static bool ParseTextToRemoteInfo(string textbox, out string ip, out string port, out string username, out string password, string defaultPort = "3389")
        {
            ip = string.Empty;
            port = defaultPort;
            username = defaultPort == "3389" ? "Administrator" : "root";
            password = string.Empty;
            string replacedString = textbox;
            if (replacedString.EndsWith("\r\n"))
            {
                replacedString = replacedString.TrimEnd('\r', '\n');
            }
            // 判断并去除前后的双引号
            if (replacedString.StartsWith("\"") && replacedString.EndsWith("\""))
            {
                // 使用 TrimStart 和 TrimEnd 方法去除前后的双引号
                replacedString = replacedString.Substring(1, replacedString.Length - 2);
            }

            //Console.WriteLine(textbox);
            // 去除所有非大小写字母、阿拉伯数字、英文标点的字符
            string pattern = @"[^\x00-\x7F]+";
            replacedString = Regex.Replace(replacedString, pattern, "\n");
            replacedString = replacedString.Replace(" ", "\n");
            replacedString = replacedString.Replace("\r\n", "\n");
            replacedString = Regex.Replace(replacedString, @"\n+", "\n");
            //Console.WriteLine(replacedString);
            // 匹配IP地址
            string ipAddressPattern = @"\b(?:\d{1,3}\.){3}\d{1,3}\b";
            Match firstMatch = Regex.Match(replacedString, ipAddressPattern);
            if (!firstMatch.Success) { return false; }
            TryParseIpAndPort(replacedString, out ip, out port, defaultPort);
            //Console.WriteLine(replacedString);
            int index = firstMatch.Index;
            if (index > 0)
            {
                // 删除IP地址前的字符
                replacedString = replacedString.Remove(0, index);
            }
            // 输出结果
           //Console.WriteLine("删除完IP前的字符后：" + replacedString);


            string[] elements = replacedString.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            //如果解析完IP后没有账密，则不在继续
            //Console.WriteLine(elements.Length);
            if (elements.Length <= 1) { return true; }
            if (elements.Length == 2) { username = elements[1]; return true; }
            if (elements.Length >= 3) { username = elements[1]; password = elements[2]; return true; }
            //Console.WriteLine($@"host={ip}:{port},username={elements[1]},password={elements[2]}");
            return true;
        }

        public static void TryParseIpAndPort(string text, out string ip, out string port, string defaultPort)
        {
            ip = null;
            port = defaultPort;
            // 使用正则表达式匹配IP和端口
            Match ipPortMatch = Regex.Match(text, @"(\d+\.\d+\.\d+\.\d+)(?::(\d+))?");
            if (ipPortMatch.Success)
            {
                ip = ipPortMatch.Groups[1].Value;
                port = ipPortMatch.Groups[2].Success ? ipPortMatch.Groups[2].Value : defaultPort;
            }

        }
    }
}
