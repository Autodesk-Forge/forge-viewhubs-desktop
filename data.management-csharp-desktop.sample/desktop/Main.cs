using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace FPD.Sample.Desktop
{
    public partial class Main : Form
    {
        public ChromiumWebBrowser browser;

        public void InitBrowser()
        {
            Cef.Initialize(new CefSettings());
            browser = new ChromiumWebBrowser(Forge.EndPoints.OAuthRedirectURL);

            browser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            browser.Location = new System.Drawing.Point(261, 12);
            browser.MinimumSize = new System.Drawing.Size(20, 20);
            browser.Name = "webBrowser1";
            browser.Size = new System.Drawing.Size(455, 542);
            browser.TabIndex = 1;

            Controls.Add(browser);
        }

        public Main()
        {
            InitializeComponent();
            InitBrowser();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            PrepareUserData();
            browser.FrameLoadEnd += Browser_FrameLoadEnd;
        }

        private async void Browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            // if the browser redirect to this page, means the session ID is on the body
            if (e.Url.IndexOf("?code=") == -1) return;

            // remove this event...
            browser.FrameLoadEnd -= Browser_FrameLoadEnd;

            string htmlBody = await e.Browser.FocusedFrame.GetTextAsync();

            // store the session
            SessionManager.SessionId = Regex.Replace(htmlBody, "<[^>]*(>|$)", string.Empty);

            // clear the browser
            browser.Load(Forge.EndPoints.BaseURL);

            PrepareUserData();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cef.Shutdown();
        }

        private void btnSignout_Click(object sender, EventArgs e)
        {
            Cef.GetGlobalCookieManager().DeleteCookies(string.Empty, string.Empty);
            SessionManager.Signout();
            treeDataManagement.Nodes.Clear();
            lblUserName.Text = string.Empty;
            btnSignout.Visible = false;
            browser.Load(Forge.EndPoints.OAuthRedirectURL);
        }

        delegate void PrepareUserDataCallback();

        private async void PrepareUserData()
        {
            // make sure this code is being executed on the UI thread
            if (btnSignout.InvokeRequired || browser.InvokeRequired)
            {
                PrepareUserDataCallback c = new PrepareUserDataCallback(PrepareUserData);
                this.Invoke(c, null);
                return;
            }

            if (!SessionManager.HasSession || !(await SessionManager.IsSessionValid())) return;
            
            // empty
            browser.Load(Forge.EndPoints.BaseURL);

            btnSignout.Visible = true;

            // show user name
            Forge.User user = await Forge.User.UserNameAsync();
            lblUserName.Text = string.Format("{0} {1}", user.FirstName, user.LastName);

            // show hubs tree
            TreeNode[] hubs = await Forge.DataManagement.GetHubs();
            treeDataManagement.Nodes.AddRange(hubs);
        }

        private async void treeDataManagement_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string[] parameters = e.Node.Tag.ToString().Split('/');
            string resourceType = parameters[parameters.Length - 2];
            string resourceId = parameters[parameters.Length - 1];

            switch (resourceType)
            {
                case "hubs": // load projects
                    e.Node.Nodes.AddRange(await Forge.DataManagement.GetProjects(resourceId));
                    break;
                case "projects":
                    string hubId = parameters[parameters.Length - 3];
                    e.Node.Nodes.AddRange(await Forge.DataManagement.GetTopFolder(hubId, resourceId /*projectId*/));
                    break;
                case "folders":
                    string projectId = parameters[parameters.Length - 3];
                    e.Node.Nodes.AddRange(await Forge.DataManagement.GetFolderContents(projectId, resourceId/*folderId*/));
                    break;
                case "items":
                    projectId = parameters[parameters.Length - 3];
                    e.Node.Nodes.AddRange(await Forge.DataManagement.GetItemVersions(projectId, resourceId/*itemId*/));
                    break;
                case "versions":
                    browser.RequestHandler = new RequestHandler();
                    browser.Load(Forge.EndPoints.ViewerURL(resourceId));
                    //browser.ShowDevTools();
                    break;
            }

            e.Node.Expand();
        }

        private class RequestHandler : IRequestHandler
        {
            public CefReturnValue OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
            {
                var headers = request.Headers;
                headers["FPDSampleSessionId"] = SessionManager.SessionId;
                headers["FPDSampleLocalId"] = SessionManager.MachineId;
                request.Headers = headers;
                return CefReturnValue.Continue;
            }

            public bool GetAuthCredentials(IWebBrowser browserControl, IBrowser browser, IFrame frame, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback) { return false; }
            public IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response) { return null; }
            public bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect) { return false; }
            public bool OnCertificateError(IWebBrowser browserControl, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback) { return false; }
            public bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture) { return false; }
            public void OnPluginCrashed(IWebBrowser browserControl, IBrowser browser, string pluginPath) { }
            public bool OnProtocolExecution(IWebBrowser browserControl, IBrowser browser, string url) { return false; }
            public bool OnQuotaRequest(IWebBrowser browserControl, IBrowser browser, string originUrl, long newSize, IRequestCallback callback) { return false; }
            public void OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser, CefTerminationStatus status) { }
            public void OnRenderViewReady(IWebBrowser browserControl, IBrowser browser) { }
            public void OnResourceLoadComplete(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength) { }
            public void OnResourceRedirect(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, ref string newUrl) { }
            public bool OnResourceResponse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response) { return false; }
            public bool OnSelectClientCertificate(IWebBrowser browserControl, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback) { return false; }
            public bool CanGetCookies(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request) { return true; }
            public bool CanSetCookie(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, Cookie cookie) { return true; }
        }
    }
}
