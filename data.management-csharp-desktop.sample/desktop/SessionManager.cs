using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace FPD.Sample.Desktop
{
    class SessionManager
    {
        private static string _sessionId;
#if DEBUG
        private const string FILENAME = "SessionID.txt";
#else
        // On release we should use a name that doesn't make sense
        private const string FILENAME = "SomeRandomName.txt";
#endif

        /// <summary>
        /// Return the session id stored locally from the server
        /// </summary>
        public static string SessionId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_sessionId))
                {
                    IsolatedStorageFile store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
                    if (store.FileExists(FILENAME))
                    {
                        IsolatedStorageFileStream output = new IsolatedStorageFileStream(FILENAME, FileMode.Open, store);
                        StreamReader reader = new StreamReader(output);
                        _sessionId = reader.ReadToEnd().TrimEnd();
                        reader.Close();
                        output.Close();
                    }
                }
                return _sessionId;
            }
            set
            {
                IsolatedStorageFile store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
                IsolatedStorageFileStream stream = new IsolatedStorageFileStream(FILENAME, FileMode.Create, store);
                StreamWriter writer = new StreamWriter(stream);
                writer.WriteLine(value);
                writer.Close();
                stream.Close();
                _sessionId = value;
            }
        }

        /// <summary>
        /// Return TRUE if there is a session on the local instance. Use IsSessionValid to confirm with server
        /// </summary>
        public static bool HasSession
        {
            get
            {
                return !string.IsNullOrWhiteSpace(SessionId);
            }
        }


        /// <summary>
        /// Return TRUE if the session is valid on the server
        /// </summary>
        /// <returns></returns>
        public async static Task<bool> IsSessionValid()
        {
            return await Forge.User.IsSessionValid();
        }

        /// <summary>
        /// MachineID should be unique enough to identify where the request is comming from
        /// </summary>
        public static string MachineId
        {
            get
            {
                // MAC address, but can be another value
                // https://stackoverflow.com/a/7661829
                return (from nic in NetworkInterface.GetAllNetworkInterfaces() where nic.OperationalStatus == OperationalStatus.Up select nic.GetPhysicalAddress().ToString()).FirstOrDefault();
            }
        }

        /// <summary>
        /// Remove the local session file
        /// </summary>
        public static void Signout()
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            if (store.FileExists(FILENAME)) store.DeleteFile(FILENAME);
            _sessionId = string.Empty;
        }
    }
}
