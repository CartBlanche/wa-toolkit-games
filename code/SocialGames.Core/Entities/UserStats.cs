namespace Microsoft.Samples.SocialGames.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    public class UserStats
    {
        [Key]
        public string UserId { get; set; }

        [DataMember]
        public float GameCount { get; set; }

        [DataMember]
        public float Victories { get; set; }

        [DataMember]
        public float Defeats { get; set; }
    }
}