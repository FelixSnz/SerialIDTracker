using Cognex.DataMan.SDK;
using Cognex.DataMan.SDK.Discovery;
using Cognex.DataMan.SDK.Utils;
//using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SerialIDTracker.Utils;

namespace SerialIDTracker.Models
{
    internal class CognexScanner
    {

        private static EthSystemDiscoverer _ethSystemDiscoverer = null;
        private static SerSystemDiscoverer _serSystemDiscoverer = null;

        private static ResultCollector _results;

        private static SynchronizationContext _syncContext = null;

        public event EventHandler<string> DataScanned;

        //public static readonly ILogger Logger = LogManager.GetCurrentClassLogger();



        public CognexScanner(SynchronizationContext syncContext)
        {
            _syncContext = syncContext;

        }



        public void ConnectByIp(string ipAddress, int port)
        {

            Singleton.Instance.Connector = new EthSystemConnector(IPAddress.Parse(ipAddress), port);
            (Singleton.Instance.Connector as EthSystemConnector).UserName = "admin";
            (Singleton.Instance.Connector as EthSystemConnector).Password = ""; // Set the password here.

            InitializeSystem();
        }

        private void ConnectByDiscoveredSystem(object systemInfo)
        {
            if (systemInfo is EthSystemDiscoverer.SystemInfo ethSystemInfo)
            {
                Singleton.Instance.Connector = new EthSystemConnector(ethSystemInfo.IPAddress, ethSystemInfo.Port);
                (Singleton.Instance.Connector as EthSystemConnector).UserName = "admin";
                (Singleton.Instance.Connector as EthSystemConnector).Password = ""; // Set the password here.
            }
            else if (systemInfo is SerSystemDiscoverer.SystemInfo serSystemInfo)
            {
                Singleton.Instance.Connector = new SerSystemConnector(serSystemInfo.PortName, serSystemInfo.Baudrate);
            }
            InitializeSystem();


        }

        public void DiscoverDevice()
        {
            _ethSystemDiscoverer = new EthSystemDiscoverer();
            _serSystemDiscoverer = new SerSystemDiscoverer();

            _ethSystemDiscoverer.SystemDiscovered += OnEthSystemDiscovered;
            _serSystemDiscoverer.SystemDiscovered += OnSerSystemDiscovered;

            _ethSystemDiscoverer.Discover();
            _serSystemDiscoverer.Discover();

        }



        private void OnEthSystemDiscovered(EthSystemDiscoverer.SystemInfo systemInfo)
        {
            _syncContext.Post(delegate
            {
                Console.WriteLine($"Ethernet system discovered: {systemInfo.IPAddress}:{systemInfo.Port}");
                ConnectByDiscoveredSystem(systemInfo);
            }, null);
        }

        private void OnSerSystemDiscovered(SerSystemDiscoverer.SystemInfo systemInfo)
        {
            _syncContext.Post(delegate
            {
                Console.WriteLine($"Serial system discovered: {systemInfo.PortName}");
                ConnectByDiscoveredSystem(systemInfo);
            }, null);
        }

        public void InitializeSystem()
        {
            if (Singleton.Instance.IsClosing || Singleton.Instance.System != null || Singleton.Instance.Connector == null)
                return;

            Singleton.Instance.System = new DataManSystem(Singleton.Instance.Connector);
            Singleton.Instance.System.DefaultTimeout = 5000;

            Singleton.Instance.System.SystemConnected += OnSystemConnected;
            Singleton.Instance.System.SystemDisconnected += OnSystemDisconnected;
            Singleton.Instance.System.AutomaticResponseArrived += AutomaticResponseArrived;

            ResultTypes requestedResultTypes = ResultTypes.ReadXml;
            _results = new ResultCollector(Singleton.Instance.System, requestedResultTypes);
            _results.ComplexResultCompleted += Results_ComplexResultCompleted;
            _results.SimpleResultDropped += Results_SimpleResultDropped;

            Singleton.Instance.System.SetKeepAliveOptions(false, 3000, 1000);

            Singleton.Instance.System.Connect();

            try
            {
                Singleton.Instance.System.SetResultTypes(requestedResultTypes);
            }
            catch
            { }
        }

        private static void OnSystemConnected(object sender, EventArgs e)
        {
            _syncContext.Post(delegate
            {
                Console.WriteLine("System connected");
            }, null);
        }

        private static void OnSystemDisconnected(object sender, EventArgs e)
        {
            _syncContext.Post(delegate
            {
                Console.WriteLine("System disconnected. Reason: " + e.ToString());
                CleanupConnection();
                Console.ReadKey();
            }, null);
        }

        private static void AutomaticResponseArrived(object sender, AutomaticResponseArrivedEventArgs e)
        {
            _syncContext.Post(delegate
            {
                Console.WriteLine("Automatic response: " + e.Data.ToString());
            }, null);
        }

        private void Results_ComplexResultCompleted(object sender, ComplexResult e)
        {
            _syncContext.Post(delegate
            {
                AbstractResults(e);
            }, null);
        }

        private static void Results_SimpleResultDropped(object sender, SimpleResult e)
        {
            _syncContext.Post(delegate
            {
                Console.WriteLine("Partial result dropped: {0}, id={1}", e.Id.Type.ToString(), e.Id.Id);
            }, null);
        }

        private void AbstractResults(ComplexResult complexResult)
        {
            List<string> imageGraphics = new List<string>();
            string readResult = null;
            int resultId = -1;
            ResultTypes collectedResults = ResultTypes.None;

            // Take a reference or copy values from the locked result info object. This is done
            // so that the lock is used only for a short period of time.

            foreach (var simpleResult in complexResult.SimpleResults)
            {
                collectedResults |= simpleResult.Id.Type;

                switch (simpleResult.Id.Type)
                {
                    case ResultTypes.ImageGraphics:
                        imageGraphics.Add(simpleResult.GetDataAsString());
                        break;
                    case ResultTypes.ReadXml:
                        readResult = XmlFiles.GetString(simpleResult.GetDataAsString());
                        break;

                    case ResultTypes.ReadString:
                        readResult = simpleResult.GetDataAsString();

                        break;
                }
                resultId = simpleResult.Id.Id;


            }

            if (readResult != null)
            {
                DataScanned?.Invoke(this, readResult);
                Console.WriteLine($"Complex result arrived: resultId = {resultId}, read result = {readResult}");
                Console.WriteLine($"Complex result contains {collectedResults.ToString()}");
            }
        }



        private static void CleanupConnection()
        {
            Console.WriteLine("cleaning conn...");
            _ethSystemDiscoverer.Dispose();
            _ethSystemDiscoverer = null;

            _serSystemDiscoverer.Dispose();
            _serSystemDiscoverer = null;

            if (Singleton.Instance.System != null)
            {
                Singleton.Instance.System.Dispose();
                Singleton.Instance.System.Disconnect();
                Singleton.Instance.System = null;
            }

            if (Singleton.Instance.Connector != null)
            {
                Singleton.Instance.Connector.Disconnect();
                Singleton.Instance.Connector = null;
            }
        }
    }
}
