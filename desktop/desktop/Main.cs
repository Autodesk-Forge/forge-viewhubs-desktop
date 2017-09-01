using CefSharp;
using CefSharp.WinForms;
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FPD.Sample.Desktop
{
    public partial class Main : Form
    {
        public ChromiumWebBrowser browser;

        public void InitBrowser()
        {
            Cef.Initialize(new CefSettings());
            browser = new ChromiumWebBrowser(string.Empty);

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

            // store the session
            string htmlBody = await e.Browser.FocusedFrame.GetTextAsync();
            SessionManager.SessionId = Regex.Replace(htmlBody, "<[^>]*(>|$)", string.Empty);

            // clear the browser
            browser.LoadHtml(string.Empty);
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cef.Shutdown();
        }

        private void btnSignout_Click(object sender, EventArgs e)
        {
            Cef.GetGlobalCookieManager().DeleteCookies(string.Empty, string.Empty);
            SessionManager.Signout();
            PrepareUserData();
        }

        private void PrepareUserData()
        {
            if (!SessionManager.HasSession || !SessionManager.IsSessionValid)
            {
                browser.Load("http://localhost:3000/oauth?LocalId=" + SessionManager.MachineId);
                return;
            }
            
            // show user name

        }
    }
}
