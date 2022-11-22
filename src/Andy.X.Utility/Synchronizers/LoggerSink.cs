using System;
using System.Diagnostics;

namespace Buildersoft.Andy.X.Utility.Synchronizers
{
    // this class is needed for the Synchronizers logging
    public class LoggerSink
    {
        private readonly string _path;

        public LoggerSink(string path)
        {
            _path = path;
            InitializeSink();
        }

        public void InitializeSink()
        {
            Trace.Listeners.Clear();

            TextWriterTraceListener twtl = new TextWriterTraceListener(_path,
                AppDomain.CurrentDomain.FriendlyName);
            twtl.Name = "TextLogger";
            twtl.TraceOutputOptions = TraceOptions.ThreadId | TraceOptions.DateTime;

            ConsoleTraceListener ctl = new ConsoleTraceListener(false);
            ctl.TraceOutputOptions = TraceOptions.DateTime;

            Trace.Listeners.Add(twtl);
            Trace.Listeners.Add(ctl);
            Trace.AutoFlush = true;
        }
    }
}
