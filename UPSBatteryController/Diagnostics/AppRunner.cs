using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace UPSBatteryController.Diagnostics
{
    /// <summary>
    /// Stores data needed for process managing
    /// </summary>
    public class AppRunner : IDisposable
    {
        public const long MAX_BUFFER_LINE_COUNT = 400;

        protected AppRunner()
        {
            this.SystemProcessObject = null;
            this.StdOut = new OutputBuffer();
            this.StdError = new OutputBuffer();
            this.CurrentCommandLine = "";
        }

        public AppRunner(string appPath)
            : this(new string[] { appPath })
        {
        }

        public AppRunner(IEnumerable<string> possibleAppPaths)
            : this()
        {
            this.AppPath = GetAvailableAppPath(possibleAppPaths);

            if (AppPath == null)
                throw new AppNotFoundException();
        }

        protected string QuoteString(string str)
        {
            bool containsSpaces = str.Trim().Contains(' ');
            bool isQuoted = str.Trim().First() == '\"' && str.Trim().Last() == '\"';
            if (!containsSpaces && isQuoted)
                str = str.Trim('\"');
            else if (containsSpaces && !isQuoted)
                str = "\"" + str + "\"";
            return str;
        }

        private string GetAvailableAppPath(IEnumerable<string> possibleAppPaths)
        {
            var res = possibleAppPaths.ToList().Find(path => File.Exists(path));

            if (res == null)
            {
                if (possibleAppPaths.Contains("cmd"))
                    res = "cmd";
                else
                {
                    foreach (var c in possibleAppPaths)
                    {
                        var path = QuoteString(c);
                        if (new CmdRunner().Run(path + " help").ExitCode == 0)
                        {
                            res = path;
                            break;
                        }
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Launch programm in separate process without lock of parent process
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="workingDir"></param>
        public virtual void RunAsync(
            string parameters = "",
            string workingDir = null)
        {
            RunProcess(parameters, workingDir, 0);
        }

        /// <summary>
        /// Launch programm in separate process with lock of parent process
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="workingDir"></param>
        /// <param name="timeout">maximum time for lock</param>
        /// <returns>command result data</returns>
        public virtual CmdResult Run(
            string parameters = "",
            string workingDir = null,
            TimeSpan? timeout = null)
        {
            return RunProcess(
                parameters,
                workingDir,
                timeout == null ? int.MaxValue : (int)timeout.Value.TotalMilliseconds);
        }

        /// <summary>
        /// Launch programm in separate process
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="workingDir"></param>
        /// <param name="msecLockTimeout">if == 0, then parent process does not lock</param>
        /// <returns></returns>
        private CmdResult RunProcess(
            string parameters = "",
            string workingDir = null,
            int msecLockTimeout = 0)
        {
            CmdResult result = null;

            if (SystemProcessObject != null)
                throw new ProcessAlreadyStartedException();

            var process = new Process();
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = QuoteString(AppPath),
                    Arguments = parameters,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.GetEncoding(866),
                    StandardErrorEncoding = Encoding.GetEncoding(866)
                };
                if (workingDir != null)
                    startInfo.WorkingDirectory = workingDir;
                process.StartInfo = startInfo;

                OutputWaitHandle = new AutoResetEvent(msecLockTimeout == 0);
                ErrorWaitHandle = new AutoResetEvent(msecLockTimeout == 0);

                // Capture stdout data
                process.OutputDataReceived += this.OutputDataReceived;

                // Capture stderr data
                process.ErrorDataReceived += this.ErrorDataReceived;

                CurrentCommandLine = AppPath + " " + parameters;
                

                // Process process's execution end
                process.EnableRaisingEvents = true;
                process.Exited += this.Exited;
                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                SystemProcessObject = process;
            }
            catch (Exception ex)
            {
                process.Dispose();
                throw;
            }

            if (msecLockTimeout != 0)
            {
                if (SystemProcessObject.WaitForExit(msecLockTimeout) &&
                    OutputWaitHandle.WaitOne(msecLockTimeout) &&
                    ErrorWaitHandle.WaitOne(msecLockTimeout))
                {
                    int exitCode = SystemProcessObject.ExitCode;

                    //  Call WaitForExit() once more to ensure stdout and stderr are read to end (see Remarks section in msdn doc for this method)
                    SystemProcessObject.WaitForExit();
                    SystemProcessObject.Dispose();

                    result = new CmdResult
                    {
                        ExitCode = exitCode,
                        StandardError = StdError.ToString(),
                        StandardOutput = StdOut.ToString()
                    };
                    StdError.Clear();
                    StdOut.Clear();

                    SystemProcessObject = null;
                }
                else
                {
                    Kill();
                    throw new Exception("Process did not exit in a given timeout");
                }
            }

            return result;
        }


        /// <summary>
        /// Kill managed process, if there is running one
        /// </summary>
        /// <returns></returns>
        public virtual bool Kill()
        {
            if (IsRunning)
            {
                try
                {
                    CurrentCommandLine = "";
                    SystemProcessObject.Kill();
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    SystemProcessObject.Dispose();
                    SystemProcessObject = null;
                }
            }

            return true;
        }

        public bool WaitForExit(TimeSpan waitTimeout)
        {
            return SystemProcessObject.WaitForExit((int)waitTimeout.TotalMilliseconds);
        }

        /// <summary>
        /// Callback which will be asynchronously fired upon new line at stdout buffer
        /// </summary>
        public StandardBufferAppendedCallback OnStdoutLine { get; set; }

        /// <summary>
        /// Callback which will be asynchronously fired upon new line at stderr buffer
        /// </summary>
        public StandardBufferAppendedCallback OnStderrLine { get; set; }

        public bool IsRunning { get { return ProcessIsRunning(); } }

        private bool ProcessIsRunning()
        {
            bool res = SystemProcessObject != null;
            if (res)
            {
                try
                {
                    res &= SystemProcessObject.StartTime != default(DateTime);
                }
                catch { }
                try
                {
                    res &= !SystemProcessObject.HasExited;
                }
                catch { }
            }
            return res;
        }

        /// <summary>
        /// Callback which will be asynchronously fired upon application is exited
        /// </summary>
        public OnExitCallback OnExit { get; set; }

        /// <summary>
        /// Full path of target application
        /// </summary>
        public string AppPath { get; set; }

        /// <summary>
        /// Tells if executable has been successfully retrieved
        /// </summary>
        /// <returns></returns>
        public bool AppFound { get { return AppPath != null; } }

        private Process SystemProcessObject { get; set; }
        private OutputBuffer StdOut { get; set; }
        private OutputBuffer StdError { get; set; }

        /// <summary>
        /// Get command line of current process, if it is running
        /// </summary>
        public string CurrentCommandLine { get; private set; }

        private AutoResetEvent OutputWaitHandle = null;
        private AutoResetEvent ErrorWaitHandle = null;

        private void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                if (OnStdoutLine != null)
                    OnStdoutLine(e.Data);
                if (StdOut.LinesCount >= MAX_BUFFER_LINE_COUNT)
                    StdOut.PopFront();
                StdOut.PushBack(e.Data);
            }
            else
                OutputWaitHandle.Set();
        }

        private void ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                if (OnStderrLine != null)
                    OnStderrLine(e.Data);
                if (StdError.LinesCount >= MAX_BUFFER_LINE_COUNT)
                    StdError.PopFront();
                StdError.PushBack(e.Data);
            }
            else
                ErrorWaitHandle.Set();
        }

        private void Exited(object sender, EventArgs e)
        {
            if (SystemProcessObject != null)
            {
                OutputWaitHandle.Set();
                ErrorWaitHandle.Set();

                try
                {
                    int exitCode = SystemProcessObject.ExitCode;
                    DateTime exitTime = SystemProcessObject.ExitTime;
                    OnExit(string.Format("Exit code: {0}. Exit time: {1}.", exitCode, exitTime));
                }
                catch (Exception ex)
                {
                }
            }
        }

        #region IDisposable

        public virtual void Dispose()
        {
            Dispose(true);
        }

        ~AppRunner()
        {
            Dispose(false);
        }

        private bool disposed = false;

        private void Dispose(bool disposing)
        {
            if (disposed)
                return;

            Kill();
        }

        #endregion
    }

    public delegate void OnExitCallback(string message);

    /// <summary>
    /// Describes callback for event of new line in stdout/stderr
    /// </summary>
    /// <param name="line"></param>
    public delegate void StandardBufferAppendedCallback(string line);
}
