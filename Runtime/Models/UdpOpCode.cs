namespace ArenaLink.Models
{
    public enum UdpOpCode : byte
    {
        DefaultMessage = 0x00,
        
        RegisterClient = 0x01,
        RegisterClientResponse = 0x02,
        
        UpdatePlayer = 0x10,
        
        ReceiveMove = 0x20,
        
        PlayerAction = 0x50,
    }
}