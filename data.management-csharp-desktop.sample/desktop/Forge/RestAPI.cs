using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPD.Sample.Desktop.Forge
{
    public class RestAPI<T>
    {
        public static async Task<T> RequestAsync(string endpoint, IDictionary<string, string> headers, bool includeIdHeaders)
        {
            var client = new RestClient("http://localhost:3000");
            var request = new RestRequest(endpoint, Method.GET);

            if (includeIdHeaders)
            {
                if (headers == null) headers = new Dictionary<string, string>();
                headers.Add("SessionId", SessionManager.SessionId);
                headers.Add("LocalId", SessionManager.MachineId);
            }

            if (headers!=null)
                foreach (KeyValuePair<string, string> header in headers)
                    request.AddHeader(header.Key, header.Value);

            IRestResponse response = await client.ExecuteTaskAsync(request);
            return JsonConvert.DeserializeObject<T>(response.Content);
        }

        public static async Task<T> RequestAsync(string endpoint, bool includeIdHeaders)
        {
            return await RequestAsync(endpoint, null, includeIdHeaders);
        }
    }
}
