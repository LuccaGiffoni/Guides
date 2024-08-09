using Data.Entities;
using Data.Enums;
using Data.Responses;
using Data.ScriptableObjects;
using EventSystem;
using KBCore.Refs;
using Messages;
using PickPositions.General;
using SceneBehaviours.Manager;
using Services.Implementations;
using UnityEngine;

namespace PickPositions.Roles
{
    public class ManagerPickPositionLoader : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Scene] private OperationManagerBehaviour operationManagerBehaviour;
        [SerializeField, Scene] private PickPositionCreator pickPositionCreator;
        [SerializeField, Scene] private PopupService popupService;
        
        [Header("Pick Positions")]
        [SerializeField] private GameObject pickPositionPrefab;

        [Header("Runtime Data"), SerializeField] private RuntimeDataForManager runtimeDataForManager;
        
        #region Listeners

        private void OnEnable() => EventManager.DatabaseEvents.OnStepsLoaded.Get(EChannels.Database).AddListener(CreateAllSavedPickPositions);
        private void OnDisable() => EventManager.DatabaseEvents.OnStepsLoaded.Get(EChannels.Database).RemoveListener(CreateAllSavedPickPositions);

        #endregion
        
        private void CreateAllSavedPickPositions(Response<StepList> response)
        {
            if (!response.isSuccess || runtimeDataForManager.OVRSpatialAnchor == null) return;
            
            foreach (var step in response.data.Steps)
            {
                if (step.SX == 0 || step.SY == 0 || step.SZ == 0) continue;

                var position = new Vector3(step.PX, step.PY , step.PZ);
                var rotation = new Quaternion(step.RX, step.RY, step.RZ, step.RW);
                var scale = new Vector3(step.SX, step.SY, step.SZ);
                
                var isPickPositionValid = Instantiate(pickPositionPrefab, position, rotation, runtimeDataForManager.OVRSpatialAnchor.transform)
                    .TryGetComponent(out ManagerPickPosition createdPickPosition);
                
                if (!isPickPositionValid)
                {
                    popupService.SendMessageToUser(PickPositionLogMessages.pickPositionAlreadyCreatedOrLoaded, EPopupType.Warning);
                    continue;
                }

                createdPickPosition.name = $"Step {step.StepIndex}";
                createdPickPosition.SetPickPosition(step.StepIndex, step.StepID, scale, position, rotation);
                
                runtimeDataForManager.PickPositions.Add(createdPickPosition);
            }
        }
    }
}