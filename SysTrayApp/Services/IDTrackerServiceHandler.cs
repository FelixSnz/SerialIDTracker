//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.ServiceProcess;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;

//namespace SysTrayApp.Services
//{
//    internal class IDTrackerServiceHandler
//    {

//        private enum ServiceState
//        {
//            Stopped,
//            Running,
//            Paused
//        }

//        private ServiceController _serviceController;

//        private ServiceState _currentState;

        


//        private void UpdateServiceState()
//        {
//            _serviceController.Refresh();

//            switch (_serviceController.Status)
//            {
//                case ServiceControllerStatus.Stopped:
//                    _currentState = ServiceState.Stopped;
//                    break;
//                case ServiceControllerStatus.Running:
//                    _currentState = ServiceState.Running;
//                    break;
//                case ServiceControllerStatus.Paused:
//                    _currentState = ServiceState.Paused;
//                    break;
//            }
//        }

//        private void StartMenuItem_Click(object sender, RoutedEventArgs e)
//        {
//            _serviceController.Start();
//            _serviceController.WaitForStatus(ServiceControllerStatus.Running);
//            UpdateServiceState();
//            UpdateMenuItemsAvailability();
//        }

//        private void StopMenuItem_Click(object sender, RoutedEventArgs e)
//        {
//            // Stop your service
//            _serviceController.Stop();
//            _serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
//            UpdateServiceState();
//            UpdateMenuItemsAvailability();
//        }

//        private void PauseMenuItem_Click(object sender, RoutedEventArgs e)
//        {
//            _serviceController.Pause();
//            _serviceController.WaitForStatus(ServiceControllerStatus.Paused);
//            UpdateServiceState();
//            UpdateMenuItemsAvailability();
//        }

//        private void ResumeMenuItem_Click(object sender, RoutedEventArgs e)
//        {
//            _serviceController.Continue();
//            _serviceController.WaitForStatus(ServiceControllerStatus.Running);
//            UpdateServiceState();
//            UpdateMenuItemsAvailability();
//        }

//        private void RestartMenuItem_Click(object sender, RoutedEventArgs e)
//        {
//            _serviceController.Stop();
//            _serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
//            _serviceController.Start();
//            _serviceController.WaitForStatus(ServiceControllerStatus.Running);
//            UpdateServiceState();
//            UpdateMenuItemsAvailability();
//        }

//        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
//        {
//            // Close your application
//            Application.Current.Shutdown();
//        }


//        private void ViewLogsMenuItem_Click(object sender, RoutedEventArgs e)
//        {
//            // Show logs window or perform any desired action
//        }

//        private void HelpMenuItem_Click(object sender, RoutedEventArgs e)
//        {
//            // Show help window or perform any desired action
//        }

//        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
//        {
//            // Show about window or perform any desired action
//        }

        
//    }
//}
