using TMPro;
using UnityEngine.Networking;
using UnityEngine;
using System.Collections;
using System.Globalization;
using UnityEngine.XR.Interaction.Toolkit;

public class ReferenceZoneManager : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private SetupOperation setupOperation;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private XRGrabInteractable xRGrabInteractable;

    [Header("Debugger")]
    [SerializeField] private TextMeshProUGUI debugger;

    [Header("API")]
    [SerializeField] public string apiUrl = "http://[serverAddress]/api/api.php";

    private void Start()
    {
        if (PlayerPrefs.GetString("serverAddress") != null && PlayerPrefs.GetString("serverAddress") != string.Empty)
        {
            apiUrl = "http://" + PlayerPrefs.GetString("serverAddress") + "/api/api.php";
        }
        else
        {
            debugger.text = "O endereço do servidor não foi configurado!";
        }
    }

    public void UpdateOperation()
    {
        debugger.text += $"\n\nAtualizando a Operação com ID {setupOperation.ReceivedOperations[0].OperationID}.";
        StartCoroutine(UpdateOperationZoneReference(setupOperation.ReceivedOperations[0].OperationID));
    }

    IEnumerator UpdateOperationZoneReference(int operationID)
    {
        string url = apiUrl + "?action=update_operation";
        debugger.text += $"Position: {transform.position} & Rotation: {transform.rotation}";

        WWWForm form = new();
        form.AddField("operationId", operationID.ToString());
        form.AddField("PX", transform.position.x.ToString().Replace(',', '.'));
        form.AddField("PY", transform.position.y.ToString().Replace(',', '.'));
        form.AddField("PZ", transform.position.z.ToString().Replace(',', '.'));
        form.AddField("RX", transform.rotation.x.ToString().Replace(',', '.'));
        form.AddField("RY", transform.rotation.y.ToString().Replace(',', '.'));
        form.AddField("RZ", transform.rotation.z.ToString().Replace(',', '.'));
        form.AddField("RW", transform.rotation.w.ToString().Replace(',', '.'));


        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                debugger.text += $"\n{www.error}";
            }
            else
            {
                debugger.text += "\nEnviando dados para a API...";

                string jsonResponse = www.downloadHandler.text;
                debugger.text += $"\n\n{jsonResponse}";
            }
        }

        yield return null;
    }

    public void ChangeReferenceZoneVisibility()
    {
        if (meshRenderer.enabled)
        {
            meshRenderer.enabled = false;
            xRGrabInteractable.enabled = false;
        }
        else
        {
            meshRenderer.enabled = true;
            xRGrabInteractable.enabled = true;
        }
    }
}