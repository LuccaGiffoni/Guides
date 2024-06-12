using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ServerSettings : MonoBehaviour
{
    [Header("Action scripts")]
    [SerializeField] private NumericKeyboard numericKeyboard;
    [SerializeField] private SceneTransitionManager sceneTransitionManager;

    [Header("UI Feedback")]
    [SerializeField] private TextMeshProUGUI serverAddress;

    private void Start()
    {
        var serverAddressString = PlayerPrefs.GetString("serverAddress");

        if (!string.IsNullOrEmpty(serverAddressString))
        {
            serverAddress.text = "Servidor atual: " + serverAddressString;
        }
        else
        {
            serverAddress.text = "Servidor atual: Nenhum...";
        }
    }

    public void SaveServerAddress()
    {
        PlayerPrefs.SetString("serverAddress", numericKeyboard.GetServerAddress());
        sceneTransitionManager.GoToSceneWithoutFader(0);
    }
}