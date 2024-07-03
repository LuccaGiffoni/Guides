using Data.Entities;
using Data.Settings;
using EventSystem;
using EventSystem.Enums;
using Helper;
using KBCore.Refs;
using Language;
using Responses;
using SceneBehaviours.OperationManager;
using UnityEngine;

namespace PickPositions
{
    public class ManagerPickPositionLoader : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField] private OperationManagerBehaviour operationManagerBehaviour;
        [SerializeField] private PickPositionCreator pickPositionCreator;
        [SerializeField, Scene] private PopupManager popupManager;
        
        [Header("Pick Positions")]
        [SerializeField] private GameObject pickPositionPrefab;

        #region Listeners

        private void OnEnable()
        {
            EventManager.DatabaseEvents.OnStepsLoaded.Get(EChannels.Anchor).AddListener(CreateAllSavedPickPositions);
        }

        private void OnDisable()
        {
            EventManager.DatabaseEvents.OnStepsLoaded.Get(EChannels.Anchor).RemoveListener(CreateAllSavedPickPositions);
        }

        #endregion
        
        private void CreateAllSavedPickPositions(Response<StepList> response)
        {
            // Just to assert no errors
            if (!response.isSuccess) return;
            
            foreach (var step in response.data.Steps)
            {
                if (step.SX == 0 || step.SY == 0 || step.SZ == 0) return;
                
                var position = new Vector3(step.PX, step.PY , step.PZ);
                var rotation = new Quaternion(step.RX, step.RY, step.RZ, step.RW);
                var scale = new Vector3(step.SX, step.SY, step.SZ);

                // Find and validate anchor
                var activeAnchor = FindFirstObjectByType<OVRSpatialAnchor>();
                if (activeAnchor == null)
                {
                    popupManager.SendMessageToUser(AnchorLogMessages.savedAnchorIsNotLoaded, PopupType.Error);
                    return;
                }
                
                // Create PickPosition if exists
                var isPickPositionValid = Instantiate(pickPositionPrefab, position, rotation, activeAnchor.transform)
                    .TryGetComponent(out ManagerPickPosition createdPickPosition);
                createdPickPosition.name = $"Step {step.StepIndex}";
                
                if (!isPickPositionValid)
                {
                    popupManager.SendMessageToUser(PickPositionLogMessages.pickPositionAlreadyCreatedOrLoaded, PopupType.Warning);
                    return;
                }
                
                createdPickPosition.SetPickPosition(step.StepIndex, scale, position, rotation);
                
                // Save it to static class
                // CHANGE IT TO EVENT TO SAVE ON PICK POSITION SERVICE CLASS
                ManagerRuntimeData.pickPositionsOnScene.Add(createdPickPosition);
            }
        }
    }
}