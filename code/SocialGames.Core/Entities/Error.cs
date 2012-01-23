namespace Microsoft.Samples.SocialGames.Entities
{

    public class Error
    {
        public string ErrorCode { get; set; }

        public string ErrorString { get; set; } // Reason: unknownError | cannotConnect | userNotFound | notAuthenicated

        public string Description { get; set; }
    }
}