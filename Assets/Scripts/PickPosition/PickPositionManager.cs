using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class PickPositionManager : MonoBehaviour
{
    [Header("Debugger")]
    [SerializeField] private TextMeshProUGUI debugger;

    [Header("Scripts")]
    [SerializeField] private SetupOperation setupOperation;
    [SerializeField] private Transform referenceZoneTransform;

    public PickPosition runtimePickPosition;

    public void UpdatePickPosition()
    {
        StartCoroutine(UpdatePickPositionRoutine());
    }

    IEnumerator UpdatePickPositionRoutine()
    {
        if (GameObject.FindGameObjectWithTag("Cube").TryGetComponent(out runtimePickPosition))
        {
            debugger.text += $"\nAtualizando o passo com ID {setupOperation.ActualStep.StepID}";

            string url = setupOperation.apiUrl + "?action=update_step";

            var position = runtimePickPosition.gameObject.transform.position - referenceZoneTransform.position;

            WWWForm form = new();
            form.AddField("stepId", setupOperation.ActualStep.StepID.ToString());
            form.AddField("PX", position.x.ToString().Replace(',', '.'));
            form.AddField("PY", position.y.ToString().Replace(',', '.'));
            form.AddField("PZ", position.z.ToString().Replace(',', '.'));
            form.AddField("RX", runtimePickPosition.gameObject.transform.rotation.x.ToString().Replace(',', '.'));
            form.AddField("RY", runtimePickPosition.gameObject.transform.rotation.y.ToString().Replace(',', '.'));
            form.AddField("RZ", runtimePickPosition.gameObject.transform.rotation.z.ToString().Replace(',', '.'));
            form.AddField("RW", runtimePickPosition.gameObject.transform.rotation.w.ToString().Replace(',', '.'));
            form.AddField("SX", runtimePickPosition.gameObject.transform.localScale.x.ToString().Replace(',', '.'));
            form.AddField("SY", runtimePickPosition.gameObject.transform.localScale.y.ToString().Replace(',', '.'));
            form.AddField("SZ", runtimePickPosition.gameObject.transform.localScale.z.ToString().Replace(',', '.'));

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
                    setupOperation.PickPositions.Add(runtimePickPosition);

                    string jsonResponse = www.downloadHandler.text;
                    debugger.text += $"\n{jsonResponse}";
                }
            }

            yield return null;
        }
        else
        {
            debugger.text += "\nNenhum PickPosition foi criado. Clique no botão 'A' do controle direito para criar um.";
        }
    }
}
