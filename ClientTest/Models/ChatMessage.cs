using System;

namespace ClientTest.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TimeSent { get; set; }
        public string Message { get; set; }
    }
}
