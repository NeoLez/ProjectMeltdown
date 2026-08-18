using System;
using System.Collections.Generic;
using UnityEngine;

namespace Root.Log
{
    [CreateAssetMenu(fileName = "LoggerSettingsSO", menuName = "Settings/LoggerSettings")]
    public class LoggerSettingsSO : ScriptableObject
    {
        public List<LogType> typesToLog;
        public List<LogSeverity> severitiesToLog;
        
        [NonSerialized] public HashSet<LogType> types = new();
        [NonSerialized] public HashSet<LogSeverity> severities = new();
        public bool update = true;
        
    }
}