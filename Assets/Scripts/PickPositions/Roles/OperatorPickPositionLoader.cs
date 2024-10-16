using Data.Enums;
using Data.ScriptableObjects;
using KBCore.Refs;
using Messages;
using SceneBehaviours.Operator;
using Services.Implementations;
using UnityEngine;
using UnityEngine.Serialization;

namespace PickPositions.Roles
{
    public class OperatorPickPositionLoader : ValidatedMonoBehaviour
    {
        #region Properties
        
        [Header("References")]
        [SerializeField, Scene] private OperationOperatorBehaviour operationOperatorBehaviour;
        [FormerlySerializedAs("popupManager")] [SerializeField] private PopupService popupService;
        
        [Header("Pick Positions"), SerializeField] private GameObject pickPositionPrefab;
        [Header("Runtime Data"), SerializeField] private RuntimeDataForOperator runtimeDataForOperator;

        #endregion
        
        #region Events
        
        private void OnEnable() => operationOperatorBehaviour.onStepsReceived.AddListener(CreateAllSavedPickPositions);
        private void OnDisable() => operationOperatorBehaviour.onStepsReceived.RemoveListener(CreateAllSavedPickPositions);

        #endregion
        
        private void CreateAllSavedPickPositions()
        {
            foreach (var step in runtimeDataForOperator.Steps.Steps)
            {
                if (step.SX == 0 || step.SY == 0 || step.SZ == 0) return;
                
                var position = new Vector3(step.PX, step.PY , step.PZ);
                var rotation = new Quaternion(step.RX, step.RY, step.RZ, step.RW);
                var scale = new Vector3(step.SX, step.SY, step.SZ);
            
                var isPickPositionValid = Instantiate(pickPositionPrefab, position, Quaternion.identity,
                    runtimeDataForOperator.OVRSpatialAnchor.transform).TryGetComponent(out OperatorPickPosition createdPickPosition);
                
                if (!isPickPositionValid)
                {
                    popupService.SendMessageToUser(PickPositionLogMessages.pickPositionAlreadyCreatedOrLoaded, EPopupType.Warning);
                    return;
                }
                
                createdPickPosition.SetPickPosition(step.StepIndex, runtimeDataForOperator.ActiveStep.StepID, scale, position, rotation);
                runtimeDataForOperator.PickPositions.Add(createdPickPosition);
            }
        }
    }
}