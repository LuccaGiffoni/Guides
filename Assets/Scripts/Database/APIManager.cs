using UnityEngine;

public class APIManager : MonoBehaviour
{
    public static string apiUrl = "http://[server]/api/api_old.php";

    public static string ConfigureAPIUrl()
    {
        if (PlayerPrefs.GetString("serverAddress") != null && PlayerPrefs.GetString("serverAddress") != string.Empty)
        {
            apiUrl = "http://" + PlayerPrefs.GetString("serverAddress") + "/api/api_old.php";
            return apiUrl;
        }
        else
        {
            return "O endereço do servidor não foi configurado!";
        }
    }
}
