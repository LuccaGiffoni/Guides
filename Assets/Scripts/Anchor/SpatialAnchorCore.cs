using System;
using System.Collections.Generic;
using Database.Settings;
using Helper;
using KBCore.Refs;
using Language;
using Meta.XR.BuildingBlocks;
using SceneBehaviours.OperationManager;
using UnityEngine;

namespace Anchor
{
    public class SpatialAnchorCore : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Self] private SpatialAnchorCoreBuildingBlock anchorCore;
        [SerializeField, Self] private SpatialAnchorDatabase anchorDatabase;
        [SerializeField, Scene] private PopupManager popupManager;
        [SerializeField, Scene] private OperationManagerBehaviour operationManagerBehaviour;
        
        [Header("Anchor")]
        [SerializeField, Tooltip("This prefab will get instantiated every time the user creates a new SpatialAnchor")] public GameObject anchorPrefab;
        
        private readonly List<OVRSpatialAnchor> _anchors = new();

        private void Start()
        {
            anchorCore.OnAnchorCreateCompleted.AddListener(HandleAnchorCreateCompleted);
            anchorCore.OnAnchorsLoadCompleted.AddListener(HandleAnchorLoadCompleted);
        }

        public void ToggleAnchorVisibility()
        {
            if (ManagerRuntimeData.activeAnchor != null)
            {
                ManagerRuntimeData.activeAnchor.gameObject.SetActive(!ManagerRuntimeData.activeAnchor.gameObject.activeInHierarchy);
            }
        }
        
        private void HandleAnchorCreateCompleted(OVRSpatialAnchor anchor, OVRSpatialAnchor.OperationResult result)
        {
            if (result != OVRSpatialAnchor.OperationResult.Success) return;
            
            if (ManagerRuntimeData.selectedOperation.AnchorUuid != Guid.Empty)
            {
                popupManager.SendMessageToUser(AnchorLogMessages.savedAnchorIsNotLoaded, PopupType.Warning);
                return;
            }
            
            DeleteUnsavedSpatialAnchorsFromMemory();
            ConfigureUnsavedAnchorOnScene(anchor);
            
            popupManager.SendMessageToUser(AnchorLogMessages.createdAnchorNotSavedYet, PopupType.Info);
        }

        private void ConfigureUnsavedAnchorOnScene(OVRSpatialAnchor anchor)
        {
            ManagerRuntimeData.activeAnchor = anchor;
            var spatialAnchor = ManagerRuntimeData.activeAnchor.GetComponent<SpatialAnchor>();
            spatialAnchor.SetSpatialAnchorData(AnchorLogMessages.anchorNotSavedYet, ManagerRuntimeData.activeAnchor.Uuid.ToString());
            
            _anchors.Add(anchor);
        }

        private void DeleteUnsavedSpatialAnchorsFromMemory()
        {
            foreach (var anchor in _anchors) anchorCore.EraseAnchorByUuid(anchor.Uuid);
            
            _anchors.Clear();
        }

        public async void DeleteSavedAnchorFromMemoryAndDatabase()
        {
            if (ManagerRuntimeData.activeAnchor != null)
            {
                await anchorDatabase.ClearSpatialAnchorFromDatabase();
                anchorCore.EraseAnchorByUuid(ManagerRuntimeData.activeAnchor.Uuid);
                ManagerRuntimeData.activeAnchor = null;
                
                popupManager.SendMessageToUser(AnchorLogMessages.anchorClearedFromDatabaseAndMemory, PopupType.Info);
            }
            else
            {
                await anchorDatabase.ClearSpatialAnchorFromDatabase();
                ManagerRuntimeData.activeAnchor = null;
                
                popupManager.SendMessageToUser(AnchorLogMessages.anchorClearedFromDatabase, PopupType.Info);
            }
        }

        private void HandleAnchorLoadCompleted(List<OVRSpatialAnchor> anchors)
        {
            ManagerRuntimeData.activeAnchor = anchors[0];
            
            if(ManagerRuntimeData.activeAnchor == null) return;
            var spatialAnchor = ManagerRuntimeData.activeAnchor.GetComponent<SpatialAnchor>();

            if (ManagerRuntimeData.selectedOperation != null)
                operationManagerBehaviour.UpdatePanelInformation();
            
            spatialAnchor.SetSpatialAnchorData(AnchorLogMessages.anchorLocalized, ManagerRuntimeData.activeAnchor.Uuid.ToString());
        }
        
        public void LoadSavedSpatialAnchorToScene()
        {
            if(ManagerRuntimeData.selectedOperation.AnchorUuid == Guid.Empty)
            {
                popupManager.SendMessageToUser(AnchorLogMessages.anchorNotFoundOnDatabase, PopupType.Warning);
                return;
            }
            
            var guids = new List<Guid> { ManagerRuntimeData.selectedOperation.AnchorUuid };
            anchorCore.LoadAndInstantiateAnchors(anchorPrefab, guids);
        }
    }
}