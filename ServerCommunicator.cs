using System.Diagnostics;
using System.Net.Sockets;

public class ServerCommunicator
{

    private const string serverIP = "localhost";
    private DateTime lastCommunicationTime;

    public ServerCommunicator()
    {
        CreateConnection();
    }

    public void CreateConnection()
    {
        Task t = new Task(ServerConnection);
        t.Start();
    }

    public void ServerConnection()
    {
        Socket client = null;

        while (client == null || !client.Connected)
        {
            client = new Socket(SocketType.Stream, ProtocolType.Tcp);
            client.ReceiveTimeout = 1;

            IAsyncResult result = client.BeginConnect(serverIP, 7777, null, null);
            result.AsyncWaitHandle.WaitOne(5000);
            Console.WriteLine(client.Connected ? "Connected to server." : "Unable to connect to server.");
            if (!client.Connected)
                client.Close();
        }

        lastCommunicationTime = DateTime.Now;
        while ((DateTime.Now - lastCommunicationTime).Seconds < 4)
        {
            if (client.Poll(10000, SelectMode.SelectRead))
            {
                byte[] data = new byte[1024];
                int receivedByteCount = client.Receive(data);
                lastCommunicationTime = DateTime.Now;

                NetPacket dataPacket = new NetPacket(data.ToList().GetRange(0, receivedByteCount).ToArray());
                Console.WriteLine("Received: " + (Route)dataPacket.ReadByte());
            }
        }

        Console.WriteLine("Disconnected from server.");
        client.Close();

        // Attempt new connection
        CreateConnection();

    }

}