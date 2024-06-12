using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using static UnityEngine.Rendering.HDROutputUtils;
using UnityEngine.UI;

public class SetupOperation : MonoBehaviour
{
    [SerializeField] public string apiUrl = "http://[serverAddress]/api/api.php";

    [Header("Debugger")]
    [SerializeField] private TextMeshProUGUI debugger;

    [Header("User Information")]
    [SerializeField] private TextMeshProUGUI stepIndexText;
    [SerializeField] private TextMeshProUGUI stepDescription;
    [SerializeField] private TextMeshProUGUI operationDescription;

    [Header("Buttons")]
    [SerializeField] private Transform scrollviewContent;
    [SerializeField] private GameObject stepButton;
    [SerializeField] private GameObject cubePrefab;

    [Header("Reference Zone")]
    [SerializeField] private Transform referenceZoneTransform;

    public List<Operation> ReceivedOperations = new();
    public List<Step> ReceivedSteps = new();

    public Step ActualStep;
    public int runtimeIndex;

    public List<PickPosition> PickPositions = new();

    private void Update()
    {
        if(ActualStep.StepIndex != runtimeIndex + 1 && ReceivedSteps.Count > 0)
        {
            ActualStep = ReceivedSteps[runtimeIndex];
            ConfigureUI();
        }
    }

    private void Start()
    {
        runtimeIndex = 0;

        if (PlayerPrefs.GetString("serverAddress") != null && PlayerPrefs.GetString("serverAddress") != string.Empty)
        {
            apiUrl = "http://" + PlayerPrefs.GetString("serverAddress") + "/api/api_old.php";
            ConsumeAPI();
        }
        else
        {
            debugger.text = "O endere�o do servidor n�o foi configurado!";

            #if UNITY_EDITOR
            apiUrl = LocalNetworkManager.LocalServerAddress;
            StartCoroutine(GetAllOperations());
            #endif
        }
    }

    public void ConsumeAPI()
    {
        debugger.text = $"Conectando com o servidor no endere�o {apiUrl}";
        StartCoroutine(GetAllOperations());
    }

    IEnumerator GetAllOperations()
    {
        using UnityWebRequest www = UnityWebRequest.Get(apiUrl + "?action=get_operations");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            debugger.text += $"\n{www.error}";
        }
        else
        {
            debugger.text += "\n\nConsultando a API...";

            string jsonResponse = www.downloadHandler.text;
            ReceivedOperations = JsonUtility.FromJson<OperationList>("{\"operations\":" + jsonResponse + "}").operations;

            if (ReceivedOperations.Count > 0)
            {
                debugger.text += $"\nA operação {ReceivedOperations[0].Description} foi recebida com sucesso.";

                StartCoroutine(GetStepsForOperation(ReceivedOperations[0].OperationID));
            }
            else
            {
                debugger.text += $"Nenhuma opera��o foi recebida do banco de dados. Verifique o banco de dados!";
            }
        }
    }

    IEnumerator GetStepsForOperation(int operationId)
    {
        string stepsUrl = apiUrl + "?action=get_steps&operationId=" + operationId;

        using UnityWebRequest www = UnityWebRequest.Get(stepsUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            debugger.text = www.error;
        }
        else
        {
            string jsonResponse = www.downloadHandler.text;
            ReceivedSteps = JsonUtility.FromJson<StepList>("{\"steps\":" + jsonResponse + "}").steps;

            debugger.text += $"Todos os {ReceivedSteps.Count} da opera��o foram recebidos.";

            InitialUISetup();
        }
    }

    public void ConfigureUI()
    {
        ActualStep = ReceivedSteps[runtimeIndex];
        stepIndexText.text = $"Passo {runtimeIndex + 1}";
        stepDescription.text = ReceivedSteps[runtimeIndex].Description;

        foreach(var pickPosition in PickPositions)
        {
            if(pickPosition.runtimeIndex == runtimeIndex)
            {
                var cube = Instantiate(cubePrefab);
                cube.GetComponent<PickPosition>().ConfigurePickPosition();
            }
        }
    }

    private void InitialUISetup()
    {
        foreach (var step in ReceivedSteps)
        {
            var button = Instantiate(stepButton, scrollviewContent).GetComponent<Button>();
            var pickPositionButton = button.GetComponent<PickPositionButton>();

            pickPositionButton.stepIndexText.text = $"Passo {step.StepIndex}";
            pickPositionButton.stepIndex = step.StepIndex;
        }

        ActualStep = ReceivedSteps[0];
        stepIndexText.text = $"Passo {0 + 1}";
        stepDescription.text = ReceivedSteps[0].Description;
    }
}