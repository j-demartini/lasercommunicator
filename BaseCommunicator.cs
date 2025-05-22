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
        while (true)
        {}
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

}