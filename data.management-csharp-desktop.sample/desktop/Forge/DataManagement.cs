using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FPD.Sample.Desktop.Forge
{
    class DataManagement
    {
        public class Item
        {
            public string ID;
            public string Type;
            public string Text;
        }

        private async static Task<TreeNode[]> GetList(string endpoint)
        {
            IList<TreeNode> nodes = new List<TreeNode>();
            IList<Item> hubs = await RestAPI<IList<Item>>.RequestAsync(endpoint, true);
            foreach (Item hub in hubs)
            {
                TreeNode node = new TreeNode(hub.Text);
                node.Tag = hub.ID;
                nodes.Add(node);
            }
            return nodes.ToArray();
        }

        public async static Task<TreeNode[]> GetHubs()
        {
            return await GetList("api/forge/datamanagement/hubs");
        }

        public async static Task<TreeNode[]> GetProjects(string hubId)
        {
            return await GetList("api/forge/datamanagement/hubs/" + hubId + "/projects");
        }

        public async static Task<TreeNode[]> GetTopFolder(string hubId, string projectId)
        {
            return await GetList("api/forge/datamanagement/hubs/" + hubId + "/projects/" + projectId + "/topFolders");
        }

        public async static Task<TreeNode[]> GetFolderContents(string projectId, string folderId)
        {
            return await GetList("api/forge/datamanagement/projects/" + projectId + "/folders/" + folderId + "/contents");
        }

        public async static Task<TreeNode[]> GetItemVersions(string projectId, string itemId)
        {
            return await GetList("api/forge/datamanagement/projects/" + projectId + "/items/" + itemId + "/versions");
        }
    }
}
