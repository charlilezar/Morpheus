using System.Diagnostics;
using System.Runtime.CompilerServices;
using NLog;

namespace ddPCR.DriverPlatform.Ins.Mcu
{
    public static class LogService
    {
        public static ILogger Logger()
        {
            var frame = new StackFrame(1, false);
            var method = frame.GetMethod();
            var name = method == null ? frame.GetFileName() : method.DeclaringType?.FullName;

            if (name == null) name = typeof(LogService).FullName;

            return LogManager.GetLogger(name);
        }

        public static ILogger Logger(string name)
        {
            return LogManager.GetLogger(name);
        }
    }
}