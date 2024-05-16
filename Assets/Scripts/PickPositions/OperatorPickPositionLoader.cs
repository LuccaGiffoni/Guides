﻿using Database.Settings;
using Helper;
using KBCore.Refs;
using Language;
using SceneBehaviours.OperationOperator;
using UnityEngine;
using UnityEngine.Serialization;

namespace PickPositions
{
    public class OperatorPickPositionLoader : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Scene] private OperationOperatorBehaviour operationOperatorBehaviour;
        [SerializeField, Scene] private PopupManager popupManager;
        
        [Header("Pick Positions")]
        [SerializeField] private GameObject pickPositionPrefab;

        private void OnEnable() => operationOperatorBehaviour.onStepsReceived.AddListener(CreateAllSavedPickPositions);

        private void CreateAllSavedPickPositions()
        {
            foreach (var step in OperatorRuntimeData.steps.Steps)
            {
                if (step.SX == 0 || step.SY == 0 || step.SZ == 0) return;
                
                var position = new Vector3(step.PX, step.PY , step.PZ);
                var rotation = Quaternion.Euler(step.RX, step.RY, step.RZ);
                var scale = new Vector3(step.SX, step.SY, step.SZ);
            
                var isPickPositionValid = Instantiate(pickPositionPrefab, position, Quaternion.identity,
                    OperatorRuntimeData.activeAnchor.transform).TryGetComponent(out OperatorPickPosition createdPickPosition);
                if (!isPickPositionValid)
                {
                    popupManager.SendMessageToUser(PickPositionLogMessages.pickPositionAlreadyCreatedOrLoaded, PopupType.Warning);
                    return;
                }
                
                createdPickPosition.SetPickPosition(OperatorRuntimeData.selectedStep.StepIndex, scale, position, rotation);
                OperatorRuntimeData.pickPositionsOnScene.Add(createdPickPosition);
            }
        }
        
        private void OnDisable() => operationOperatorBehaviour.onStepsReceived.RemoveListener(CreateAllSavedPickPositions);
    }
}