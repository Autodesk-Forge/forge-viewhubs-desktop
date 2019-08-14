using Autodesk.Forge;
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
                if (_database == null) _database = Client.GetDatabase(ConfigVariables.OAUTH_DATABASE.Split('/').Last().Split('?').First());
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
            try
            {
                var filterBuilder = Builders<BsonDocument>.Filter;
                var filter = filterBuilder.Eq("_id", new ObjectId(sessionId)) & filterBuilder.Eq("local_id", localId);
                var users = Database.GetCollection<BsonDocument>("users");
                long count = await users.CountAsync(filter);
                return (count == 1);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static async Task<string> GetAccessToken(string sessionId, string localId)
        {
            var filterBuilder = Builders<BsonDocument>.Filter;
            var filter = filterBuilder.Eq("_id", new ObjectId(sessionId)) & filterBuilder.Eq("local_id", localId);
            var users = Database.GetCollection<BsonDocument>("users");
            var docs = await users.Find(filter).ToListAsync();

            var doc = docs.First();
            var accessToken = (string)doc["access_token"];

            DateTime expiresAt = (DateTime)doc["expires_at"];
            if (expiresAt < DateTime.UtcNow)
            {
                // refresh the access_token maintaining the session id (document ID on the database)
                ThreeLeggedApi oauth = new ThreeLeggedApi();
                DynamicDictionary bearer = await oauth.RefreshtokenAsync(ConfigVariables.FORGE_CLIENT_ID, ConfigVariables.FORGE_CLIENT_SECRET, "refresh_token", (string)doc["refresh_token"]);

                var update = Builders<BsonDocument>.Update
                    .Set("access_token", (string)bearer.Dictionary["access_token"])
                    .Set("refresh_token", (string)bearer.Dictionary["refresh_token"])
                    .Set("expires_at", DateTime.UtcNow.AddSeconds((long)bearer.Dictionary["expires_in"]));

                var result = await users.UpdateOneAsync(filter, update);

                accessToken = (string)bearer.Dictionary["access_token"];
            }

            return accessToken;
        }
    }
}