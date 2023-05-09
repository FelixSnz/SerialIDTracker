using System;
using System.IO.Pipes;
using System.Text;
using System.Windows;

class NamedPipeClient
{
    private const string PipeName = "SerialTrackerService";
    private const int BufferSize = 4096;

    public void Connect()
    {
        using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut))
        {
            try
            {
                Console.WriteLine("Connecting to named pipe server...");
                pipeClient.Connect(5000);
                Console.WriteLine("Connected to named pipe server.");

                string messageToSend = "Hello, Named Pipe Server!";
                SendMessage(pipeClient, messageToSend);
                Console.WriteLine("acabao");

                //uncomment when handshake implemented
                //string receivedMessage = ReceiveMessage(pipeClient);
                //Console.WriteLine($"Received message: {receivedMessage}");

            }
            catch (TimeoutException e)
            {
                MessageBox.Show($"error de espera, no se encontro servidor {PipeName}: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
                //Console.WriteLine();
            }

        }
    }

    private void SendMessage(NamedPipeClientStream pipeClient, string message)
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        pipeClient.Write(messageBytes, 0, messageBytes.Length);
        Console.WriteLine($"Sent message: {message}");
    }

    private string ReceiveMessage(NamedPipeClientStream pipeClient)
    {
        byte[] buffer = new byte[BufferSize];
        int bytesRead = pipeClient.Read(buffer, 0, buffer.Length);
        return Encoding.UTF8.GetString(buffer, 0, bytesRead);
    }
}