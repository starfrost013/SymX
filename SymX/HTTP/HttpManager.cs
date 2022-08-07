using NuCore.Utilities;
using System.Net.Http;
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
                NCLogging.Log("Initialising HTTP client...");
                Client = new HttpClient
                {
                    BaseAddress = new Uri(baseAddress)
                };

                Client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(userAgentVendor, userAgentVersion));
                return true;
            }
            catch (Exception ex)
            {
                _ = new NCException($"An error occurred while initialising the HTTP client.\n\n{ex}", 105, "An exception occurred during HttpManager::Init.", NCExceptionSeverity.FatalError, null, true);
                return true; 
            }

        }
    }
}
