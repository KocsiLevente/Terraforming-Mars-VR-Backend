using System;

namespace TerraformingMarsBackend.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public int GameRoomId { get; set; }
        public bool IsLobbyMessage { get; set; }
        public DateTime TimeSent { get; set; }
        public string Message { get; set; }

        public string ToJsonString()
        {
            return "{ \"Id\":" + Id + ", \"Name\": \"" + UserName + "\", \"TimeSent\": \"" + TimeSent.ToString("G") + "\", \"Message\": \""+ Message + "\"}";
        }
    }
}
