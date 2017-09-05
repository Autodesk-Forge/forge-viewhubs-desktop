using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Autodesk.Forge;
using FPD.Sample.Cloud;

namespace FPD.Sample.Cloud.Pages.OAuth
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ThreeLeggedApi oAuth3legged = new ThreeLeggedApi();
            string oauthUrl = oAuth3legged.Authorize(
                ConfigVariables.FORGE_CLIENT_ID,
                oAuthConstants.CODE,
                ConfigVariables.FORGE_CALLBACK_URL,
                new Scope[] { Scope.DataRead },
                Page.Request.QueryString["localId"]);
            Response.Redirect(oauthUrl);
        }
    }
}