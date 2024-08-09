using Data.Enums;
using Data.Responses;
using Data.Runtime;
using Data.ScriptableObjects;
using Data.Settings;
using EventSystem;
using KBCore.Refs;
using SceneBehaviours.Manager;
using TMPro;
using UnityEngine;

namespace StepButtons
{
    public class StepButton : ValidatedMonoBehaviour
    {
        [HideInInspector] public int stepIndex { get; set; }
        [SerializeField, Self] public TextMeshProUGUI stepNumberText;
        
        [SerializeField, Scene] private OperationManagerBehaviour operationManagerBehaviour;
        [Header("Runtime Data"), SerializeField] private RuntimeDataForManager runtimeDataForManager;

        private void Start() => operationManagerBehaviour = FindFirstObjectByType<OperationManagerBehaviour>();

        public void MoveToThisStep()
        {
            var response = Response<int>.Success(stepIndex);
            runtimeDataForManager.Index = stepIndex; 
            
            EventManager.StepEvents.OnStepChanged.Get(EChannels.Step).Invoke(response);
        }
    }
}