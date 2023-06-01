using System;
using System.Collections.Generic;
using TerraformingMarsBackend.Service;

namespace TerraformingMarsBackend.Models
{
    public static class MultiplayerLobby
    {
        public static List<TerraformingMarsUser> OnlineUsers
        {
            get => GameDataService.GetAvailableUsers();
        }

        public static List<GameRoom> AvailableGameRooms
        {
            get => GameDataService.GetAvailableGameRooms();
        }

        public static List<ChatMessage> ChatMessages
        {
            get => GameDataService.GetLobbyChatMessages();
        }
    }
}
