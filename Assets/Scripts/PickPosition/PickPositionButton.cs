using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PickPositionButton : MonoBehaviour
{
    private Button button;
    public int stepIndex;
    public TextMeshProUGUI stepIndexText;
    private SetupOperation operation;

    private void Start()
    {
        button = GetComponent<Button>();
        stepIndexText = GetComponent<TextMeshProUGUI>();
        operation = GameObject.FindGameObjectWithTag("API").GetComponent<SetupOperation>();

        button.onClick.AddListener(() => ChangeRuntimeIndex());
    }

    private void ChangeRuntimeIndex()
    {
        operation.runtimeIndex = stepIndex - 1;

        if(GameObject.FindGameObjectWithTag("PickPositionManager").TryGetComponent(out PickPositionCreator pickPos))
        {
            pickPos.DestroyActiveCube();
        }
    }
}