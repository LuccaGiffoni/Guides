using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static SetupOperation;

public class OperationSetup : MonoBehaviour
{
    [Header("User Information")]
    [SerializeField] private TextMeshProUGUI stepIndexText;
    [SerializeField] private TextMeshProUGUI stepDescription;
    [SerializeField] private TextMeshProUGUI operationDescription;

    [Header("Buttons")]
    [SerializeField] private Transform scrollviewContent;
    [SerializeField] private GameObject stepButton;

    public void InitialUISetup(List<Step> steps)
    {
        foreach(var step in steps)
        {
            Instantiate(stepButton, scrollviewContent);
        }

        ConfigureUI(0);
    }

    public void ConfigureUI(int stepIndex)
    {
        //if(stepIndex <= APIManager.Instance.ReceivedSteps.Count)
        //{
        //    stepIndexText.text = $"Passo {stepIndex}";
        //    stepDescription.text = APIManager.Instance.ReceivedSteps[stepIndex].Description;
        //}
    }
}