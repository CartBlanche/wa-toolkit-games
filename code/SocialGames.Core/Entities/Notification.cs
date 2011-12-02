namespace Microsoft.Samples.SocialGames.Entities
{
    using System;

    public class Notification
    {
        public Guid Id { get; set; }

        public string Type { get; set; } // system | gift | achievement

        public string Message { get; set; }

        public string IconUrl { get; set; }

        public string SenderId { get; set; }

        public string SenderName { get; set; }

        public string Url { get; set; }

        public object Data { get; set; }

        public DateTime DateTime { get; set; }
    }
}