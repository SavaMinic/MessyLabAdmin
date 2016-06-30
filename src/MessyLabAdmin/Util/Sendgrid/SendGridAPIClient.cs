using SendGrid.CSharp.HTTP.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessyLabAdmin.Util.Sendgrid
{
    public class SendGridAPIClient
    {
        private string _apiKey;
        public string Version;
        public dynamic client;
        private Uri _baseUri;
        private enum Methods
        {
            GET, POST, PATCH, DELETE
        }

        /// <summary>
        ///     Create a client that connects to the SendGrid Web API
        /// </summary>
        /// <param name="apiKey">Your SendGrid API Key</param>
        /// <param name="baseUri">Base SendGrid API Uri</param>
        public SendGridAPIClient(string apiKey, string baseUri = "https://api.sendgrid.com", string version = "v3")
        {
            _baseUri = new Uri(baseUri);
            _apiKey = apiKey;
            Version = version;
            Dictionary<string, string> requestHeaders = new Dictionary<string, string>();
            requestHeaders.Add("Authorization", "Bearer " + apiKey);
            requestHeaders.Add("Content-Type", "application/json");
            requestHeaders.Add("User-Agent", "sendgrid/" + Version + " csharp");
            client = new Client(host: baseUri, requestHeaders: requestHeaders, version: version);
        }
    }
}
