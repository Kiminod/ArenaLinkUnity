using System.Collections.Generic;
using ArenaLink.Models.Responses.Actions;

namespace ArenaLink.Models.Responses
{
    public class MatchUpdate
    {
        public int MatchId { get; set; }
        public long TimeStamp { get; set; }
        public List<ActionDecoder> GameActions { get; set; }
    }
}