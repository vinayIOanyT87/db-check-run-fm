namespace InProcLogging
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Threading;


	public class FileLogger : SrmThread, ILogger
    {
        #region Classes

        private class LogMessage
        {
            private LogSeverity severity;
            private string message;
            private string threadId;
            private DateTime executeTime;

            public LogMessage(LogSeverity severity, string message, string threadId, DateTime executeTime)
            {
                this.severity = severity;
                this.message = message;
                this.threadId = threadId;
                this.executeTime = executeTime;
            }

            public override string ToString()
            {
                return string.Format("{0} | {1} | {2} | {3}",
                    this.executeTime.ToString("MM/dd/yyyy hh:mm:ss.ff"),
                    this.severity, this.message, this.threadId);
            }
        } 

        #endregion

        #region Fields

        private static Dictionary<int, FileLogger> threadInstanceParameter = new Dictionary<int, FileLogger>();
        private string fileName = "";
        private string backupFileName = "";
        private long maximumFileSize = 1024; //kilobytes
        private Queue<LogMessage> logMessages = new Queue<LogMessage>();
        private AutoResetEvent waitHandle = new AutoResetEvent(false);
        private readonly int minimumQueueSizeToWrite = 10;
        private LogSeverity severityLogged = LogSeverity.Debug;
        private ThreadPriority threadPriority = ThreadPriority.Lowest;
        private int dropCount = 0;

        #endregion

        #region Properties

        public LogSeverity SeverityLogged
        {
            get
            {
                return this.severityLogged;
            }
            set
            {
                this.severityLogged = value;
            }
        }

        #endregion

        #region Methods

        public FileLogger(params object[] args)
        {
            try
            {
                string logFileName = (string)args[0];
                int maxLogFileSizeKb = (int)args[1];
                LogSeverity loggingLevel = (LogSeverity)args[2];
                ThreadPriority priority = (ThreadPriority) args[3];
                this.Initialize(logFileName, maxLogFileSizeKb, loggingLevel, priority);
            }
            catch(Exception)
            {                
            }
        }

        private void Initialize(string logFileName, int maxLogFileSizeKb, 
                                LogSeverity loggingLevel, ThreadPriority priority)
        {
            this.fileName = logFileName;
            this.backupFileName = this.fileName + ".previous";
            this.maximumFileSize = maxLogFileSizeKb;
            this.SeverityLogged = loggingLevel;
            this.threadPriority = priority;
            //CreateMessageThread();
            this.Start();
        }

        public FileLogger(string logFileName, int maxLogFileSizeKb, LogSeverity loggingLevel, 
                          ThreadPriority priority)
        {
            this.Initialize(logFileName, maxLogFileSizeKb, loggingLevel, priority);
        }

        //private void CreateMessageThread()
        //{
        //    messageWritingThread = new Thread(new ThreadStart(WritingThreadLoop));
        //    // Since there isn't a paramaterized thread start on CE, we need to store the parameters for 
        //    // a thread in a dictionary by the thread's ID.
        //    int id = messageWritingThread.ManagedThreadId;
        //    AddThreadInstanceParameter(id, this);
        //    messageWritingThread.Priority = threadPriority;
        //    messageWritingThread.IsBackground = true;
        //    messageWritingThread.Start();
        //}

        /// <summary>
        /// Logs the specified message severity.
        /// </summary>
        /// <param name="messageSeverity">The message severity.</param>
        /// <param name="message">The message.</param>
        public void Log(LogSeverity messageSeverity, string message)
        {
            this.AddLogMessage(messageSeverity, message);
        }

        public void Log(LogSeverity severity, string message, params object[] args)
        {
            this.Log(severity, string.Format(message, args));
        }

        public void Flush()
        {
            this.PurgeLogMessages();
        }

        private void AddLogMessage(LogSeverity messageSeverity, string message)
        {
            if (messageSeverity >= this.SeverityLogged)
            {
                string threadId = Thread.CurrentThread.Name;
                if (threadId == null)
                {
                    threadId = Thread.CurrentThread.ManagedThreadId.ToString();
                }
                this.EnqueueLogMessage(new LogMessage(messageSeverity, message, threadId, DateTime.Now));
            }
        }

        private void EnqueueLogMessage(LogMessage logMessage)
        {
            lock (this.logMessages)
            {
                if (this.minimumQueueSizeToWrite * 2 > this.logMessages.Count)
                {
                    this.logMessages.Enqueue(logMessage);
                }
                else
                {
                    this.dropCount++;
                } 
            }
            this.SignalWrite();
        }

        private LogMessage DequeLogMessage()
        {
            lock (this.logMessages)
            {
                if (this.logMessages.Count > 0)
                {
                    return this.logMessages.Dequeue();
                }
                return null;
            }
        }

        private void SignalWrite()
        {
            if (this.logMessages.Count > this.minimumQueueSizeToWrite)
                this.waitHandle.Set();
        }

        private void LogFileSizeCheck()
        {
            FileInfo info = new FileInfo(this.fileName);
            if (info.Length >= this.maximumFileSize * 1024)
            {
                if (File.Exists(this.backupFileName))
                {
                    File.Delete(this.backupFileName);
                }
                File.Move(this.fileName, this.backupFileName);
            }
        }

        private string errStr = "";

        public string ErrStr
        {
            get { lock (this.logMessages) { return this.errStr; } }
            set { lock (this.logMessages) { this.errStr = value; } }
        }

        private void MemberThreadLoop()
        {
            while (!this.mShutdown)
            {
                this.waitHandle.WaitOne(10000, false);
                if (this.logMessages.Count > 0)
                {
                    IntPtr fd = (IntPtr)FileOps.INVALID_HANDLE_VALUE;
                    try
                    {
                        if (!FileOps.AppendFile(this.fileName, out fd))
                        {
                            continue;
                        }
                        int drpCnt = 0;
                        lock (this.logMessages)
                        {
                            drpCnt = this.dropCount;
                            this.dropCount = 0;
                        }
                        uint fileSize = 0;
                        if (drpCnt > 0)
                        {
                            FileOps.WriteLogStatement(ref fd, drpCnt.ToString() + " dropped log statements!!!!!\r\n", out fileSize);
                        }
                        LogMessage message = this.DequeLogMessage();
                        while (message != null)
                        {
                            string messageString = message.ToString() + "\r\n";

                            bool retVal = FileOps.WriteLogStatement(ref fd, messageString, out fileSize);
                            if (fileSize >= this.maximumFileSize * 1024)
                            {
                                break;
                            }
                            else
                            {
                                message = this.DequeLogMessage();
                            }
                        }
                    }
                    catch (Exception exc1)
                    {
                        this.errStr = exc1.Message;
                    }
                    finally
                    {
                        if (fd.ToInt32() != FileOps.INVALID_HANDLE_VALUE)
                        {
                            FileOps.CloseFile(ref fd);
                        }
                    }
                    this.LogFileSizeCheck();
                }
            }
        }

        public override void Run()
        {
            Thread local = Thread.CurrentThread;
            local.Priority = this.threadPriority;
            local.IsBackground = true;

            while(this.mShutdown == false)
            {
                try
                {
                    this.MemberThreadLoop();
                }
                catch (ThreadAbortException)
                {
                    //PurgeLogMessages();
                    return;
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        private void PurgeLogMessages()
        {
            while (this.logMessages.Count > 0)
            {
                this.waitHandle.Set();
                Thread.Sleep(10);
            }
        }

        public void Dispose()
        {
            this.mShutdown = true;
            this.waitHandle.Set();
        }

        #endregion
    }
}
