using System;
using Steamworks;

namespace ArenaLink.Steam
{
    public class SteamAuth
    {
        public SteamAuth()
        {
            if (SteamManager.Initialized) return;
            
            throw new InvalidOperationException("Steam is not initialized!");
        }

        public string GetAuthTicket()
        {
            var ticketData = new byte[1024];
            var steamNetworkingIdentity = new SteamNetworkingIdentity();

           SteamUser.GetAuthSessionTicket(ticketData, ticketData.Length, out var ticketSize, ref steamNetworkingIdentity);
            
            var trimmedTicket = new byte[ticketSize];
            Array.Copy(ticketData, trimmedTicket, ticketSize);
            
            var authTicketHex = BitConverter.ToString(trimmedTicket).Replace("-", string.Empty);
             
            return authTicketHex;
        }
    }
}