// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Preliy.Flange.Planner.RawInstructions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Preliy.Flange.Planner
{
    public static class Logger
    {
        private const string TAG_DEBUG = "<b><color=#ffc107>DEBUG</color></b>";
        private const string LOG_FORMAT = "{0} >>> {1} {2} {3}";

        public static void Log(LogType logType, object message)
        {
            Preliy.Flange.Logger.UnityLogger.Log(logType, Preliy.Flange.Logger.TAG_PACKAGE, message);
        }
        
        public static void Log(LogType logType, object message, Object context)
        {
            Preliy.Flange.Logger.UnityLogger.Log(logType, Preliy.Flange.Logger.TAG_PACKAGE, message, context);
        }

        public static void Log(LogType logType, Task task, Instruction instruction, string message, string comment = "")
        {
            var format = string.Format(LOG_FORMAT, $"{task.name}", instruction.Name, message, comment);
            Preliy.Flange.Logger.UnityLogger.Log(logType, Preliy.Flange.Logger.TAG_PACKAGE, format, task);
        }
        
        public static void LogVerbose(LogType logType, Task task, Instruction instruction, string message, string comment = "")
        {
            if (!task.Verbose) return;
            var format = string.Format(LOG_FORMAT, $"{task.name}", instruction.Name, message, comment);
            Preliy.Flange.Logger.UnityLogger.Log(logType, $"[{TAG_DEBUG}] {Preliy.Flange.Logger.TAG_PACKAGE}", format, task);
        }
    }

}
