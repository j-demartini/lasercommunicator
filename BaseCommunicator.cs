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
        //serverCommunicator = new ServerCommunicator();
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
        SerialPort port = new SerialPort("/dev/ttyS0", 9600, Parity.None, 8, StopBits.One)
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
                try
                {
                    string line = port.ReadLine();
                    Console.WriteLine(line); // You should see $GPRMC, $GPGGA etc.
                }
                catch (TimeoutException)
                {
                    // Do nothing, just wait for next line
                }

                Thread.Sleep(10); // Prevent CPU overuse
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

}
