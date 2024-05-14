using KBCore.Refs;
using Scene;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Database.Settings
{
    public class ServerManager : ValidatedMonoBehaviour
    {
        [SerializeField, Scene] private SceneTransitionManager sceneTransitionManager;
        
        [SerializeField] private InputField inputField;
        [SerializeField] private TextMeshProUGUI serverAddress;

        private void Start()
        {
            serverAddress.text = "Servidor atual: " + PlayerPrefs.GetString(ConnectionSettings.ServerPrefs);
        }

        public void UpdateServerAddress()
        {
            ConnectionSettings.SetNewServerAddress(inputField.text);
            
            serverAddress.text = "Servidor atual: " + PlayerPrefs.GetString(ConnectionSettings.ServerPrefs);
            sceneTransitionManager.LoadSceneByIndex(0);
        }
    }
}
