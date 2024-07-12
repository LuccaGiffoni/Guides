using System;
using System.Collections.Generic;
using Data.Entities;
using Data.Enums;
using Data.Responses;
using Data.Runtime;
using Data.Settings;
using EventSystem;
using Helper;
using KBCore.Refs;
using Messages;
using Meta.XR.BuildingBlocks;
using PickPositions;
using PickPositions.Roles;
using SceneBehaviours.Manager;
using Services.Implementations;
using UnityEngine;
using UnityEngine.Serialization;

namespace Anchor
{
    public class SpatialAnchorCore : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Self] private SpatialAnchorCoreBuildingBlock anchorCore;
        [SerializeField] private SpatialAnchorSpawnerBuildingBlock anchorSpawner;
        [SerializeField, Self] private SpatialAnchorDatabase anchorDatabase;
        [FormerlySerializedAs("popupManager")] [SerializeField, Scene] private PopupService popupService;
        [SerializeField, Scene] private OperationManagerBehaviour operationManagerBehaviour;
        [SerializeField] private ManagerPickPositionLoader managerPickPositionLoader;

        [SerializeField, Scene] private OVRCameraRig cameraRig;
        
        [Header("Anchor")]
        [SerializeField, Tooltip("This prefab will get instantiated every time the user creates a new SpatialAnchor")] public GameObject anchorPrefab;
        
        private readonly List<OVRSpatialAnchor> anchors = new();
        private Operation operation = null;

        #region Listeners

        private void OnEnable()
        {
            anchorCore.OnAnchorCreateCompleted.AddListener(HandleAnchorCreateCompleted);
            anchorCore.OnAnchorsLoadCompleted.AddListener(HandleAnchorLoadCompleted);
        }

        private void OnDisable()
        {
            anchorCore.OnAnchorCreateCompleted.RemoveListener(HandleAnchorCreateCompleted);
            anchorCore.OnAnchorsLoadCompleted.RemoveListener(HandleAnchorLoadCompleted);
        }

        #endregion
        
        private void Start()
        {
            TryLoadSpatialAnchor();
        }

        private void TryLoadSpatialAnchor()
        {
            operation = Operation.Read(Application.persistentDataPath);
            if (operation.AnchorUuid != Guid.Empty)
            {
                LoadSavedSpatialAnchorToScene();
            }
            else
            {
                popupService.SendMessageToUser(AnchorLogMessages.anchorNotFoundOnDatabase, EPopupType.Warning);
                
                // Trigger Anchor creation
                // IMPLEMENT HERE
            }
        }
        
        // Event Responses
        private void HandleAnchorCreateCompleted(OVRSpatialAnchor anchor, OVRSpatialAnchor.OperationResult result)
        {
            // REFACTOR COMPLETE
            // REFACTOR COMPLETE
            // REFACTOR COMPLETE
            if (result != OVRSpatialAnchor.OperationResult.Success) return;
            
            if (ManagerRuntimeData.selectedOperation.AnchorUuid != Guid.Empty)
            {
                popupService.SendMessageToUser(AnchorLogMessages.savedAnchorIsNotLoaded, EPopupType.Warning);
                return;
            }
            
            DeleteUnsavedSpatialAnchorsFromMemory();
            ConfigureUnsavedAnchorOnScene(anchor);
            
            popupService.SendMessageToUser(AnchorLogMessages.createdAnchorNotSavedYet, EPopupType.Info);
        }

        private void ConfigureUnsavedAnchorOnScene(OVRSpatialAnchor anchor)
        {
            ManagerRuntimeData.activeAnchor = anchor;
            var spatialAnchor = ManagerRuntimeData.activeAnchor.GetComponent<SpatialAnchor>();
            spatialAnchor.SetSpatialAnchorData(AnchorLogMessages.anchorNotSavedYet, ManagerRuntimeData.activeAnchor.Uuid.ToString());
            
            anchors.Add(anchor);
        }

        private void DeleteUnsavedSpatialAnchorsFromMemory()
        {
            foreach (var anchor in anchors) anchorCore.EraseAnchorByUuid(anchor.Uuid);
            
            anchors.Clear();
        }

        public async void DeleteSavedAnchorFromMemoryAndDatabase()
        {
            if (ManagerRuntimeData.activeAnchor != null)
            {
                await anchorDatabase.ClearSpatialAnchorFromDatabase();
                anchorCore.EraseAnchorByUuid(ManagerRuntimeData.activeAnchor.Uuid);
                ManagerRuntimeData.activeAnchor = null;
                
                popupService.SendMessageToUser(AnchorLogMessages.anchorClearedFromDatabaseAndMemory, EPopupType.Info);
            }
            else
            {
                await anchorDatabase.ClearSpatialAnchorFromDatabase();
                ManagerRuntimeData.activeAnchor = null;
                
                popupService.SendMessageToUser(AnchorLogMessages.anchorClearedFromDatabase, EPopupType.Info);
            }
        }
        
        // NOT FINISHED YET
        private async void HandleAnchorLoadCompleted(List<OVRSpatialAnchor> foundAnchors)
        {
            // Set anchor
            var ovrSpatialAnchor = foundAnchors[0];
            var spatialAnchor  = ovrSpatialAnchor.GetComponent<SpatialAnchor>();
            spatialAnchor.SetSpatialAnchorData(AnchorLogMessages.anchorLocalized, ovrSpatialAnchor.Uuid.ToString());
            
            // Event triggering and responses
            if (foundAnchors.Count <= 0)
            {
                popupService.SendMessageToUser(AnchorLogMessages.anchorNotFoundOnDevice, EPopupType.Error);
                
                // Trigger Anchor creation
                // IMPLEMENT HERE
                return;
            }
            
            var found = Response<OVRSpatialAnchor>.Success(foundAnchors[0], AnchorLogMessages.anchorLocalized);
            EventManager.AnchorEvents.OnAnchorLoaded.Get(EChannels.Anchor).Invoke(found);
        }
        
        public void LoadSavedSpatialAnchorToScene()
        {
            var guids = new List<Guid> { operation.AnchorUuid };
            anchorCore.LoadAndInstantiateAnchors(anchorPrefab, guids);
        }
    }
}