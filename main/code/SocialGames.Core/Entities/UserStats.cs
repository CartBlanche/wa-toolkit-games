namespace Microsoft.Samples.SocialGames.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    public class UserStats
    {
        [Key]
        public string UserId { get; set; }

        [DataMember]
        public float XP { get; set; }

        [DataMember]
        public float Rank { get; set; }

        [DataMember]
        public float Kills { get; set; }

        [DataMember]
        public float Victories { get; set; }

        [DataMember]
        public float Accuracy { get; set; }

        [DataMember]
        public float TerrainDeformation { get; set; }
    }
}