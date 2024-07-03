using EventSystem;
using EventSystem.Enums;
using KBCore.Refs;
using Responses;
using SceneBehaviours.OperationManager;
using TMPro;
using UnityEngine;

namespace SceneBehaviours.StepButtons
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
            EventManager.StepEvents.OnStepChanged.Get(EChannels.Step).Invoke(response);
        }
    }
}