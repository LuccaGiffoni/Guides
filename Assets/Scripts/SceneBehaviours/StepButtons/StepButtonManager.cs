using Database.Settings;
using KBCore.Refs;
using SceneBehaviours.OperationManager;
using UnityEngine;

namespace SceneBehaviours.StepButtons
{
    public class StepButtonManager : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Self] public StepButton stepButton;
        [SerializeField, Scene] private OperationManagerBehaviour operationManagerBehaviour;

        private void Start()
        {
            operationManagerBehaviour = FindFirstObjectByType<OperationManagerBehaviour>();
        }

        public void MoveToStep()
        {
            RuntimeData.selectedStep = RuntimeData.steps.Steps[stepButton.stepIndex - 1];
            operationManagerBehaviour.ConfigureUserInterface();
        }
    }
}