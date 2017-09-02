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
        private const string FILENAME = "SessionID.txt";

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
            }
        }

        public static bool HasSession
        {
            get
            {
                return !string.IsNullOrWhiteSpace(SessionId);
            }
        }

        public async static Task<bool> IsSessionValid()
        {
            return await Forge.User.IsSessionValid();
        }

        public static string MachineId
        {
            get
            {
                // MAC address, but can be another value
                // https://stackoverflow.com/a/7661829
                return (from nic in NetworkInterface.GetAllNetworkInterfaces() where nic.OperationalStatus == OperationalStatus.Up select nic.GetPhysicalAddress().ToString()).FirstOrDefault();
            }
        }

        public static void Signout()
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            if (store.FileExists(FILENAME)) store.DeleteFile(FILENAME);
            _sessionId = string.Empty;
        }
    }
}
