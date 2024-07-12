using Data.Enums;
using Data.Responses;
using Data.Runtime;
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

        private void Start()
        {
            operationManagerBehaviour = FindFirstObjectByType<OperationManagerBehaviour>();
            
            if(operationManagerBehaviour != null) Debug.Log(operationManagerBehaviour.gameObject.name);
        }

        public void MoveToThisStep()
        {
            var response = Response<int>.Success(stepIndex);
            ManagerRuntimeData.index = stepIndex; 
            
            EventManager.StepEvents.OnStepChanged.Get(EChannels.Step).Invoke(response);
        }
    }
}