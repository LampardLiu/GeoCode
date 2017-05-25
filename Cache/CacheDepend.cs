using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace GeoCode
{
    public class CacheDepend<TCache> where TCache : class
    {
        private string DependFileFullName;
        private string DependPath;
        private string DependFileName;
        private TCache CacheObj;
        private FileSystemWatcher Watcher;
        private string Md5Value;
        private StringBuilder sb = new StringBuilder(32);
        /// <summary>
        /// 依赖文件发生了变化
        /// </summary>
        public event EventHandler<TCacheArge<TCache>> DependFiledChanged;

        /// <summary>
        /// 构造（指定缓存和缓存依赖文件）
        /// </summary>
        /// <param name="cache">缓存</param>
        /// <param name="dependFilePath">依赖文件</param>
        public CacheDepend(TCache cache, string dependFilePath)
        {
            if (!File.Exists(dependFilePath) || dependFilePath.EndsWith("\\"))
            {
                throw new ArgumentException("Arge：dependFilePath不是有效的文件名【" + dependFilePath + "】或者文件不存在");
            }
            DependFileFullName = dependFilePath;
            DependPath = dependFilePath.Substring(0, dependFilePath.LastIndexOf("\\") + 1);
            DependFileName = dependFilePath.Substring(dependFilePath.LastIndexOf("\\") + 1);
            CacheObj = cache;
            StartWatchFile();
        }

        /// <summary>
        /// 监控文件
        /// </summary>
        private void StartWatchFile()
        {
            Watcher = new FileSystemWatcher(DependPath);
            Watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            Watcher.Filter = DependFileName;
            Watcher.EnableRaisingEvents = true;
            Watcher.Changed += (sender, e) =>
            {
                //判断文件已经改变，执行方法体
                if (FileContentChaged(DependFileFullName))
                {
                    if (DependFiledChanged != null)
                    {
                        DependFiledChanged(sender, new TCacheArge<TCache>(CacheObj));
                    }
                }
            };
        }

        /// <summary>
        /// 判断文件是否改变，如果改变直接更新md5
        /// </summary>
        /// <param name="fileFullName">文件完整路径（包含路径和文件名以及文件后缀）</param>
        /// <returns>true 表示改变，fasle表示没变</returns>
        private bool FileContentChaged(string fileFullName)
        {
            //首次，没有md5
            if (string.IsNullOrEmpty(Md5Value))
            {
                Md5Value = GetFileMd5Value(fileFullName);
                return true;
            }
            var md5value = GetFileMd5Value(fileFullName);
            //文件md5没变，文件内容没变
            if (md5value == Md5Value) return false;
            //文件发生了改变
            Md5Value = md5value;
            return true;
        }

        /// <summary>
        /// 获取文件md5
        /// </summary>
        /// <param name="fileFullName">文件完整路径（包含路径和文件名以及文件后缀）</param>
        /// <returns>文件的md5</returns>
        private string GetFileMd5Value(string fileFullName)
        {
            using (FileStream fs = new FileStream(fileFullName, FileMode.Open))
            {
                var md5buffer = MD5.Create().ComputeHash(fs);
                sb.Clear();
                for (int i = 0; i < md5buffer.Length; i++)
                {
                    sb.Append(md5buffer[i].ToString("x2"));
                }
                fs.Close();
                return sb.ToString();
            }
        }
    }

    /// <summary>
    /// 事件参数
    /// </summary>
    /// <typeparam name="TCache"></typeparam>
    public class TCacheArge<TCache> : EventArgs where TCache : class
    {
        public TCache Cache { get; set; }
        public TCacheArge(TCache cache)
        {
            this.Cache = cache;
        }
    }
}
