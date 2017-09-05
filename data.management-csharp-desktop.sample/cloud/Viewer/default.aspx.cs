using FPD.Sample.Cloud.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FPD.Sample.Cloud.Viewer
{
    public partial class _default : System.Web.UI.Page
    {
        protected async void Page_Load(object sender, EventArgs e)
        {
            string sessionId, localId;
            if (!HeaderUtils.GetSessionLocalIDs(out sessionId, out localId)) return;
            if (!await OAuthDB.IsSessionIdValid(sessionId, localId)) return;

            string urn = Page.Request.QueryString["urn"];
            string proxyRoute = string.Format("{0}/api/forge/viewerproxy/", Request.Url.GetLeftPart(UriPartial.Authority));

            string script = string.Format("<script>showModel('{0}', '{1}');</script>", urn, proxyRoute);

            ClientScript.RegisterStartupScript(GetType(), "ShowModel", script);
        }


        private static string Protect(string data)
        {
            return Convert.ToBase64String(System.Web.Security.MachineKey.Protect(System.Text.Encoding.UTF8.GetBytes(data)));
        }
    }
}