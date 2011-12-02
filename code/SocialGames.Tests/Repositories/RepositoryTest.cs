namespace Microsoft.Samples.SocialGames.Tests.Repositories
{
    using System.Globalization;
    using System.Web.Script.Serialization;

    public abstract class RepositoryTest
    {
        protected string Serialized<T>(T objectToSerialize, bool jsonpSupport)
        {
            var serializer = new JavaScriptSerializer();
            var serialized = serializer.Serialize(objectToSerialize);

            if (jsonpSupport)
            {
                serialized = string.Format(CultureInfo.InvariantCulture, "Callback({0})", serialized);
            }

            return serialized;
        }
    }
}