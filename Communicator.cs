public class Communicator
{

    public static Communicator Instance { get; private set; }

    private LaserCommunicator laserCommunicator;
    private ServerCommunicator serverCommunicator;

    static void Main(string[] args)
    {
        Instance = new Communicator();
    }

    public Communicator()
    {
        laserCommunicator = new LaserCommunicator();
        serverCommunicator = new ServerCommunicator();
        while (true) { Thread.Sleep(100); }
        ;
    }

    public void ResetServer()
    {
        serverCommunicator = new ServerCommunicator();
    }

}