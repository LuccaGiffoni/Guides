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
            if (RuntimeData.activeAnchor != null)
            {
                RuntimeData.activeAnchor.gameObject.SetActive(!RuntimeData.activeAnchor.gameObject.activeInHierarchy);
            }
        }
        
        private void HandleAnchorCreateCompleted(OVRSpatialAnchor anchor, OVRSpatialAnchor.OperationResult result)
        {
            if (result != OVRSpatialAnchor.OperationResult.Success) return;
            
            if (RuntimeData.selectedOperation.AnchorUuid != Guid.Empty)
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
            RuntimeData.activeAnchor = anchor;
            var spatialAnchor = RuntimeData.activeAnchor.GetComponent<SpatialAnchor>();
            spatialAnchor.SetSpatialAnchorData(AnchorLogMessages.anchorNotSavedYet, RuntimeData.activeAnchor.Uuid.ToString());
            
            _anchors.Add(anchor);
        }

        private void DeleteUnsavedSpatialAnchorsFromMemory()
        {
            foreach (var anchor in _anchors) anchorCore.EraseAnchorByUuid(anchor.Uuid);
            
            _anchors.Clear();
        }

        public async void DeleteSavedAnchorFromMemoryAndDatabase()
        {
            if (RuntimeData.activeAnchor != null)
            {
                await anchorDatabase.ClearSpatialAnchorFromDatabase();
                anchorCore.EraseAnchorByUuid(RuntimeData.activeAnchor.Uuid);
                
                popupManager.SendMessageToUser(AnchorLogMessages.anchorClearedFromDatabaseAndMemory, PopupType.Info);
            }
            else
            {
                await anchorDatabase.ClearSpatialAnchorFromDatabase();
                
                popupManager.SendMessageToUser(AnchorLogMessages.anchorClearedFromDatabase, PopupType.Info);
            }
        }

        private void HandleAnchorLoadCompleted(List<OVRSpatialAnchor> anchors)
        {
            RuntimeData.activeAnchor = anchors[0];
            
            if(RuntimeData.activeAnchor == null) return;
            var spatialAnchor = RuntimeData.activeAnchor.GetComponent<SpatialAnchor>();

            if (RuntimeData.selectedOperation != null)
                operationManagerBehaviour.CreatePickPositionForStep();
            
            spatialAnchor.SetSpatialAnchorData(AnchorLogMessages.anchorLocalized, RuntimeData.activeAnchor.Uuid.ToString());
        }
        
        public void LoadSavedSpatialAnchorToScene()
        {
            if(RuntimeData.selectedOperation.AnchorUuid == Guid.Empty)
            {
                popupManager.SendMessageToUser(AnchorLogMessages.anchorNotFoundOnDatabase, PopupType.Warning);
                return;
            }
            
            var guids = new List<Guid> { RuntimeData.selectedOperation.AnchorUuid };
            anchorCore.LoadAndInstantiateAnchors(anchorPrefab, guids);
        }
    }
}