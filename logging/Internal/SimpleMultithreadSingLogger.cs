using System;
using System.IO;

namespace DebugOmgDispClient.logging.Internal
{
    /// <summary>
    /// Simple Logger class (based on Singleton), supporting multithreading and sync, 
    /// allows you to record unwrapped data in a text log file, before the class will be deactivated
    /// </summary>
    public class SimpleMultithreadSingLogger : IDisposable
    {
        private static readonly Lazy<SimpleMultithreadSingLogger> instance = new Lazy<SimpleMultithreadSingLogger>(() => new SimpleMultithreadSingLogger());
        
        private readonly StreamWriter streamWriter;
        private readonly object sync = new object();
        private bool disposed;

        public static SimpleMultithreadSingLogger Instance => instance.Value;

        public SimpleMultithreadSingLogger()
        {
            streamWriter = new StreamWriter(@"log.txt", append: true);
        }

        public void Write(string text)
        {
            lock (sync)
            {
                streamWriter.WriteLine(String.Format($"{DateTime.Now} : {text}"));
                streamWriter.Flush();                
            }
        }

        public void Dispose()
        {
            if (disposed) return;
            if (streamWriter != null)
            {
                streamWriter.WriteLine(String.Format($"\n\n")); 
                streamWriter.Dispose();
            }

            disposed = true;
        }
    }    
}
