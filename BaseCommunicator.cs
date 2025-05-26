using System.IO.Ports;
using System.Text;

public class BaseCommunicator
{

    public static BaseCommunicator? Instance { get; private set; }

    private LaserCommunicator laserCommunicator;
    private ServerCommunicator serverCommunicator;


    static void Main(string[] args)
    {
        new BaseCommunicator();
    }

    public BaseCommunicator()
    {
        Instance = this;
        laserCommunicator = new LaserCommunicator();
        serverCommunicator = new ServerCommunicator();
        Task.Run(ReceiveGPS);
        while (true)
        { }
    }

    public void ResetServer()
    {
        Thread.Sleep(1000);
        Console.WriteLine("Attempting reinitialization...");
        serverCommunicator = new ServerCommunicator();
    }

    public void ParsePacket(NetPacket packet)
    {

    }

    public void ReceiveGPS()
    {
        Console.WriteLine("Attempting GPS receive...");
        SerialPort port = new SerialPort("/dev/ttyS0", 9600);
        port.Open();
        if (port.IsOpen)
        {
            while (true)
            {
                byte[] buffer = new byte[1024];
                port.Read(buffer, 0, port.BytesToRead);

                string data = Encoding.ASCII.GetString(buffer);
                Console.WriteLine("Received: " + data);
            }
        }
    }

}
