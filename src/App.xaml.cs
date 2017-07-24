using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using log4net;

namespace ClipboardManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            RegisterGlobalExceptionHandling(Log);
            var clipboardView = new Views.ClipboardView();
            clipboardView.Show();
        }

        private void RegisterGlobalExceptionHandling(ILog log)
        {
            AppDomain.CurrentDomain.UnhandledException +=
                (sender, args) => CurrentDomainOnUnhandledException(args);

            Application.Current.Dispatcher.UnhandledException +=
                (sender, args) => DispatcherOnUnhandledException(args);

            Application.Current.DispatcherUnhandledException +=
                (sender, args) => CurrentOnDispatcherUnhandledException(args);

            TaskScheduler.UnobservedTaskException +=
                (sender, args) => TaskSchedulerOnUnobservedTaskException(args);
        }

        private void TaskSchedulerOnUnobservedTaskException(UnobservedTaskExceptionEventArgs args)
        {
            LogError(args.Exception);
        }

        private void CurrentOnDispatcherUnhandledException(DispatcherUnhandledExceptionEventArgs args)
        {
            LogError(args.Exception);
        }

        private void DispatcherOnUnhandledException(DispatcherUnhandledExceptionEventArgs args)
        {
            LogError(args.Exception);
        }

        private void CurrentDomainOnUnhandledException(UnhandledExceptionEventArgs args)
        {
            var exception = args.ExceptionObject as Exception;
            LogError(exception);
        }

        private void LogError(Exception ex)
        {
            Log.Error("Domain wide exception", ex);
        }
        
    }
}
