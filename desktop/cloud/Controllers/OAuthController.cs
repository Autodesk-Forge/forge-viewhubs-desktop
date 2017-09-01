using Autodesk.Forge;
using Autodesk.Forge.Model;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace FPD.Sample.Cloud.Controllers
{
    public class OAuthController : ApiController
    {
        [HttpGet]
        [Route("api/forge/callback/oauth")] // see Web.Config FORGE_CALLBACK_URL variable
        public async Task<HttpResponseMessage> OAuthCallback(string code, string state)
        {
            ThreeLeggedApi oauth = new ThreeLeggedApi();
            DynamicDictionary bearer = await oauth.GettokenAsync(ConfigVariables.FORGE_CLIENT_ID, ConfigVariables.FORGE_CLIENT_SECRET, oAuthConstants.AUTHORIZATION_CODE, code, ConfigVariables.FORGE_CALLBACK_URL);

            // the local_id of the requester machine should be provised on the original 
            // login call as passed as state
            if (!string.IsNullOrWhiteSpace(state)) bearer.Dictionary.Add("local_id", state);

            // at this point we can store the access & refresh token on a database and return 
            // the respective DB unique ID, that way the application can refresh the token in
            // and the client will not see it. 
            string sessionId = await OAuthDB.RegisterUser(bearer);

            // and encrypt the database ID to send to the user
            byte[] userIdentifier = System.Web.Security.MachineKey.Protect(Encoding.UTF8.GetBytes(sessionId));

            // return to user 
            HttpResponseMessage res = Request.CreateResponse(System.Net.HttpStatusCode.OK);
            res.Content = new StringContent(Convert.ToBase64String(userIdentifier), Encoding.UTF8, "text/plain");

            return res;
        }
    }
}
