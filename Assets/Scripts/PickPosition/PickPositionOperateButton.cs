using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PickPositionOperateButton : MonoBehaviour
{
    private Button button;
    public int stepIndex;
    public TextMeshProUGUI stepIndexText;
    private OperateOperation operation;
    public string description;

    private void Start()
    {
        button = GetComponent<Button>();
        stepIndexText = GetComponent<TextMeshProUGUI>();
        operation = GameObject.FindGameObjectWithTag("API").GetComponent<OperateOperation>();

        button.onClick.AddListener(() => ChangeRuntimeIndex());
    }

    public void ChangeRuntimeIndex()
    {
        operation.runtimeIndex = stepIndex - 1;

        foreach(var cube in GameObject.FindGameObjectsWithTag("Cube"))
        {
            cube.GetComponent<InteractionManager>().ConfigureColor();
        }
    }

    public void SetDescription(string description)
    {
        this.description = description;
    }
}