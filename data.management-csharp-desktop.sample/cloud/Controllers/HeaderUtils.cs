using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace FPD.Sample.Cloud.Controllers
{
    public class HeaderUtils
    {
        public static bool GetSessionLocalIDs(out string sessionId, out string localId)
        {
            sessionId = string.Empty;
            localId = string.Empty;
            
            HttpRequest req = HttpContext.Current.Request;
            if (string.IsNullOrWhiteSpace(req.Headers["FPDSampleSessionId"])) return false;
            if (string.IsNullOrWhiteSpace(req.Headers["FPDSampleLocalId"])) return false;

            sessionId = req.Headers["FPDSampleSessionId"];
            localId = req.Headers["FPDSampleLocalId"];

            // unencrypt the sessionId
            sessionId = Encoding.UTF8.GetString(System.Web.Security.MachineKey.Unprotect(Convert.FromBase64String(sessionId)));

            return true;
        }
    }
}