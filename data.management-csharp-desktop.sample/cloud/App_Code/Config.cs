using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace FPD.Sample.Cloud
{
    public class ConfigVariables
    {
        /// <summary>
        /// The client ID of this app
        /// </summary>
        internal static string FORGE_CLIENT_ID { get { return GetAppSetting("FORGE_CLIENT_ID"); } }

        /// <summary>
        /// The client secret of this app
        /// </summary>
        internal static string FORGE_CLIENT_SECRET { get { return GetAppSetting("FORGE_CLIENT_SECRET"); } }

        /// <summary>
        /// The client secret of this app
        /// </summary>
        internal static string FORGE_CALLBACK_URL { get { return GetAppSetting("FORGE_CALLBACK_URL"); } }

        internal static string OAUTH_DATABASE { get { return GetAppSetting("OAUTH_DATABASE"); } }

        /// <summary>
        /// Read settings from web.config.
        /// See appSettings section for more details.
        /// </summary>
        /// <param name="settingKey"></param>
        /// <returns></returns>
        private static string GetAppSetting(string settingKey)
        {
            return WebConfigurationManager.AppSettings[settingKey];
        }
    }
}