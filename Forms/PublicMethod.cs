using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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



    }
}
