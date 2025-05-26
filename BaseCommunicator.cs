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
        //laserCommunicator = new LaserCommunicator();
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
        SerialPort port = new SerialPort("/dev/ttyAMA0", 9600, Parity.None, 8, StopBits.One)
        {
            Encoding = Encoding.ASCII,
            ReadTimeout = 1000
        };

        try
        {
            port.Open();
            Console.WriteLine("GPS port opened.");

            while (true)
            {
                string line = port.ReadLine();
                if (line.Contains("GPGLL"))
                    ParseGPS(line);
                Thread.Sleep(10);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            if (port.IsOpen)
                port.Close();
        }
    }

    public void ParseGPS(string line)
    {
        string[] split = line.Split(',');
        string latitudeDDM = split[1];
        string longitudeDDM = split[3];

        int latDecimal = latitudeDDM.IndexOf('.');
        float latMinutes = float.Parse(latitudeDDM.Substring(latDecimal - 2));
        float latDegrees = float.Parse(latitudeDDM.Substring(0, latDecimal - 2));
        latDegrees += latMinutes / 60f;

        latDegrees = split[2].Equals("N") ? latDegrees : -latDegrees;

        int lonDecimal = longitudeDDM.IndexOf('.');
        float lonMinutes = float.Parse(longitudeDDM.Substring(lonDecimal - 2));
        float lonDegrees = float.Parse(longitudeDDM.Substring(0, lonDecimal - 2));
        lonDegrees += lonMinutes / 60f;

        lonDegrees = split[4].Equals("E") ? lonDegrees : -lonDegrees;


        NetPacket p = new NetPacket();
        p.Write((byte)Route.SERVER);
        p.Write((byte)PacketType.COMPLETE);
        p.Write((byte)MessageType.GPS);
        p.Write(latDegrees);
        p.Write(lonDegrees);
        serverCommunicator.EnqueuePacket(p);

        Console.WriteLine("GPS Received: " + latDegrees + " " + lonDegrees);

    }

}
