using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using ILogger = NLog.ILogger;


namespace SerialIDTracker.Services
{
    internal class Producer
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        static NamedPipeClient namedPClient = new NamedPipeClient();
        
        public static  void OnDataScanned(object sender, string Data)
        {
            
            //Logger.Info(Data);
            namedPClient.SendData(Data);
        }
    }
}
