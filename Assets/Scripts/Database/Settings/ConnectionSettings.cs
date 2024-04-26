using UnityEngine;

namespace Database.Settings
{
    public static class ConnectionSettings
    {
        public static string LocalServerAddress = "192.168.15.12";
        public static string apiUrl { get; private set; }

        public static string ConfigureAPIUrl()
        {
            apiUrl = $"http://" + LocalServerAddress + "/api/api.php";
            return apiUrl;
        }

        public static void SetNewServerAddress(string serverAddress)
        {
            LocalServerAddress = serverAddress;
            ConfigureAPIUrl();
        }
    }
}