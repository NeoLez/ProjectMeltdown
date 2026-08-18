using Timers;
using UnityEngine;

namespace Root.Log
{
    public class LoggerSettings : ScriptableSingleton<LoggerSettingsSO>
    {
        public static bool ShouldLogType(LogType type)
        {
            InitializeIfNeeded();
            return Instance.types.Contains(type);
        }
        
        public static bool ShouldLogSeverity(LogSeverity severity)
        {
            InitializeIfNeeded();
            return Instance.severities.Contains(severity);
        }

        private static void InitializeIfNeeded()
        {
            if (Instance.update)
            {
                Instance.update = false;
                Instance.types.Clear();
                foreach (var type in Instance.typesToLog)
                {
                    Instance.types.Add(type);
                }
                Instance.severities.Clear();
                foreach (var severity in Instance.severitiesToLog)
                {
                    Instance.severities.Add(severity);
                }
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            LoadAsset();
        }
    }
}