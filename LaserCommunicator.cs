public class LaserCommunicator
{

    public static LaserCommunicator? Instance { get; private set; }

    public LaserCommunicator()
    {
        Instance = this;
    }

    public void ParsePacket(NetPacket packet)
    {
        // Send it to the laser to parse
        
    }

}