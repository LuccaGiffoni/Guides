using KBCore.Refs;
using SceneBehaviours.Operator;
using TMPro;
using UnityEngine;

namespace StepButtons
{
    public class OperatorStepButton : ValidatedMonoBehaviour
    {
        [HideInInspector] public int stepIndex { get; set; }
        [SerializeField, Self] public TextMeshProUGUI stepNumberText;
        [SerializeField, Scene] private OperationOperatorBehaviour operationOperatorBehaviour;

        private void Start() => operationOperatorBehaviour = FindFirstObjectByType<OperationOperatorBehaviour>();

        public void MoveToThisStep() => operationOperatorBehaviour.MoveToStep(stepIndex);
    }
}