using System;
using System.IO.Pipes;
using System.Text;
using System.Threading;

namespace SerialIDTracker.Services
{




    class NamedPipeServer
    {
        private NamedPipeServerStream _pipeServer;
        private const string PipeName = "SerialTrackerService";
        private const int BufferSize = 4096;

        public event EventHandler<string> DataReceived;

        public NamedPipeServer()
        {
            Initialize();
        }

        private void Initialize()
        {
            _pipeServer = new NamedPipeServerStream(PipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
        }

        public void Start()
        {
            _pipeServer.BeginWaitForConnection(OnClientConnected, null);
        }

        private void OnClientConnected(IAsyncResult asyncResult)
        {
            _pipeServer.EndWaitForConnection(asyncResult);

            byte[] buffer = new byte[BufferSize];
            _pipeServer.BeginRead(buffer, 0, buffer.Length, OnDataReceived, buffer);
        }

        private void OnDataReceived(IAsyncResult asyncResult)
        {
            int bytesRead = _pipeServer.EndRead(asyncResult);

            if (bytesRead > 0)
            {
                byte[] buffer = asyncResult.AsyncState as byte[];
                string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                DataReceived?.Invoke(this, receivedData);

                _pipeServer.BeginRead(buffer, 0, buffer.Length, OnDataReceived, buffer);
            }
        }

        public void SendData(string data)
        {
            if (_pipeServer.IsConnected)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                _pipeServer.Write(buffer, 0, buffer.Length);
            }
        }

        public void Dispose()
        {
            _pipeServer?.Dispose();
        }
    }


}
