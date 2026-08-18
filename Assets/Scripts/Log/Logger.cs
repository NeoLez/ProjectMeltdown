using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Root.Log
{
    public static class Logger
    {
        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void Log(string msg, LogType type, LogSeverity severity = LogSeverity.Normal)
        {
            if (LoggerSettings.ShouldLogSeverity(severity) && LoggerSettings.ShouldLogType(type))
            {
                msg = $"[{type.ToString()}] - {msg}";
                switch (severity)
                {
                    case LogSeverity.Normal:
                        Debug.Log(msg);
                        return;
                    case LogSeverity.Warning:
                        Debug.LogWarning(msg);
                        return;
                    case LogSeverity.Error:
                        Debug.LogError(msg);
                        return;
                    case LogSeverity.Fatal:
                        Debug.LogError(msg);
                        return;
                }
            }
        }
    }
}