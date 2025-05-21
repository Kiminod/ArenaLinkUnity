using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ArenaLink.ArenaLink;
using ArenaLink.Models;
using ArenaLink.Models.Request;
using ArenaLink.Models.Responses;
using UnityEngine;

namespace ArenaLink.Networking.UDP
{
    public class UdpClientHandler
    {
        private static ArenaLinkService ArenaLinkService => ArenaLinkService.Instance;
        private UdpClient _client;
        private IPEndPoint _serverEndpoint;
        private bool _isRegisteredEndpoint;
        

        public void Start()
        {
            var hostEntry = Dns.GetHostEntry(ArenaLinkService.GetUdpServerAddress());
            var serverIP = hostEntry.AddressList[0];
            
            Debug.Log($"UDP => Server IP: {serverIP}");
            
            var localEndPoint = new IPEndPoint(IPAddress.Any, 0);
            _client = new UdpClient(localEndPoint);
            _serverEndpoint = new IPEndPoint(serverIP, ArenaLinkService.GetUdpServerPort());
            Task.Run(ReceiveLoop);
        }
        
        private async Task ReceiveLoop()
        {
            try
            {
                while (true)
                {
                    var result = await _client.ReceiveAsync();
                    HandleData((UdpOpCode)result.Buffer[0], result.Buffer[1..]);
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Udp client error: {ex.Message}");
            }
        }

        public async Task RegisterClient(string token)
        {
            while (!_isRegisteredEndpoint)
            {
                Debug.Log($"UDP => Sending register client request to server. Token: {token}");
                var data = Encoding.UTF8.GetBytes(token);
                await SendAsync(UdpOpCode.RegisterClient, data);
                await Task.Delay(1000); // Wait for 1 second before sending the next packet
            }
        }
        
        public async Task SendPlayerPosition(Vector3 position)
        {
            var playerActionUpdate = new PlayerActionUpdate
            {
                Position = new Vector3Position(position),
                TimeStamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
            };
            
            var json = JsonSerializer.Serialize(playerActionUpdate);
            var data = Encoding.UTF8.GetBytes(json);
            
            await SendAsync(UdpOpCode.UpdatePlayer, data);
        }
        
        public async Task SendAsync(UdpOpCode code, byte[] data)
        {
            var packet = new byte[data.Length + 1];
            packet[0] = (byte)code;
            Array.Copy(data, 0, packet, 1, data.Length);
            await _client.SendAsync(packet, packet.Length, _serverEndpoint);
        }
        

        public async Task SendString(string message)
        {
            var data = Encoding.UTF8.GetBytes(message);
            await SendAsync(UdpOpCode.DefaultMessage, data);
        }
        
        
        private void HandleData(UdpOpCode code, byte[] data)
        {
            switch (code)
            {
                case UdpOpCode.RegisterClientResponse:
                    OnClientRegistered();
                    break;
                case UdpOpCode.ReceiveMove:
                    OnReceivedUpdateFromServer(data);
                    break;
                    
            }
        }
        
        private void OnReceivedUpdateFromServer(byte[] data)
        {
            var json = Encoding.UTF8.GetString(data);
            var matchUpdate = JsonSerializer.Deserialize<MatchUpdate>(json);
            NetworkManager.Instance.MatchUpdate(matchUpdate);
        }
        
        private void OnClientRegistered()
        {
            _isRegisteredEndpoint = true;
        }
    }
}