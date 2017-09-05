using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPD.Sample.Desktop.Forge
{
    public static class EndPoints
    {
        public static string BaseURL { get { return ConfigurationManager.AppSettings["serverAddress"]; } }
        public static string OAuthRedirectURL { get { return BaseURL + "/oauth?LocalId=" + SessionManager.MachineId; } }
        public static string ViewerURL(string urn) { return BaseURL + "/Viewer?urn=" + urn; }
    }


    public class RestAPI<T>
    {
        public static async Task<T> RequestAsync(string endpoint, IDictionary<string, string> headers, bool includeIdHeaders)
        {
            var client = new RestClient(EndPoints.BaseURL);
            var request = new RestRequest(endpoint, Method.GET);

            if (includeIdHeaders)
            {
                if (headers == null) headers = new Dictionary<string, string>();
                headers.Add("FPDSampleSessionId", SessionManager.SessionId);
                headers.Add("FPDSampleLocalId", SessionManager.MachineId);
            }

            if (headers != null)
                foreach (KeyValuePair<string, string> header in headers)
                    request.AddHeader(header.Key, header.Value);

            IRestResponse response = await client.ExecuteTaskAsync(request);
            if (response.StatusCode!= System.Net.HttpStatusCode.OK)
                throw new Exception("Cannot call " + endpoint);
            return JsonConvert.DeserializeObject<T>(response.Content);
        }

        public static async Task<T> RequestAsync(string endpoint, bool includeIdHeaders)
        {
            return await RequestAsync(endpoint, null, includeIdHeaders);
        }
    }
}
