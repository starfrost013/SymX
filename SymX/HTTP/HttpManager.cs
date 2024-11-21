using System.Net.Http.Headers;

namespace SymX
{
    public static class HttpManager
    {
        public static HttpClient Client { get; set; }

        public static bool Init(string baseAddress, string userAgentVendor, string userAgentVersion)
        {
            try
            {
                Logger.Log("Initialising HTTP client...");
                Client = new HttpClient
                {
                    BaseAddress = new Uri(baseAddress)
                };

                Client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(userAgentVendor, userAgentVersion));
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError($"An error occurred while initialising the HTTP client.\n\n{ex}", 105, LoggerSeverity.FatalError, null, true);
                return true; 
            }

        }
    }
}
