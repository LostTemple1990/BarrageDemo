﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YKEngine
{
    public class Logger
    {
        private const bool logFlag = true;

        public static void Log(object message)
        {
            if (logFlag)
            {
                Debug.Log(message);
            }
        }

        public static void LogWarn(object message)
        {
            if (logFlag)
            {
                Debug.LogWarning(message);
            }
        }

        public static void LogError(object message)
        {
            if (logFlag)
            {
                Debug.LogError(message);
            }
        }
    }
}
