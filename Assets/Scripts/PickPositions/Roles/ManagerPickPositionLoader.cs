using System.Collections.Generic;
using System.Linq;
using Data.Entities;
using Data.Enums;
using Data.Responses;
using Data.Runtime;
using Data.ScriptableObjects;
using EventSystem;
using KBCore.Refs;
using Messages;
using PickPositions.General;
using SceneBehaviours.Manager;
using Services.Implementations;
using UnityEngine;
using UnityEngine.Serialization;

namespace PickPositions.Roles
{
    public class ManagerPickPositionLoader : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField] private OperationManagerBehaviour operationManagerBehaviour;
        [SerializeField] private PickPositionCreator pickPositionCreator;
        [FormerlySerializedAs("popupManager")] [SerializeField, Scene] private PopupService popupService;
        
        [Header("Pick Positions")]
        [SerializeField] private GameObject pickPositionPrefab;

        [Header("Runtime Data"), SerializeField] private RuntimeDataForManager runtimeDataForManager;
        
        #region Listeners

        private void OnEnable()
        {
            EventManager.DatabaseEvents.OnStepsLoaded.Get(EChannels.Database).AddListener(CreateAllSavedPickPositions);
            EventManager.StepEvents.OnStepChanged.Get(EChannels.Step).AddListener(AssignLoadedPickPosition);
        }

        private void OnDisable()
        {
            EventManager.DatabaseEvents.OnStepsLoaded.Get(EChannels.Database).RemoveListener(CreateAllSavedPickPositions);
            EventManager.StepEvents.OnStepChanged.Get(EChannels.Step).RemoveListener(AssignLoadedPickPosition);
        }

        #endregion

        private void AssignLoadedPickPosition(Response<int> response)
        {
            var loadedPickPosition = runtimeDataForManager.PickPositions.FirstOrDefault(x => x.stepIndex == response.data);
            pickPositionCreator.pickPositionOnEditMode = loadedPickPosition;
        }
        
        private void CreateAllSavedPickPositions(Response<StepList> response)
        {
            if (!response.isSuccess) return;
            
            foreach (var step in response.data.Steps)
            {
                if (step.SX == 0 || step.SY == 0 || step.SZ == 0) continue;

                var position = new Vector3(step.PX, step.PY , step.PZ);
                var rotation = new Quaternion(step.RX, step.RY, step.RZ, step.RW);
                var scale = new Vector3(step.SX, step.SY, step.SZ);
                
                // Find and validate anchor
                var activeAnchor = FindFirstObjectByType<OVRSpatialAnchor>();
                if (activeAnchor == null)
                {
                    popupService.SendMessageToUser(AnchorLogMessages.savedAnchorIsNotLoaded, EPopupType.Error);
                    return;
                }
                
                // Create PickPosition if exists
                var isPickPositionValid = Instantiate(pickPositionPrefab, position, rotation, activeAnchor.transform)
                    .TryGetComponent(out ManagerPickPosition createdPickPosition);
                createdPickPosition.name = $"Step {step.StepIndex}";
                
                if (!isPickPositionValid)
                {
                    popupService.SendMessageToUser(PickPositionLogMessages.pickPositionAlreadyCreatedOrLoaded, EPopupType.Warning);
                    return;
                }
                
                createdPickPosition.SetPickPosition(step.StepIndex, step.StepID, scale, position, rotation);
                
                // Save it to SO
                runtimeDataForManager.PickPositions.Add(createdPickPosition);

                // Assign loaded cube to Edit Mode for first step
                if (createdPickPosition.stepIndex == 1) pickPositionCreator.pickPositionOnEditMode = createdPickPosition;
            }
        }
    }
}