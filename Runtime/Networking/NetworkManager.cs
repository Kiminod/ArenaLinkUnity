using System;
using System.Text.Json;
using System.Threading.Tasks;
using ArenaLink.Models.Responses;
using ArenaLink.Networking.Auth;
using ArenaLink.Networking.SignalR;
using ArenaLink.Networking.UDP;
using ArenaLink.Steam;
using UnityEngine;

namespace ArenaLink.Networking
{
    public partial class NetworkManager : MonoBehaviour
    {
        public static NetworkManager Instance { get; private set; }
        
        private AuthClient Auth { get; set; }
        private SignalRClient SignalR { get; set; }
        private UdpClientHandler UdpClient { get; set; }

        private static SteamManager SteamManager => SteamManager.Instance;
        private string _authToken;
        
        private async void Awake()
        {
            try
            {
                if (Instance != null)
                {
                    Destroy(gameObject);
                    return;
                }
    
                Instance = this;
                DontDestroyOnLoad(gameObject);

                // Initialize services here
                Auth = new AuthClient();
                SignalR = gameObject.AddComponent<SignalRClient>();
                UdpClient = new UdpClientHandler(); // Initialize UDP client
                
                if (!SteamManager.Initialized)
                {
                    SteamManager.OnInit += () => _ = Login();
                }
                else
                {
                    await Login();
                }
                
                
                // ====== Test section ======
                
                UdpClient.Start();
                await UdpClient.RegisterClient(_authToken);
                
                await Task.Delay(1000);
                    
                
                // ===========================
            }
            catch (Exception e)
            {
                Debug.LogException(new Exception("NetworkManager initialization failed", e));
            }
        }
        
        private async Task Login()
        {
            var ticket = SteamManager.GetAuthTicket();

            var count = 1;
            await Task.Delay(500);
            while (count <= 5)
            {
                var response = await Auth.Login(ticket);
                count++;
                
                if (string.IsNullOrEmpty(response)) continue;
                
                try
                {
                    _authToken = JsonSerializer.Deserialize<LoginResponse>(response).Token;
                    break;
                }
                catch (Exception e)
                {
                    Debug.LogException(new Exception($"Response: {response}", e));
                }
                
                await Task.Delay(500);
            }
            
            if (count == 6) throw new TimeoutException("NetworkManager login timed out");
            
            await SignalR.ConnectAsync(_authToken);
        }
        
        public async Task SendPlayerPosition(Vector3 position)
        {
            await UdpClient.SendPlayerPosition(position);
        }
        
        public async Task CreateLobby()
        {
            await SignalR.CreateLobby();
        }
        
        public async Task FindMatch()
        {
            await SignalR.FindMatch();
        }
        
        public async Task AcceptMatch(int matchId)
        {
            await SignalR.AcceptMatch(matchId);
        }
    }
}