using System;
using ArenaLink.Models.Responses;

namespace ArenaLink.Networking
{
    public partial class NetworkManager
    {
        public event Action<int> OnLobbyCreated;
        public event Action<int> OnMatchFound;
        public event Action<int> OnMatchStarted;
        public event Action<MatchUpdate> OnMatchUpdate;

        public void LobbyCreated(int lobbyId)
        {
            OnLobbyCreated?.Invoke(lobbyId);
        }

        public void MatchFound(int matchId)
        {
            OnMatchFound?.Invoke(matchId);
        }

        public void MatchStarted(int matchId)
        {
            OnMatchStarted?.Invoke(matchId);
        }
        
        public void MatchUpdate(MatchUpdate matchUpdate)
        {
            OnMatchUpdate?.Invoke(matchUpdate);
        }
    }
}