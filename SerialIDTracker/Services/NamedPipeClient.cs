using System;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace SerialIDTracker.Services
{
    internal class NamedPipeClient
    {
        private NamedPipeClientStream _pipeClient;
        private const string PipeName = "SerialTrackerService";
        private const int BufferSize = 4096;

        public event EventHandler<string> DataReceived;

        public NamedPipeClient()
        {
            Initialize();
        }

        private void Initialize()
        {
            _pipeClient = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
            _pipeClient.Connect();
        }

        public void Connect()
        {
             _pipeClient.Connect(5000);
            //StartListening();
        }

        private void StartListening()
        {
            byte[] buffer = new byte[BufferSize];
            _pipeClient.BeginRead(buffer, 0, buffer.Length, OnDataReceived, buffer);
        }

        private void OnDataReceived(IAsyncResult asyncResult)
        {
            int bytesRead = _pipeClient.EndRead(asyncResult);

            if (bytesRead > 0)
            {
                byte[] buffer = asyncResult.AsyncState as byte[];
                string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                DataReceived?.Invoke(this, receivedData);

                _pipeClient.BeginRead(buffer, 0, buffer.Length, OnDataReceived, buffer);
            }
        }

        public void SendData(string data)
        {
            if (_pipeClient.IsConnected)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                _pipeClient.Write(buffer, 0, buffer.Length);
            }
        }

        public void Dispose()
        {
            _pipeClient?.Dispose();
        }
    }
}