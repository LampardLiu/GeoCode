using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace GeoCode.Tool
{
    public class UsedDataManager
    {
        private static readonly string DataFilePath = System.AppDomain.CurrentDomain.BaseDirectory + "geocodedata\\";
        private static Dictionary<string, int> KeyUsedCount = new Dictionary<string, int>();
        private static Thread cycleThread;
        private static TimeSpan CycleFlushInterval = TimeSpan.FromSeconds(10);
        /// <summary>
        /// 是否为新的一天
        /// </summary>
        public static bool IsNewDay
        {
            get
            {
                var currentDayFileFullName = DataFilePath + DateTime.Now.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture) + ".dat";
                if (File.Exists(currentDayFileFullName))
                {
                    return false;
                }
                else
                {
                    if (Directory.Exists(DataFilePath))
                    {
                        Directory.Delete(DataFilePath, true);
                    }
                    Directory.CreateDirectory(DataFilePath);
                    File.Create(currentDayFileFullName);
                    return true;
                }
            }
        }

        static UsedDataManager()
        {
            if (!Directory.Exists(DataFilePath))
                Directory.CreateDirectory(DataFilePath);
            var currentDayFileFullName = DataFilePath + DateTime.Now.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture) + ".dat";
            FileToDictiory(currentDayFileFullName);
            CycleFlushDataToFile();
        }

        /// <summary>
        /// 设置用户使用次数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="count">使用次数</param>
        internal static void SetUseCount(string key, int count)
        {
            if (KeyUsedCount.Keys.Contains(key))
            {
                KeyUsedCount[key] = count;
            }
        }

        /// <summary>
        /// 获取用户使用次数
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>使用次数</returns>
        internal static int GetUsedCount(string key)
        {
            if (!KeyUsedCount.ContainsKey(key))
            {
                WriteAndReLoadDataFile(key, DataFilePath + DateTime.Now.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture) + ".dat");
            }
            return KeyUsedCount[key];
        }

        /// <summary>
        /// 写文件并且将新的内容重新加载,为了减少文件的打开和关闭次数，将两个行为合并在一起
        /// </summary>
        /// <param name="key"></param>
        /// <param name="filename"></param>
        private static void WriteAndReLoadDataFile(string key, string filename)
        {
            lock (KeyUsedCount)
            {
                try
                {
                    FileInfo fileInfo = new FileInfo(filename);
                    using (FileStream fs = fileInfo.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                    {
                        var buffer = new byte[fs.Length];
                        if (fs.CanRead)
                            fs.Read(buffer, 0, (int)fs.Length);
                        var content = Encoding.UTF8.GetString(buffer);
                        //将文件内容对应到内存对象中
                        FileContentToDictiory(content);
                        //添加新的key
                        KeyUsedCount.Add(key, 0);
                        content += "\r\n" + key + " 0";
                        var writebuffer = Encoding.UTF8.GetBytes(content);
                        fs.Write(writebuffer, 0, writebuffer.Length);
                        fs.Flush();
                        fs.Close();
                    }
                }
                catch (Exception ex)
                {
                    LoggerManager.Logger.Error(ex);
                }
            }
        }

        /// <summary>
        /// 将文件内容转存储到内存字段中
        /// </summary>
        /// <param name="file"></param>
        private static void FileToDictiory(string filename)
        {
            if (File.Exists(filename))
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open))
                {
                    var buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    fs.Close();
                    var content = Encoding.UTF8.GetString(buffer);
                    var keyList = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string item in keyList)
                    {
                        if (item.StartsWith("#"))
                            continue;
                        var keyAndCountList = item.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (keyAndCountList.Length != 2) continue;
                        int count;
                        if (int.TryParse(keyAndCountList[1], out count))
                        {
                            if (KeyUsedCount.ContainsKey(keyAndCountList[0]))
                            {
                                KeyUsedCount[keyAndCountList[0]] = count;
                            }
                            else
                            {
                                KeyUsedCount.Add(keyAndCountList[0], count);
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 将文件内容转存储到内存字段中
        /// </summary>
        /// <param name="content"></param>
        private static void FileContentToDictiory(string content)
        {
            var keyList = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in keyList)
            {
                if (item.StartsWith("#"))
                    continue;
                var keyAndCountList = item.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (keyAndCountList.Length != 2) continue;
                int count;
                if (int.TryParse(keyAndCountList[1], out count))
                {
                    if (KeyUsedCount.ContainsKey(keyAndCountList[0]))
                    {
                        KeyUsedCount[keyAndCountList[0]] = count;
                    }
                    else
                    {
                        KeyUsedCount.Add(keyAndCountList[0], count);
                    }
                }
            }
        }

        /// <summary>
        /// 将内存中的数据更新到硬盘
        /// </summary>
        /// <param name="filename"></param>
        private static void FlushDataToFile(string filename)
        {
            lock (KeyUsedCount)
            {
                try
                {
                    FileInfo fileInfo = new FileInfo(filename);
                    using (FileStream fs = fileInfo.OpenWrite())
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (var key in KeyUsedCount.Keys)
                        {
                            sb.Append(key);
                            sb.Append(" ");
                            sb.Append(KeyUsedCount[key]);
                        }
                        var buffer = Encoding.UTF8.GetBytes(sb.ToString());
                        sb.Clear();
                        sb = null;
                        fs.Write(buffer, 0, buffer.Length);
                        fs.Flush();
                        fs.Close();
                    }
                }
                catch (Exception ex)
                {
                    LoggerManager.Logger.Error(ex);
                }
            }
        }

        /// <summary>
        /// 循环刷新数据
        /// </summary>
        private static void CycleFlushDataToFile()
        {
            if (cycleThread == null)
            {
                var filename = DataFilePath + DateTime.Now.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture) + ".dat";
                cycleThread = new Thread(() =>
                {
                    while (true)
                    {
                        if (File.Exists(filename))
                        {
                            FlushDataToFile(filename);
                        }
                        Thread.Sleep(CycleFlushInterval);
                    }
                });
            }
            if (cycleThread.ThreadState != ThreadState.Running)
                cycleThread.Start();
        }
    }
}
