using System;
using System.Collections.Generic;
using Data.Entities;
using Data.Enums;
using Data.Responses;
using Data.ScriptableObjects;
using EventSystem;
using KBCore.Refs;
using Messages;
using Meta.XR.BuildingBlocks;
using PickPositions.Roles;
using SceneBehaviours.Manager;
using Services.Implementations;
using UnityEngine;

namespace Anchor
{
    public class SpatialAnchorCore : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Self] private SpatialAnchorCoreBuildingBlock anchorCore;
        [SerializeField] private SpatialAnchorSpawnerBuildingBlock anchorSpawner;
        [SerializeField, Self] private SpatialAnchorDatabase anchorDatabase;
        [SerializeField, Scene] private PopupService popupService;
        [SerializeField, Scene] private OperationManagerBehaviour operationManagerBehaviour;
        [SerializeField] private ManagerPickPositionLoader managerPickPositionLoader;

        [SerializeField, Scene] private OVRCameraRig cameraRig;
        
        [Header("Anchor")]
        [SerializeField, Tooltip("This prefab will get instantiated every time the user creates a new SpatialAnchor")] public GameObject anchorPrefab;
        
        [Header("Scriptable Object"), SerializeField] private RuntimeDataForManager runtimeDataForManager;

        private readonly List<OVRSpatialAnchor> anchors = new();

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
        
        private void Start() => TryLoadSpatialAnchor();

        private void TryLoadSpatialAnchor()
        {
            runtimeDataForManager.Clear();
            runtimeDataForManager.Operation = Operation.Read(Application.persistentDataPath);

            if (runtimeDataForManager.Operation.AnchorUuid != Guid.Empty) LoadSavedSpatialAnchorToScene();
            else
            {
                popupService.SendMessageToUser(AnchorLogMessages.anchorNotFoundOnDatabase, EPopupType.Warning);
                EventManager.AnchorEvents.OnAnchorLoaded.Get(EChannels.Anchor).Invoke(Response<OVRSpatialAnchor>.Failure("Not found on database!"));
            }
        }
        
        // Event Responses
        private void HandleAnchorCreateCompleted(OVRSpatialAnchor anchor, OVRSpatialAnchor.OperationResult result)
        {
            if (result != OVRSpatialAnchor.OperationResult.Success) return;
            
            if (runtimeDataForManager.Operation.AnchorUuid != Guid.Empty)
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
            runtimeDataForManager.OVRSpatialAnchor = anchor;
            runtimeDataForManager.SpatialAnchor = anchor.GetComponent<SpatialAnchor>();
            
            runtimeDataForManager.SpatialAnchor.SetSpatialAnchorData(AnchorLogMessages.anchorNotSavedYet, anchor.Uuid.ToString());
            Debug.Log("Anchor set on ScriptableObject!");
            
            anchors.Add(anchor);
        }

        private void DeleteUnsavedSpatialAnchorsFromMemory()
        {
            foreach (var anchor in anchors) anchorCore.EraseAnchorByUuid(anchor.Uuid);
            anchors.Clear();
        }

        public async void DeleteSavedAnchorFromMemoryAndDatabase()
        {
            if (runtimeDataForManager.OVRSpatialAnchor != null && runtimeDataForManager.SpatialAnchor != null)
            {
                await anchorDatabase.ClearSpatialAnchorFromDatabase();
                anchorCore.EraseAnchorByUuid(runtimeDataForManager.OVRSpatialAnchor.Uuid);
                
                runtimeDataForManager.OVRSpatialAnchor = null;
                runtimeDataForManager.SpatialAnchor = null;
                
                popupService.SendMessageToUser(AnchorLogMessages.anchorClearedFromDatabaseAndMemory, EPopupType.Info);
            }
            else
            {
                await anchorDatabase.ClearSpatialAnchorFromDatabase();
                runtimeDataForManager.OVRSpatialAnchor = null;
                runtimeDataForManager.SpatialAnchor = null;
                
                popupService.SendMessageToUser(AnchorLogMessages.anchorClearedFromDatabase, EPopupType.Info);
            }
        }
        
        private void HandleAnchorLoadCompleted(List<OVRSpatialAnchor> foundAnchors)
        {
            // Set anchor
            runtimeDataForManager.OVRSpatialAnchor = foundAnchors[0];
            runtimeDataForManager.SpatialAnchor = foundAnchors[0].GetComponent<SpatialAnchor>();
            runtimeDataForManager.SpatialAnchor.SetSpatialAnchorData(AnchorLogMessages.anchorLocalized, runtimeDataForManager.OVRSpatialAnchor.Uuid.ToString());
            
            if (foundAnchors.Count <= 0)
            {
                popupService.SendMessageToUser(AnchorLogMessages.anchorNotFoundOnDevice, EPopupType.Error);
                return;
            }
            
            var found = Response<OVRSpatialAnchor>.Success(foundAnchors[0], AnchorLogMessages.anchorLocalized);
            EventManager.AnchorEvents.OnAnchorLoaded.Get(EChannels.Anchor).Invoke(found);
        }
        
        public void LoadSavedSpatialAnchorToScene()
        {
            var guids = new List<Guid> { runtimeDataForManager.Operation.AnchorUuid };
            anchorCore.LoadAndInstantiateAnchors(anchorPrefab, guids);
        }
    }
}