﻿using UPSBatteryController.Controllers.Application;
using GroupAdr.Library.AsyncEvents;
using GroupAdr.Logger;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Threading;

namespace UPSBatteryController.Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Fields

        private AggregateCatalog _catalog;
        private CompositionContainer _container;
        private IApplicationController _controller;
        private ILogger _logger;
        private ThreadEventSource _defaultEventSource;
        private static readonly Semaphore _singleInstanceWatcher;
        private static readonly bool _createdNew;

        #endregion

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            LogFactory.LoggerCreator = new NLogCreator();
            _logger = LogFactory.GetLogger();

            DispatcherUnhandledException += AppDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;

            _catalog = new AggregateCatalog();
            // Add the WpfApplicationFramework assembly to the catalog
            _catalog.Catalogs.Add(new AssemblyCatalog(typeof(ViewModel).Assembly));
            // Add the Writer.Presentation assembly to the catalog
            _catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
            // Add the Writer.Applications assembly to the catalog
            _catalog.Catalogs.Add(new AssemblyCatalog(typeof(IApplicationController).Assembly));

            _container = new CompositionContainer(_catalog, CompositionOptions.DisableSilentRejection);
            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue(_container);
            _container.Compose(batch);

            //Настройка источника событий
            _defaultEventSource = new ThreadEventSource(TimeSpan.FromSeconds(1));
            _container.ComposeExportedValue<IAsyncEventSource>(_defaultEventSource);
            _container.ComposeExportedValue<Func<DateTime>>(() => DateTime.Now);

            _controller = _container.GetExportedValue<IApplicationController>();
            _controller.Run();
            _controller.ExitRequested += (sender, args) => Shutdown();
        }

        private void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            HandleException(e.Exception, false);
        }

        private void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException(e.ExceptionObject as Exception, e.IsTerminating);
        }

        private void HandleException(Exception e, bool isTerminating)
        {
            if (e == null) { return; }

            Trace.TraceError(e.ToString());

            if (!isTerminating)
            {
                _logger.LogUnhandledException(e);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _controller.Shutdown();
            _container.Dispose();
            _catalog.Dispose();

            base.OnExit(e);
        }

        static App()
        {
            // Ensure other instances of this application are not running.
            _singleInstanceWatcher = new Semaphore(
                0, // Initial count.
                1, // Maximum count.
                Assembly.GetExecutingAssembly().GetName().Name,
                out _createdNew);

            if (_createdNew)
            {
                // This thread created the kernel object so no other instance
                // of this application must be running.
            }
            else
            {
                // This thread opened an existing kernel object with the same
                // string name; another instance of this app must be running now.

                // Gets a new System.Diagnostics.Process component and the
                // associates it with currently active process.
                Process current = Process.GetCurrentProcess();
                String processName = current.ProcessName;

                // Enumerate through all the process resources on the share
                // local computer that the specified process name.
                foreach (Process process in
                     Process.GetProcessesByName(processName))
                {
                    if (process.Id != current.Id)
                    {
                        if (process.MainWindowHandle != IntPtr.Zero)
                        {
                            NativeMethods.SetForegroundWindow(process.MainWindowHandle);
                            NativeMethods.ShowWindow(process.MainWindowHandle,
                                WindowShowStyle.Restore);
                        }
                        break;
                    }
                }

                // Terminate this process and gives the underlying operating 
                // system the specified exit code.
                Environment.Exit(-2);
            }
        }

        private static class NativeMethods
        {
            /// <summary>
            /// Brings the thread that created the specified window into the
            /// foreground and activates the window. Keyboard input is directed
            /// to the window, and various visual cues are changed for the user.
            /// The system assigns a slightly higher priority to the thread that
            /// created the foreground window than it does to other threads.
            /// </summary>
            /// <param name="hWnd">A handle to the window that should be
            /// activated and brought to the foreground.
            /// </param>
            /// <returns>If the window was brought to the foreground, the
            /// return value is nonzero. </returns>
            [DllImport("user32.dll")]
            internal static extern bool SetForegroundWindow(IntPtr hWnd);

            /// <summary>Shows a Window</summary>
            /// <remarks>
            /// <para>To perform certain special effects when showing or hiding a
            /// window, use AnimateWindow.</para>
            /// <para>The first time an application calls ShowWindow, it should use
            /// the WinMain function's nCmdShow parameter as its nCmdShow ..
            /// Subsequent calls to ShowWindow must use one of the values in the
            /// given list, instead of the one specified by the WinMain function's
            /// nCmdShow parameter.</para>
            /// <para>As noted in the discussion of the nCmdShow parameter, the
            /// nCmdShow value is ignored in the first call to ShowWindow if the
            /// program that launched the application specifies startup information
            /// in the structure. In this case, ShowWindow uses the information
            /// specified in the STARTUPINFO structure to show the window. On
            /// subsequent calls, the application must call ShowWindow with ..
            /// set to SW_SHOWDEFAULT to use the startup information provided by ..
            /// program that launched the application. This behavior is designed ..
            /// the following situations: </para>
            /// <list type="">
            ///    <item>Applications create their main window by calling ..
            ///    with the WS_VISIBLE flag set. </item>
            ///    <item>Applications create their main window by calling ..
            ///    with the WS_VISIBLE flag cleared, and later call ShowWindow ..
            ///    SW_SHOW flag set to make it visible.</item>
            /// </list></remarks>
            /// <param name="hWnd">Handle to the window.</param>
            /// <param name="nCmdShow">Specifies how the window is to be shown.
            /// This parameter is ignored the first time an application calls
            /// ShowWindow, if the program that launched the application provides a
            /// STARTUPINFO structure. Otherwise, the first time ShowWindow .. ,
            /// the value should be the value obtained by the WinMain function ..
            /// nCmdShow parameter. In subsequent calls, this parameter ..
            /// the WindowShowStyle members.</param>
            /// <returns>
            /// If the window was previously visible, the return value is nonzero.
            /// If the window was previously hidden, the return value is zero.
            /// </returns>
            [DllImport("user32.dll")]
            internal static extern bool ShowWindow(IntPtr hWnd,
                WindowShowStyle nCmdShow);
        }

        /// <summary>
        /// Enumeration of the different ways of showing a window.</summary>
        internal enum WindowShowStyle : uint
        {
            /// <summary>Hides the window and activates another window.</summary>
            /// <remarks>See SW_HIDE</remarks>
            Hide = 0,
            /// <summary>Activates and displays a window. If the window ..
            /// or maximized, the system restores it to its original size and
            /// position. An application should specify this flag when displaying
            /// the window for the first time.</summary>
            /// <remarks>See SW_SHOWNORMAL</remarks>
            ShowNormal = 1,
            /// <summary>Activates the window and displays it ..</summary>
            /// <remarks>See SW_SHOWMINIMIZED</remarks>
            ShowMinimized = 2,
            /// <summary>Activates the window and displays it ..</summary>
            /// <remarks>See SW_SHOWMAXIMIZED</remarks>
            ShowMaximized = 3,
            /// <summary>Maximizes the specified window.</summary>
            /// <remarks>See SW_MAXIMIZE</remarks>
            Maximize = 3,
            /// <summary>Displays a window in its most recent size and position.
            /// This value is similar to "ShowNormal", except the window is not
            /// actived.</summary>
            /// <remarks>See SW_SHOWNOACTIVATE</remarks>
            ShowNormalNoActivate = 4,
            /// <summary>Activates the window and displays it in its current size
            /// and position.</summary>
            /// <remarks>See SW_SHOW</remarks>
            Show = 5,
            /// <summary>Minimizes the specified window and activates the next
            /// top-level window in the Z order.</summary>
            /// <remarks>See SW_MINIMIZE</remarks>
            Minimize = 6,
            /// <summary>Displays the window as a minimized window. This value is
            /// similar to "ShowMinimized", except the window ..</summary>
            /// <remarks>See SW_SHOWMINNOACTIVE</remarks>
            ShowMinNoActivate = 7,
            /// <summary>Displays the window in its current size and position. This
            /// value is similar to "Show", except the window ..</summary>
            /// <remarks>See SW_SHOWNA</remarks>
            ShowNoActivate = 8,
            /// <summary>Activates and displays the window. If the window is
            /// minimized or maximized, the system restores it to its original size
            /// and position. An application should specify this flag ..
            /// a minimized window.</summary>
            /// <remarks>See SW_RESTORE</remarks>
            Restore = 9,
            /// <summary>Sets the show state based on the SW_ value specified ..
            /// STARTUPINFO structure passed to the CreateProcess function by the
            /// program that started the application.</summary>
            /// <remarks>See SW_SHOWDEFAULT</remarks>
            ShowDefault = 10,
            /// <summary>Windows 2000/XP: Minimizes a window, even if the thread
            /// that owns the window is hung. This flag should only be used when
            /// minimizing windows from a different thread.</summary>
            /// <remarks>See SW_FORCEMINIMIZE</remarks>
            ForceMinimized = 11
        }
    }
}
