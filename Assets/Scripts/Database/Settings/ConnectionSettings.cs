using UnityEngine;

namespace Database.Settings
{
    public static class ConnectionSettings
    {
        public static string apiUrl { get; private set; }
        public const string ServerPrefs = "Server";

        public static string ConfigureAPIUrl()
        {
            apiUrl = $"http://" + PlayerPrefs.GetString(ServerPrefs) + "/api/api.php";
            return apiUrl;
        }

        public static void SetNewServerAddress(string serverAddress)
        {
            PlayerPrefs.SetString(ServerPrefs, serverAddress);
            ConfigureAPIUrl();
        }
    }
}