using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class OperateOperation : MonoBehaviour
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
    private List<Button> Buttons = new();

    public Step ActualStep;
    public int runtimeIndex;

    private void Update()
    {
        if (ActualStep.StepIndex != runtimeIndex + 1 && ReceivedSteps.Count > 0)
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

            debugger.text = "Conectando pelo Editor no servidor local.";
            StartCoroutine(GetAllOperations());
#endif
        }
    }

    public void ConsumeAPI()
    {
        debugger.text += $"\nConectando com o servidor no endere�o {apiUrl}";
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
                debugger.text += $"\nA opera��o {ReceivedOperations[0].Description} foi recebida com sucesso.";

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

            debugger.text += $"\nAgora voc� j� pode dar in�cio aos {ReceivedSteps.Count} passos, clicando no bot�o 'Iniciar opera��o' no seu menu de pulso - todos os passos j� foram recebidos.";
        }
    }

    public void ConfigureUI()
    {
        ActualStep = ReceivedSteps[runtimeIndex];
        stepIndexText.text = $"Passo {runtimeIndex + 1}";
        stepDescription.text = ReceivedSteps[runtimeIndex].Description;

        if(GameObject.FindGameObjectsWithTag("Cube") != null)
        {
            foreach (var cube in GameObject.FindGameObjectsWithTag("Cube"))
            {
                cube.GetComponent<InteractionManager>().ConfigureColor();
            }
        }
    }

    public void CreateAllPickPositions()
    {
        foreach (var bt in GameObject.FindGameObjectsWithTag("StepButton"))
        {
            Destroy(bt);
        }
        foreach (var cube in GameObject.FindGameObjectsWithTag("Cube"))
        {
            Destroy(cube);
        }

        foreach (var step in ReceivedSteps)
        {
            var button = Instantiate(stepButton, scrollviewContent).GetComponent<Button>();
            var pickPositionButton = button.GetComponent<PickPositionOperateButton>();

            if(step.StepIndex > 1)
            {
                button.enabled = false;
                button.interactable = false;
            }
            Buttons.Add(button);

            pickPositionButton.stepIndexText.text = $"Passo {step.StepIndex}";
            pickPositionButton.stepIndex = step.StepIndex;
            pickPositionButton.SetDescription(step.PickPosition);

            var position = new Vector3(step.PX, step.PY, step.PZ);
            var rotation = new Quaternion(step.RX, step.RY, step.RZ, step.RW);
            var scale = new Vector3(step.SX, step.SY, step.SZ);

            Instantiate(cubePrefab);
            var relativePosition = position + referenceZoneTransform.position;
            cubePrefab.transform.SetPositionAndRotation(relativePosition, rotation);
            cubePrefab.transform.localScale = scale;
            cubePrefab.GetComponent<PickPositionOperate>().runtimeIndex = step.StepIndex - 1;
        }

        ConfigureUI();
    }

    public void ConfigurePickPositionsForNewReferenceZonePosition()
    {
        foreach(var cube in GameObject.FindGameObjectsWithTag("Cube"))
        {
            Destroy(cube);
        }

        foreach (var step in ReceivedSteps)
        {
            var position = new Vector3(step.PX, step.PY, step.PZ);
            var rotation = new Quaternion(step.RX, step.RY, step.RZ, step.RW);
            var scale = new Vector3(step.SX, step.SY, step.SZ);

            debugger.text += $"Step position: {position}, {rotation} and {scale}\n";

            Instantiate(cubePrefab);
            var relativePosition = position + referenceZoneTransform.position;
            cubePrefab.transform.SetPositionAndRotation(relativePosition, rotation);
            cubePrefab.transform.localScale = scale;
            cubePrefab.GetComponent<PickPositionOperate>().runtimeIndex = step.StepIndex - 1;
        }
    }

    public void UnlockNextButton()
    {
        StartCoroutine(UnlockNextButtonAfterTime());
    }

    private IEnumerator UnlockNextButtonAfterTime()
    {
        yield return new WaitForSeconds(ReceivedSteps[runtimeIndex].AssemblyTime);
        Buttons[runtimeIndex + 1].enabled = true;
        Buttons[runtimeIndex + 1].interactable = true;
    }
}