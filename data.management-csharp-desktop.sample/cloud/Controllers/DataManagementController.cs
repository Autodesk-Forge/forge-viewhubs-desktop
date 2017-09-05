using Autodesk.Forge;
using Autodesk.Forge.Model;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace FPD.Sample.Cloud.Controllers
{
    public class DataManagementController : ApiController
    {
        public class Item
        {
            public Item(string id, string text, string type)
            {
                this.ID = id;
                this.Type = type;
                this.Text = text;
            }

            public string ID;
            public string Type;
            public string Text;
        }

        [HttpGet]
        [Route("api/forge/datamanagement/hubs")]
        public async Task<IList<Item>> GetHubs()
        {
            string sessionId, localId;
            if (!HeaderUtils.GetSessionLocalIDs(out sessionId, out localId)) return null;
            if (!await OAuthDB.IsSessionIdValid(sessionId, localId)) return null;
            string userAccessToken = await OAuthDB.GetAccessToken(sessionId, localId);

            HubsApi hubsApi = new HubsApi();
            hubsApi.Configuration.AccessToken = userAccessToken;

            IList<Item> nodes = new List<Item>();
            var hubs = await hubsApi.GetHubsAsync();
            foreach (KeyValuePair<string, dynamic> hubInfo in new DynamicDictionaryItems(hubs.data))
            {
                string hubType = "hubs";
                switch ((string)hubInfo.Value.attributes.extension.type)
                {
                    case "hubs:autodesk.core:Hub":
                        hubType = "hubs";
                        break;
                    case "hubs:autodesk.a360:PersonalHub":
                        hubType = "personalhub";
                        break;
                    case "hubs:autodesk.bim360:Account":
                        hubType = "bim360hubs";
                        break;
                }
                Item item = new Item(hubInfo.Value.links.self.href, hubInfo.Value.attributes.name, hubType);
                nodes.Add(item);
            }

            return nodes;
        }

        [HttpGet]
        [Route("api/forge/datamanagement/hubs/{hubId}/projects")]
        public async Task<IList<Item>> GetProjectsAsync(string hubId)
        {
            string sessionId, localId;
            if (!HeaderUtils.GetSessionLocalIDs(out sessionId, out localId)) return null;
            if (!await OAuthDB.IsSessionIdValid(sessionId, localId)) return null;
            string userAccessToken = await OAuthDB.GetAccessToken(sessionId, localId);

            IList<Item> nodes = new List<Item>();

            ProjectsApi projectsApi = new ProjectsApi();
            projectsApi.Configuration.AccessToken = userAccessToken;
            var projects = await projectsApi.GetHubProjectsAsync(hubId);
            foreach (KeyValuePair<string, dynamic> projectInfo in new DynamicDictionaryItems(projects.data))
            {
                string projectType = "projects";
                switch ((string)projectInfo.Value.attributes.extension.type)
                {
                    case "projects:autodesk.core:Project":
                        projectType = "a360projects";
                        break;
                    case "projects:autodesk.bim360:Project":
                        projectType = "bim360projects";
                        break;
                }
                Item projectNode = new Item(projectInfo.Value.links.self.href, projectInfo.Value.attributes.name, projectType);
                nodes.Add(projectNode);
            }

            return nodes;
        }

        [HttpGet]
        [Route("api/forge/datamanagement/hubs/{hubId}/projects/{projectId}/topFolders")]
        public async Task<IList<Item>> GetTopFoldersAsync(string hubId, string projectId)
        {
            string sessionId, localId;
            if (!HeaderUtils.GetSessionLocalIDs(out sessionId, out localId)) return null;
            if (!await OAuthDB.IsSessionIdValid(sessionId, localId)) return null;
            string userAccessToken = await OAuthDB.GetAccessToken(sessionId, localId);

            IList<Item> nodes = new List<Item>();

            ProjectsApi projectsApi = new ProjectsApi();
            projectsApi.Configuration.AccessToken = userAccessToken;
            var folders = await projectsApi.GetProjectTopFoldersAsync(hubId, projectId);
            foreach (KeyValuePair<string, dynamic> folderInfo in new DynamicDictionaryItems(folders.data))
            {
                Item projectNode = new Item(folderInfo.Value.links.self.href, folderInfo.Value.attributes.displayName, "folders");
                nodes.Add(projectNode);
            }

            return nodes;
        }


        [HttpGet]
        [Route("api/forge/datamanagement/projects/{projectId}/folders/{folderId}/contents")]
        public async Task<IList<Item>> GetFolderContentsAsync(string projectId, string folderId)
        {
            string sessionId, localId;
            if (!HeaderUtils.GetSessionLocalIDs(out sessionId, out localId)) return null;
            if (!await OAuthDB.IsSessionIdValid(sessionId, localId)) return null;
            string userAccessToken = await OAuthDB.GetAccessToken(sessionId, localId);

            IList<Item> folderItems = new List<Item>();

            FoldersApi folderApi = new FoldersApi();
            folderApi.Configuration.AccessToken = userAccessToken;
            var folderContents = await folderApi.GetFolderContentsAsync(projectId, folderId);
            foreach (KeyValuePair<string, dynamic> folderContentItem in new DynamicDictionaryItems(folderContents.data))
            {
                string displayName = folderContentItem.Value.attributes.displayName;
                if (string.IsNullOrWhiteSpace(displayName))
                {
                    continue;
                }

                Item itemNode = new Item(folderContentItem.Value.links.self.href, displayName, (string)folderContentItem.Value.type);

                folderItems.Add(itemNode);
            }

            return folderItems;
        }

        [HttpGet]
        [Route("api/forge/datamanagement/projects/{projectId}/items/{itemId}/versions")]
        public async Task<IList<Item>> GetItemVersionsAsync(string projectId, string itemId)
        {
            string sessionId, localId;
            if (!HeaderUtils.GetSessionLocalIDs(out sessionId, out localId)) return null;
            if (!await OAuthDB.IsSessionIdValid(sessionId, localId)) return null;
            string userAccessToken = await OAuthDB.GetAccessToken(sessionId, localId);

            IList<Item> versionsList = new List<Item>();

            ItemsApi itemsApi = new ItemsApi();
            itemsApi.Configuration.AccessToken = userAccessToken;
            var versions = await itemsApi.GetItemVersionsAsync(projectId, itemId);
            foreach (KeyValuePair<string, dynamic> version in new DynamicDictionaryItems(versions.data))
            {
                DateTime versionDate = version.Value.attributes.lastModifiedTime;

                string urn = string.Empty;
                try { urn = (string)version.Value.relationships.derivatives.data.id; }
                catch { urn = "not_available"; } // some BIM 360 versions don't have viewable

                Item itemNode = new Item("/versions/" + urn, versionDate.ToString("dd/MM/yy HH:mm:ss"), "versions");

                versionsList.Add(itemNode);
            }

            return versionsList;
        }
    }
}
