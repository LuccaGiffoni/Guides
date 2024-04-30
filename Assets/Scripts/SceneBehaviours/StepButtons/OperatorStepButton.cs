﻿using KBCore.Refs;
using SceneBehaviours.OperationOperator;
using TMPro;
using UnityEngine;

namespace SceneBehaviours.StepButtons
{
    public class OperatorStepButton : ValidatedMonoBehaviour
    {
        [HideInInspector] public int stepIndex { get; set; }
        [SerializeField, Self] public TextMeshProUGUI stepNumberText;
        [SerializeField, Scene] private OperationOperatorBehaviour operationOperatorBehaviour;

        private void Start()
        {
            operationOperatorBehaviour = FindFirstObjectByType<OperationOperatorBehaviour>();
            
            if(operationOperatorBehaviour != null) Debug.Log(operationOperatorBehaviour.gameObject.name);
        }

        public void MoveToThisStep() => operationOperatorBehaviour.MoveToStep(stepIndex);
    }
}