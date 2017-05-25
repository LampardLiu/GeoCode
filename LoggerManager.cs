using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using System.Configuration;
using System.Diagnostics;

namespace GeoCode
{
    /// <summary>
    /// 日志类
    /// </summary>
    public class LoggerManager
    {
        /// <summary>
        /// Nlog操作对象
        /// </summary>
        public static Logger Logger = LogManager.GetCurrentClassLogger();
        static bool IsLoggerInfo = true;
        static StringBuilder infoSb = new StringBuilder();
        private LoggerManager()
        {
            var islogger = System.Configuration.ConfigurationManager.AppSettings["GeoCode_IsLoggerInfo"];
            if (islogger.ToLower() != "true")
            {
                IsLoggerInfo = false;
            }
        }

        /// <summary>
        /// 记录方法调用时间和参数信息
        /// </summary>
        /// <param name="action">执行的行为</param>
        /// <param name="parameters">参数名，将所有参数使用逗号分隔</param>
        /// <param name="arges">需要记录的参数</param>
        public static void LogTimeInfo(Action action, string parameters, params object[] arges)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (action != null)
                action();
            sw.Stop();
            infoSb.Clear();
            infoSb.Append("执行方法【");
            infoSb.Append(action.Method.Name);
            infoSb.Append("】共计耗时(毫秒):");
            infoSb.Append(sw.Elapsed.Milliseconds);
            if (arges != null)
            {
                infoSb.Append("\r\n*****参数列表：");
                infoSb.Append("参数名：[");
                infoSb.Append(parameters);
                infoSb.Append("]");
                foreach (var arg in arges)
                {
                    infoSb.Append(arg);
                    infoSb.Append(',');
                };
            }
            Logger.Info(infoSb.ToString());
        }

    }
}
