using Autodesk.Forge.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace FPD.Sample.Cloud
{
    public static class OAuthDB
    {
        private static MongoClient _client = null;
        private static IMongoDatabase _database = null;

        private static MongoClient Client
        {
            get
            {
                if (_client == null) _client = new MongoClient(ConfigVariables.OAUTH_DATABASE);
                return _client;
            }
        }
        

        private static IMongoDatabase Database
        {
            get
            {
                if (_database == null) _database = Client.GetDatabase("desktopcloudsample");
                return _database;
            }
        }

        /// <summary>
        /// Store the response from Forge POST gettoken
        /// </summary>
        /// <param name="autodeskOAuthToken">The POST gettoken response + local_id from Client identification</param>
        /// <returns>SessionId for this user</returns>
        public static async Task<string> RegisterUser(DynamicDictionary autodeskOAuthToken)
        {
            var document = new BsonDocument(autodeskOAuthToken.Dictionary);

            try
            {
                var users = Database.GetCollection<BsonDocument>("users");
                await users.InsertOneAsync(document);

                // the unique id that identifies the user
                return document["_id"].AsObjectId.ToString();
            }
            catch (Exception e)
            {
                return string.Empty;
            }           
        }

        public static async Task<bool> IsSessionIdValid(string sessionId, string localId)
        {
            return true;
        }
    }
}