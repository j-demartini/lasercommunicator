using System.IO.Ports;
using System.Runtime.CompilerServices;

public class LaserCommunicator
{

    public static LaserCommunicator? Instance { get; private set; }

    private SerialPort serialPort;
    private bool activated = true;

    public LaserCommunicator()
    {
        Instance = this;
        serialPort = new SerialPort("COM3", 115200, Parity.None);
        serialPort.Open();
        Task.Run(MonitorSerial);
    }

    public void MonitorSerial()
    {
        while (activated)
        {
            if (serialPort.IsOpen)
            {
                int b = serialPort.ReadByte();
                if(b == 254)
                Console.WriteLine("Read: " + b);
            }
        }
    }

    public void ParsePacket(NetPacket packet)
    {
        // Send it to the laser to parse
        if (serialPort.IsOpen)
        {
            // Insert length at start
            packet.InsertAtStart((byte)packet.ByteArray.Length);
            Console.WriteLine("Sending: " + GetByteString(packet.ByteArray));
            serialPort.Write(packet.ByteArray, 0, packet.ByteArray.Length);
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