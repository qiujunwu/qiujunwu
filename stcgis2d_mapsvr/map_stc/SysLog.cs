using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class SysLog
{
    private static string _loggerName = "log4netlogger";
    public static void Debug(string info)
    {
        ILog log = LogManager.GetLogger(_loggerName);
        log.Debug(info);
    }
    public static void Error(string info)
    {
        ILog log = LogManager.GetLogger(_loggerName);
        log.Error(info);
    }
    public static void Fatal(string info)
    {
        ILog log = LogManager.GetLogger(_loggerName);
        log.Fatal(info);
    }
    public static void Info(string info)
    {
        ILog log = LogManager.GetLogger(_loggerName);
        log.Info(info);
    }
    public static void Warn(string info)
    {
        ILog log = LogManager.GetLogger(_loggerName);
        log.Warn(info);
    }
}