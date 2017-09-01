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

        public static async Task<string> RegisterUser(DynamicDictionary bearer)
        {
            var document = new BsonDocument(bearer.Dictionary);

            try
            {
                var users = Database.GetCollection<BsonDocument>("users");
                await users.InsertOneAsync(document);

                return document["_id"].AsObjectId.ToString();
            }
            catch (Exception e)
            {
                return string.Empty;
            }           
        }
    }
}