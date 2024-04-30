using System;
using System.Collections.Generic;
using Database.Settings;
using Helper;
using KBCore.Refs;
using Language;
using Meta.XR.BuildingBlocks;
using SceneBehaviours.OperationOperator;
using UnityEngine;

namespace Anchor
{
    public class OperatorSpatialAnchorCore : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Self] private SpatialAnchorCoreBuildingBlock anchorCore;
        [SerializeField, Self] private SpatialAnchorDatabase anchorDatabase;
        [SerializeField] private OperationOperatorBehaviour operationOperatorBehaviour;
        [SerializeField, Scene] private PopupManager popupManager;
        
        [Header("Anchor")]
        [SerializeField, Tooltip("This prefab will get instantiated every time the user creates a new SpatialAnchor")] public GameObject anchorPrefab;
        
        private readonly List<OVRSpatialAnchor> _anchors = new();

        private void Start()
        {
            anchorCore.OnAnchorCreateCompleted.AddListener(HandleAnchorCreateCompleted);
            anchorCore.OnAnchorsLoadCompleted.AddListener(HandleAnchorLoadCompleted);
        }
        
        public void ToggleAnchorVisibility() => RuntimeData.activeAnchor.gameObject.SetActive(!RuntimeData.activeAnchor.gameObject.activeInHierarchy);

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
            anchorCore.EraseAnchorByUuid(RuntimeData.activeAnchor.Uuid);
            await anchorDatabase.ClearSpatialAnchorFromDatabase();
        }

        private async void HandleAnchorLoadCompleted(List<OVRSpatialAnchor> anchors)
        {
            // Receive first anchor found - the only one requested
            RuntimeData.activeAnchor = anchors[0];
            
            if(RuntimeData.activeAnchor == null) return;
            var spatialAnchor = RuntimeData.activeAnchor.GetComponent<SpatialAnchor>();

            spatialAnchor.SetSpatialAnchorData(AnchorLogMessages.anchorLocalized, RuntimeData.activeAnchor.Uuid.ToString()); 
            await operationOperatorBehaviour.GetStepsForOperation();
        }
        
        public void LoadSavedSpatialAnchorToOperatorScene()
        {
            if(RuntimeData.selectedOperationToOperate.AnchorUuid == Guid.Empty)
            {
                popupManager.SendMessageToUser(AnchorLogMessages.anchorNotFoundOnDatabase, PopupType.Warning);
                return;
            }
            
            var guids = new List<Guid> { RuntimeData.selectedOperationToOperate.AnchorUuid };
            anchorCore.LoadAndInstantiateAnchors(anchorPrefab, guids);
        }
    }
}