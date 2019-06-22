using System;
using System.Net;

namespace DynastyParser.Classes
{
    class Helpers
    {
        public static HttpStatusCode CheckUrlResponse(string url)
        {
            HttpStatusCode result = default;

            Uri uri = new UriBuilder(url).Uri;
            var request = WebRequest.Create(uri);
            request.Method = "HEAD";
            try
            {
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    if (response == null) return result;
                    result = response.StatusCode;
                    response.Close();
                }
            }
            catch
            {
                return HttpStatusCode.NotFound;
            }

            return result;
        }
    }
}
