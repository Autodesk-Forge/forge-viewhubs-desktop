using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPD.Sample.Desktop.Forge
{
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PictureURL { get; set; }

        public static async Task<bool> IsSessionValid()
        {
            return await RestAPI<bool>.RequestAsync("/api/forge/session/isvalid", true);
        }

        public static async Task<User> UserNameAsync()
        {
            return await RestAPI<User>.RequestAsync("/api/forge/user/profile", true);
        }
    }
}
