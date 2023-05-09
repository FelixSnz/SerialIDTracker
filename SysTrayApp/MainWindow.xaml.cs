using System;
using System.Windows;
using System.ServiceProcess;
using NLog;
using ILogger = NLog.ILogger;
using SysTrayApp.Services;

namespace SysTrayApp
{

    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private enum ServiceState
        {
            Stopped,
            Running,
            Paused
        }

        private ServiceController _serviceController;

        private ServiceState _currentState;

        public MainWindow()
        {
            InitializeComponent();
            Logger.Info("initializing ");
            NamedPipeServer server = new NamedPipeServer();

            server.DataReceived += OnDataReceived;
            server.Start();


            _serviceController = new ServiceController("IdTrackerService");
            UpdateServiceState();
            UpdateMenuItemsAvailability();
        }

        private void OnDataReceived(object sender, string data)
        {
            Console.WriteLine($"from systray: {data}");
        }


        private void UpdateServiceState()
        {
            _serviceController.Refresh();

            switch (_serviceController.Status)
            {
                case ServiceControllerStatus.Stopped:
                    _currentState = ServiceState.Stopped;
                    break;
                case ServiceControllerStatus.Running:
                    _currentState = ServiceState.Running;
                    break;
                case ServiceControllerStatus.Paused:
                    _currentState = ServiceState.Paused;
                    break;
            }
        }

        private void StartMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _serviceController.Start();
            _serviceController.WaitForStatus(ServiceControllerStatus.Running);
            UpdateServiceState();
            UpdateMenuItemsAvailability();
        }

        private void StopMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Stop your service
            _serviceController.Stop();
            _serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
            UpdateServiceState();
            UpdateMenuItemsAvailability();
        }

        private void PauseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _serviceController.Pause();
            _serviceController.WaitForStatus(ServiceControllerStatus.Paused);
            UpdateServiceState();
            UpdateMenuItemsAvailability();
        }

        private void ResumeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _serviceController.Continue();
            _serviceController.WaitForStatus(ServiceControllerStatus.Running);
            UpdateServiceState();
            UpdateMenuItemsAvailability();
        }

        private void RestartMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _serviceController.Stop();
            _serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
            _serviceController.Start();
            _serviceController.WaitForStatus(ServiceControllerStatus.Running);
            UpdateServiceState();
            UpdateMenuItemsAvailability();
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Close your application
            Application.Current.Shutdown();
        }


        private void ViewLogsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Show logs window or perform any desired action
        }

        private void HelpMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Show help window or perform any desired action
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Show about window or perform any desired action
        }

        private void UpdateMenuItemsAvailability()
        {
            StartMenuItem.IsEnabled = _currentState == ServiceState.Stopped;
            StopMenuItem.IsEnabled = _currentState == ServiceState.Running;
            PauseMenuItem.IsEnabled = _currentState == ServiceState.Running;
            ResumeMenuItem.IsEnabled = _currentState == ServiceState.Paused;
            RestartMenuItem.IsEnabled = _currentState == ServiceState.Running || _currentState == ServiceState.Paused;
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            if (WindowState == WindowState.Minimized)
            {
                this.Hide();
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.Hide();
            this.WindowState = WindowState.Minimized;
        }
    }
}
