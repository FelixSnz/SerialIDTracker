using System;
using System.IO.Pipes;
using System.Text;

namespace SerialIDTracker.Services
{
    public class NamedPipeClient : IDisposable
    {
        private readonly NamedPipeClientStream client;

        public NamedPipeClient(string pipeName)
        {
            client = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.None);
            client.Connect();
        }

        public void Send(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            client.Write(data, 0, data.Length);
        }

        public string Receive()
        {
            byte[] buffer = new byte[1024];
            int bytesRead = client.Read(buffer, 0, buffer.Length);
            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            return message;
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}
