using Autodesk.Forge;
using Autodesk.Forge.Model;
using MongoDB.Bson;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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

            // the respose come with expires_in in minutes, so let's also store the absolute time
            bearer.Dictionary.Add("expires_at", DateTime.UtcNow.AddSeconds((long)bearer.Dictionary["expires_in"]));

            // at this point we can store the access & refresh token on a database and return 
            // the respective DB unique ID, that way the application can refresh the token in
            // and the client will not see it. 
            string sessionIdUnprotected = await OAuthDB.RegisterUser(bearer);

            // and encrypt the database ID to send to the user
            string sessionIdProtected = Convert.ToBase64String(System.Web.Security.MachineKey.Protect(Encoding.UTF8.GetBytes(sessionIdUnprotected)));

            // return to user 
            HttpResponseMessage res = Request.CreateResponse(System.Net.HttpStatusCode.OK);
            res.Content = new StringContent(sessionIdProtected, Encoding.UTF8, "text/plain");

            return res;
        }

        [HttpGet]
        [Route("api/forge/session/isvalid")]
        public async Task<bool> GetIsSessionValid()
        {
            string sessionId, localId;
            if (!HeaderUtils.GetSessionLocalIDs(out sessionId, out localId)) return false;

            return await OAuthDB.IsSessionIdValid(sessionId, localId);
        }

        public class UserResponse
        {
            public string firstName;
            public string lastName;
            public string profilePicture;
        }

        [HttpGet]
        [Route("api/forge/user/profile")]
        public async Task<UserResponse> GetUserProfile()
        {
            string sessionId, localId;
            if (!HeaderUtils.GetSessionLocalIDs(out sessionId, out localId)) return null;
            if (!await OAuthDB.IsSessionIdValid(sessionId, localId)) return null;
            string userAccessToken = await OAuthDB.GetAccessToken(sessionId, localId);

            UserProfileApi userProfileApi = new UserProfileApi();
            userProfileApi.Configuration.AccessToken = userAccessToken;
            try
            {
                dynamic user = await userProfileApi.GetUserProfileAsync();
                UserResponse response = new UserResponse();
                response.firstName = user.firstName;
                response.lastName = user.lastName;

                return response;
            }
            catch(Exception e)
            {
                return null;
            }            
        }
    }
}
