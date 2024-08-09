using Data.Enums;
using Data.Responses;
using Data.ScriptableObjects;
using EventSystem;
using KBCore.Refs;
using TMPro;
using UnityEngine;

namespace StepButtons
{
    public class StepButton : ValidatedMonoBehaviour
    {
        private int index { get; set; }
        [SerializeField, Self] public TextMeshProUGUI stepNumberText;
        
        [Header("Runtime Data"), SerializeField] private RuntimeDataForManager runtimeDataForManager;

        public void Init(int stepIndex)
        {
            index = stepIndex;
            stepNumberText.text = index.ToString();
        }
        
        public void MoveToThisStep()
        {
            var response = Response<int>.Success(index);
            runtimeDataForManager.Index = index; 
            
            EventManager.StepEvents.OnStepChanged.Get(EChannels.Step).Invoke(response);
        }
    }
}