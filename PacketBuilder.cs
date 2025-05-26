public class PacketBuilder
{
    public static NetPacket Heartbeat()
    {
        NetPacket p = new NetPacket();
        p.Write((byte)Route.COMMUNICATOR);
        p.Write((byte)PacketType.COMPLETE);
        p.Write((byte)MessageType.HEARTBEAT);
        return p;
    }
}