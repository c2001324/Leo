using UnityEngine;
using System.IO;
using System.Text;
using System;


namespace Untility
{
    /// <summary>
    /// 日志系统
    /// </summary>
    public class Log : Singleton<Log>
    {
        LogType m_LogType;
        bool m_StackTrack;
        bool m_Date;
        string m_Path;

        public Log()
        {
            m_Path = Path.Combine(Application.dataPath, "log.txt");
        }

        /// <summary>
        /// 初始化使用文件日志系统
        /// </summary>
        /// <param name="type">日志级别</param>
        /// <param name="stackTrack">是否输出文件源</param>
        /// <param name="date">是否输出日志日期</param>
        public void Initialize(LogType type, bool stackTrack = true, bool date = true)
        {
            m_LogType = type;
            m_StackTrack = stackTrack;
            m_Date = date;
            Application.logMessageReceivedThreaded += HandleLog;
        }

        public void ClearLog()
        {
            File.WriteAllText(m_Path, "");
        }

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (type > m_LogType && type != LogType.Exception)
            {
                return;
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("[{0}] {1}", type.ToString(), logString);
            if (m_Date)
            {
                DateTime date = DateTime.Now;
                builder.AppendFormat("[{0}-{1} {2}:{3}:{4}]", date.Month, date.Day, date.Hour, date.Minute, date.Second);
            }
            //解析文件源
            if (m_StackTrack && (type == LogType.Error || type == LogType.Exception))
            {
                builder.AppendFormat("\n{0}", stackTrace);
            }
            File.AppendAllText(m_Path, builder.ToString());
        }

    }

}
