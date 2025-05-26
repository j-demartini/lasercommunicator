using System.IO.Ports;
using System.Runtime.CompilerServices;

public class LaserCommunicator
{

    public static LaserCommunicator? Instance { get; private set; }

    private SerialPort serialPort;
    private bool activated = true;
    private bool clear = true;
    private int totalBytesSent;
    private Queue<NetPacket> toSend = new Queue<NetPacket>(); 

    public LaserCommunicator()
    {
        Instance = this;
        serialPort = new SerialPort("COM3", 115200, Parity.None);
        serialPort.Open();
        Task.Run(SendSerial);
        Task.Run(MonitorSerial);
    }

    public void MonitorSerial()
    {
        while (activated)
        {
            if (serialPort.IsOpen)
            {
                int b = serialPort.ReadByte();
                if (b == 254)
                {
                    clear = true;
                }
                if (b == 253)
                {
                    clear = true;
                }
            }

        }
    }

    public void SendSerial()
    {
        while (activated)
        {
            if (serialPort.IsOpen)
            {
                while (toSend.Count > 0 && clear)
                {
                    NetPacket p = toSend.Dequeue();
                    clear = false;
                    serialPort.Write(p.ByteArray, 0, p.ByteArray.Length);
                    totalBytesSent += p.ByteArray.Length;
                }
            }
        }
    }

    public void ParsePacket(NetPacket packet)
    {
        // Send it to the laser to parse
        if (serialPort.IsOpen)
        {
            packet.InsertAtStart((byte)packet.ByteArray.Length);
            NetPacket[] splitPacket = packet.Split();
            foreach (NetPacket p in splitPacket)
                toSend.Enqueue(p);
        }
        else
        {
            Console.WriteLine("Port not open!");
        }
    }

    public string GetByteString(byte[] data)
    {
        string s = "";
        foreach (byte b in data)
        {
            s += b + " ";
        }
        return s;
    }

}