using SerialIDTracker.Services;
using SerialIDTracker.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cognex.DataMan.SDK;
using System.ComponentModel;
using System.Configuration.Install;

namespace SerialIDTracker
{
    public partial class IdTrackerService : ServiceBase
    {

        public IdTrackerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                SynchronizationContext _syncContext = SynchronizationContext.Current ?? new SynchronizationContext();

                
                CognexScanner cognexScanner = new CognexScanner(_syncContext);


                cognexScanner.DataScanned += Producer.OnDataScanned;
                

                
                if (args.Length > 0)
                {
                    // If an IP address is provided as an argument, connect to that IP address.
                    string ipAddress = args[0];
                    int defaultPort = 23; // Replace 23 with the default port number for your device.
                    Console.WriteLine($"Connecting to the specified IP address: {ipAddress}:{defaultPort}");

                    cognexScanner.ConnectByIp(ipAddress, defaultPort);
                }
                else
                {
                    // If an IP address is not provided, use the existing discovery mechanism.
                    cognexScanner.DiscoverDevice();
                    Console.WriteLine("Discovering systems. Press any key to exit...");
                }

                Console.ReadKey();
            }

            finally
            {
                Singleton.Instance.IsClosing = true;


                if (Singleton.Instance.System != null && Singleton.Instance.System.State == ConnectionState.Connected)
                    Singleton.Instance.System.Disconnect();

            }

        }

        protected override void OnStop()
        {
            this.Stop();
        }
    }
}
