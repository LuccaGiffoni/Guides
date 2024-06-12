using UnityEngine;

namespace Scripts_v2.Utilities
{
    public static class DatabaseConfiguration
    {
        public static string BaseUrl = "http://" + PlayerPrefs.GetString(nameof(DatabaseConfiguration)) + "/api/api.php";

        public static void UpdateServerAddress(string newAddress)
        {
            PlayerPrefs.SetString(nameof(DatabaseConfiguration), newAddress);
        }
    }
}