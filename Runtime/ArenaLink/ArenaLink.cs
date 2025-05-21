using System;
using ArenaLink.ArenaLink.Config;
using ArenaLink.ArenaLink.MainThreadDispatcher;
using ArenaLink.Networking;
using ArenaLink.Steam;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ArenaLink.ArenaLink
{
    public class ArenaLinkService
    {
        public static ArenaLinkService Instance { get; private set; }
        
        private readonly ArenaLinkOptions _options;

        private bool _instantiated;
        
        public ArenaLinkService(ArenaLinkOptions options)
        {
            _options = options;
        }

        public static ArenaLinkBuilder Configure(Action<ArenaLinkOptions> configAction)
        {
            return new ArenaLinkBuilder().Configure(configAction);
        }
        
        public string GetBaseUrl() => _options.BaseUrl;
        public string GetApiBaseUrl() => _options.ApiBaseUrl;
        public string GetSignalRHubUrl() => _options.SignalRHubUrl;
        public string GetUdpServerAddress() => _options.UdpServerAddress;
        public int GetUdpServerPort() => _options.UdpServerPort;

        public void Init()
        {
            if (_instantiated)
            {
                throw new Exception("ArenaLink is already instantiated. You cannot instantiate it again.");
            }
            
            var configurationError = CheckConfiguration();
            if (!string.IsNullOrEmpty(configurationError))
            {
                throw new Exception("ArenaLink is not configured properly. Please check the configuration. " +
                                    "Error: " + configurationError);
            }

            Instance = this;
            _instantiated = true;
            
            var arenaLinkObject = new GameObject("ArenaLinkManager");
            Object.DontDestroyOnLoad(arenaLinkObject);
            
            // Initialize the SteamManager (for Steamworks integration)
            var steamManagerGameObject = new GameObject("SteamManager");
            steamManagerGameObject.AddComponent<SteamManager>();
            steamManagerGameObject.transform.SetParent(arenaLinkObject.transform);
            
            // Initialize the NetworkManager (for managing network connections)
            var networkManagerGameObject = new GameObject("NetworkManager");
            networkManagerGameObject.AddComponent<NetworkManager>();
            networkManagerGameObject.transform.SetParent(arenaLinkObject.transform);
            
            // Initialize the UnityMainThreadDispatcher (for executing actions on the main thread)
            var unityMainThreadDispatcherGameObject = new GameObject("UnityMainThreadDispatcher");
            unityMainThreadDispatcherGameObject.AddComponent<UnityMainThreadDispatcher>();
            unityMainThreadDispatcherGameObject.transform.SetParent(arenaLinkObject.transform);
        }

        private string CheckConfiguration()
        {
            if (_options.BaseUrl == null)
            {
                return "Base URL is not set.";
            }

            if (_options.UdpServerAddress == null)
            {
                return "UDP Server Address is not set.";
            }

            if (_options.UdpServerPort == 0)
            {
                return "UDP Server Port is not set.";
            }

            return string.Empty;
        }
    }
}