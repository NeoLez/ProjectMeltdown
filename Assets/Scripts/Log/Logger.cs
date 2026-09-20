using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Root.Log
{
    public static class Logger
    {
        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void Log(string msg, LogType type, LogSeverity severity = LogSeverity.Normal, Object obj = null) {
            if (!LoggerSettings.ShouldLogSeverity(severity) || !LoggerSettings.ShouldLogType(type)) return;
            
            msg = $"[{type}] - {msg}";
            switch (severity)
            {
                case LogSeverity.Normal:
                    Debug.Log(msg, obj);
                    break;
                case LogSeverity.Warning:
                    Debug.LogWarning(msg, obj);
                    break;
                case LogSeverity.Error:
                    Debug.LogError(msg, obj);
                    break;
                case LogSeverity.Fatal:
                    Debug.LogError(msg, obj);
                    break; 
            }
        }
    }
}