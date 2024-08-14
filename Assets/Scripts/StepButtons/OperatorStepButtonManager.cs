using Data.Runtime;
using KBCore.Refs;
using SceneBehaviours.Operator;
using UnityEngine;

namespace StepButtons
{
    public class OperatorStepButtonManager : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Self] public OperatorStepButton stepButton;
        [SerializeField, Scene] private OperationOperatorBehaviour operationOperatorBehaviour;

        private void Start()
        {
            operationOperatorBehaviour = FindFirstObjectByType<OperationOperatorBehaviour>();
        }

        public void MoveToStep()
        {
            ManagerRuntimeData.selectedStep = ManagerRuntimeData.steps.Steps[stepButton.stepIndex - 1];
            operationOperatorBehaviour.UpdatePanelInformation();
        }
    }
}