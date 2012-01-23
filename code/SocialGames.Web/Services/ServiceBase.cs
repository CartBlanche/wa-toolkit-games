namespace Microsoft.Samples.SocialGames.Web.Services
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;

    public class ServiceBase
    {
        private IUserProvider userProvider;

        public ServiceBase()
            : this(new HttpContextUserProvider())
        {
        }

        public ServiceBase(IUserProvider userProvider)
        {
            this.userProvider = userProvider;
        }

        public static HttpResponseMessage SuccessResponse
        {
            get { return HttpResponse(HttpStatusCode.OK); }
        }

        public string CurrentUserId
        {
            get { return this.userProvider.UserId; }
        }

        public static HttpResponseMessage HttpStatusResponse(HttpStatusCode statusCode, string description = "")
        {
            return HttpResponse<string>(description, statusCode);
        }

        public static HttpResponseMessage BadRequest(string description)
        {
            return HttpStatusResponse(HttpStatusCode.BadRequest, description);
        }

        public static HttpResponseMessage HttpResponse<T>(T content, HttpStatusCode statusCode = HttpStatusCode.OK, string contentType = "")
        {
            HttpResponseMessage tempResponse = null;
            HttpResponseMessage response = null;

            try
            {
                if (typeof(T) == typeof(string))
                {
                    tempResponse = new HttpResponseMessage();
                    tempResponse.Content = new StringContent(content.ToString(), Encoding.UTF8);
                }
                else
                {
                    tempResponse = new HttpResponseMessage<T>(content);
                }

                tempResponse.StatusCode = statusCode;

                if (!string.IsNullOrWhiteSpace(contentType))
                {
                    tempResponse.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                }

                response = tempResponse;
                tempResponse = null;
            }
            finally
            {
                if (tempResponse != null)
                {
                    tempResponse.Dispose();
                }
            }

            return response;
        }

    }
}