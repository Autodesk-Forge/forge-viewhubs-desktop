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
        public static async Task<T> RequestAsync(string endpoint)
        {
            var client = new RestClient("http://localhost:3000");
            var request = new RestRequest(endpoint,  Method.GET);
            IRestResponse response = await client.ExecuteTaskAsync(request);
            return JsonConvert.DeserializeObject<T>(response.Content);
        }
    }
}
