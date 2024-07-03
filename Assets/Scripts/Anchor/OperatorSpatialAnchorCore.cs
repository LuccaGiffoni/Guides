using System;
using System.Collections.Generic;
using Data.Settings;
using Helper;
using KBCore.Refs;
using Language;
using Meta.XR.BuildingBlocks;
using SceneBehaviours.OperationOperator;
using UnityEngine;
using UnityEngine.Events;

namespace Anchor
{
    public class OperatorSpatialAnchorCore : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Self] private SpatialAnchorCoreBuildingBlock anchorCore;
        [SerializeField, Scene] private PopupManager popupManager;
        [SerializeField] private OperationOperatorBehaviour operationOperatorBehaviour;
        
        [Header("Anchor")]
        [SerializeField, Tooltip("This prefab will get instantiated every time the user creates a new SpatialAnchor")] public GameObject anchorPrefab;

        private readonly List<OVRSpatialAnchor> _anchors = new();

        private void Start() => LoadSavedSpatialAnchorToOperatorScene();

        private void OnEnable() => anchorCore.OnAnchorsLoadCompleted.AddListener(HandleAnchorLoadCompleted);
        private void OnDisable() => anchorCore.OnAnchorsLoadCompleted.RemoveListener(HandleAnchorLoadCompleted);

        private void LoadSavedSpatialAnchorToOperatorScene()
        {
            if(OperatorRuntimeData.selectedOperation.AnchorUuid == Guid.Empty)
            {
                popupManager.SendMessageToUser(AnchorLogMessages.anchorNotFoundOnDatabase, PopupType.Warning);
                return;
            }

            Debug.Log(OperatorRuntimeData.selectedOperation.AnchorUuid.ToString());
            var guids = new List<Guid> { OperatorRuntimeData.selectedOperation.AnchorUuid };
            
            popupManager.SendMessageToUser(AnchorLogMessages.tryingToFindAnchor, PopupType.Info);
            anchorCore.LoadAndInstantiateAnchors(anchorPrefab, guids);
        }
        
        private async void HandleAnchorLoadCompleted(List<OVRSpatialAnchor> anchors)
        {
            if(anchors.Count == 0){ Debug.Log("No anchor found");
                return;
            }
            OperatorRuntimeData.activeAnchor = anchors[0];
            var spatialAnchor = OperatorRuntimeData.activeAnchor.GetComponent<SpatialAnchor>();

            spatialAnchor.SetSpatialAnchorData(AnchorLogMessages.anchorLocalized, OperatorRuntimeData.activeAnchor.Uuid.ToString());
            
            await operationOperatorBehaviour.GetStepsForOperation();
        }
    }
}