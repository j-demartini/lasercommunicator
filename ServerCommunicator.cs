using System.Diagnostics;
using System.Net.Sockets;

public class ServerCommunicator
{

    private const string serverIP = "localhost";

    private bool activated = true;
    private DateTime lastCommunicationTime;
    private Socket server;
    private Queue<NetPacket> packetQueue = new Queue<NetPacket>();

    public ServerCommunicator()
    {
        CreateConnection();
    }

    public void CreateConnection()
    {
        Task t = new Task(ServerConnection);
        t.Start();
    }

    public void EnqueuePacket(NetPacket packet)
    {
        packetQueue.Enqueue(packet);
    }

    public void ServerConnection()
    {
        server = null;

        Console.WriteLine("Initializing server.");

        while (server == null || !server.Connected)
        {
            server = new Socket(SocketType.Stream, ProtocolType.Tcp);
            server.ReceiveTimeout = 1;

            IAsyncResult result = server.BeginConnect(serverIP, 7777, null, null);
            result.AsyncWaitHandle.WaitOne(5000);
            Console.WriteLine(server.Connected ? "Connected to server." : "Unable to connect to server.");
            if (!server.Connected)
                server.Close();
        }

        lastCommunicationTime = DateTime.Now;
        Task.Run(ServerSend);
        Task.Run(ServerReceive);
        Task.Run(Heartbeat);

    }

    public void ServerSend()
    {
        while (activated)
        {
            while (packetQueue.Count > 0)
            {
                server.Send(packetQueue.Dequeue().ByteArray);
            }
        }
    }

    public void ServerReceive()
    {
        while (activated)
        {
            if ((DateTime.Now - lastCommunicationTime).Seconds < 4)
            {
                if (server.Poll(10000, SelectMode.SelectRead))
                {
                    byte[] data = new byte[1024];
                    int receivedByteCount = server.Receive(data);
                    lastCommunicationTime = DateTime.Now;

                    NetPacket dataPacket = new NetPacket(data.ToList().GetRange(0, receivedByteCount).ToArray());
                    switch ((Route)dataPacket.ReadByte())
                    {
                        case Route.COMMUNICATOR:
                            BaseCommunicator.Instance?.ParsePacket(dataPacket);
                            break;
                        case Route.LASER:
                            LaserCommunicator.Instance?.ParsePacket(dataPacket);
                            break;
                    }
                }
            }
            else
            {
                Close();
            }

        }
    }

    public void Close()
    {
        Console.WriteLine("Disconnected from the server.");
        activated = false;
        server.Close();
        BaseCommunicator.Instance?.ResetServer();
    }

    public void Heartbeat()
    {
        while (activated)
        {
            EnqueuePacket(PacketBuilder.Heartbeat());
            Thread.Sleep(1000);
        }
    }

    public void ParsePacket(NetPacket packet)
    {
        // Send it to the server to parse
        EnqueuePacket(packet);
    }


}