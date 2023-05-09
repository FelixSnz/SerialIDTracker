using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialIDTracker.Services
{
    internal class Producer
    {

        public static  void OnDataScanned(object sender, string Data)
        {
            NamedPipeClient namedPClient = new NamedPipeClient("SerialTrackerService");
            namedPClient.Send(Data);
        }
    }
}
