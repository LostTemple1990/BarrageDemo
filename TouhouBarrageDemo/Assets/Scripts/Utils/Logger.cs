#if Release
using System.IO;
using System.Threading;
using System.Collections.Concurrent;
#endif
using UnityEngine;

public class Logger
{
#if Release
    private static Logger _logger;

    public static void Open()
    {
        if (_logger == null)
        {
            _logger = new Logger();
        }
    }

    public static void Close()
    {
        _logger.Dispose();
        _logger = null;
    }

    //public static void EndLogFile()
    //{
    //    if (_streamWriter != null)
    //        _streamWriter.Close();
    //    if (_logFileStream != null)
    //        _logFileStream.Close();
    //}

    struct LogData
    {
        public LogType level;
        public string trace;
        public string msg;
    }

    private FileStream _logFileStream = null;
    private StreamWriter _streamWriter = null;

    private string _path;
    private readonly ManualResetEvent _manualResetEvt;
    private readonly ConcurrentQueue<LogData> _msgQueue;

    private bool _isThreadRunning;

    private Logger()
    {
        _path = Application.dataPath + "/../Log.txt";
        if (!System.IO.File.Exists(_path))
        {
            _logFileStream = new FileStream(_path, FileMode.Create);
            _logFileStream.Close();
        }
        _streamWriter = new StreamWriter(_path, true);
        _manualResetEvt = new ManualResetEvent(false);
        _msgQueue = new ConcurrentQueue<LogData>();

        Application.logMessageReceivedThreaded += OnMsgReceived;

        _isThreadRunning = true;

        Thread logThread = new Thread(LogThread);
        logThread.Start();
    }

    private void LogThread()
    {
        while (_isThreadRunning)
        {
            _manualResetEvt.WaitOne();
            if (_streamWriter == null)
                break;
            LogData log;
            while (_msgQueue.Count > 0 && _msgQueue.TryDequeue(out log))
            {
                _streamWriter.WriteLine(log.msg);
            }
            _streamWriter.Flush();
            _manualResetEvt.Reset();
            Thread.Sleep(1);
        }
    }

    private void Dispose()
    {
        Application.logMessageReceivedThreaded -= OnMsgReceived;
        _isThreadRunning = false;
        _manualResetEvt.Set();
        _streamWriter.Close();
        _streamWriter = null;
    }

    private void AddLog(string msg,LogType level)
    {
        LogData log = new LogData
        {
            msg = string.Format("[{0}]{1}",level,msg),
            level = level,
        };
        _msgQueue.Enqueue(log);
        _manualResetEvt.Set();
    }

    private void OnMsgReceived(string msg, string stackTrace, LogType level)
    {
        if (level == LogType.Error || level == LogType.Exception || level == LogType.Assert)
        {
            string logMsg = string.Format("[{0}]{1}\n{2}", level, msg, stackTrace);
            _streamWriter.WriteLine(logMsg);
            _streamWriter.Flush();
            Close();
            Application.Quit();
            //AddLog(logMsg, level);
        }
    }
#endif

    public static void Log(object message)
    {
#if Debug
        Debug.Log(message);
#elif Release
        _logger.AddLog(message.ToString(), LogType.Log);
#endif
    }

    public static void LogWarn(object message)
    {
#if Debug
        Debug.LogWarning(message);
#elif Release
        _logger.AddLog(message.ToString(), LogType.Warning);
#endif
    }

    public static void LogError(object message)
    {
#if Debug
        Debug.LogError(message);
#elif Release
        //_logger.AddLog(message.ToString(), LogType.Error);
        Debug.LogError(message);
#endif
    }
}
