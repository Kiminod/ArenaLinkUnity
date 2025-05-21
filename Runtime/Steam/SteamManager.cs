using System;
using Steamworks;
using UnityEngine;

namespace ArenaLink.Steam
{
    public class SteamManager : MonoBehaviour
    {
        public static SteamManager Instance;
        public static bool Initialized => Instance != null &&  Instance._steamInitialized;

        public UserInfo UserInfo;
        
        public static event Action OnInit;

        private bool _steamInitialized;
        private SteamAuth _steamAuth;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            if (SteamAPI.RestartAppIfNecessary((AppId_t)480)) // AppID 480 (spacewars)
            {
                Application.Quit();
                return;
            }
            
            _steamInitialized = SteamAPI.Init();
            if (!_steamInitialized)
            {
                Debug.LogError("SteamAPI_Init() failed. Steam is not running.");
                return;
            }
            
            _steamAuth = new SteamAuth();
            UserInfo = new UserInfo();
        }

        private void OnEnable()
        {
            if (Instance != null) return;
            
            Instance = this;
            OnInit?.Invoke();
        }

        private void OnDestroy()
        {
            if (_steamInitialized)
            {
                SteamAPI.Shutdown();
            }
        }

        private void Update()
        {
            if (_steamInitialized)
            {
                SteamAPI.RunCallbacks();
            }
        }

        public string GetAuthTicket()
        {
            return _steamAuth.GetAuthTicket();
        }
    }
}