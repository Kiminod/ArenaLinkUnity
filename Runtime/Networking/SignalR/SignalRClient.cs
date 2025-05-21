using System;
using System.Threading.Tasks;
using ArenaLink.ArenaLink;
using ArenaLink.ArenaLink.MainThreadDispatcher;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine;

namespace ArenaLink.Networking.SignalR
{
    public class SignalRClient : MonoBehaviour
    {
        private static SignalRClient Instance { get; set; }
        private HubConnection _connection;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private async void OnDestroy()
        {
            try
            {
                await DisconnectAsync();
            }
            catch (Exception e)
            {
                Debug.LogError($"SignalR => Error disconnecting: {e}");
            }
        }
        
        private async Task DisconnectAsync()
        {
            if (_connection != null)
            {
                try
                {
                    await _connection.StopAsync();
                    await _connection.DisposeAsync();
                    Debug.Log("SignalR => Connection closed successfully");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"SignalR => Error closing connection: {ex.Message}");
                }
                finally
                {
                    _connection = null;
                }
            }
        }

        public async Task ConnectAsync(string token)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(ArenaLinkService.Instance.GetSignalRHubUrl(), options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(token);
                })
                .WithAutomaticReconnect()
                .Build();
            
            _connection.On<string>("ReceiveMessage", HandleMessage);
            _connection.On<string>("Info", HandleMessage);
            _connection.On<string>("Error", HandleErrorMessage);
            
            _connection.On<int>("LobbyCreated", OnLobbyCreated);
            _connection.On<int>("ConnectToMatch", OnMatchFound);
            _connection.On<int>("MatchStarted", OnMatchStarted);
            
            await _connection.StartAsync();
        }
        
        private static void HandleMessage(string message)
        {
            Debug.Log($"SignalR => Received message: {message}");
        }
        
        private static void HandleErrorMessage(string message)
        {
            Debug.LogError($"SignalR => Received error: {message}");
        }

        public async Task CreateLobby()
        {
            await _connection.SendAsync("CreateLobby");
        }
        
        private static void OnLobbyCreated(int lobbyId)
        {
            UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                NetworkManager.Instance.LobbyCreated(lobbyId);
            });
        }
        
        public async Task FindMatch()
        {
            await _connection.SendAsync("FindMatch");
        }

        public async Task AcceptMatch(int matchId)
        {
            await _connection.SendAsync("AcceptMatch", matchId);
        }
        
        private static void OnMatchFound(int matchId)
        {
            UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                NetworkManager.Instance.MatchFound(matchId);
            });
        }

        private static void OnMatchStarted(int matchId)
        {
            UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                NetworkManager.Instance.MatchStarted(matchId);
            });
        }
    }
}