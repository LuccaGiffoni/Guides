using System;
using System.Collections.Generic;
using Data.Entities;
using Data.Enums;
using Data.ScriptableObjects;
using KBCore.Refs;
using Messages;
using Meta.XR.BuildingBlocks;
using SceneBehaviours.Operator;
using Services.Implementations;
using UnityEngine;

namespace Anchor
{
    public class OperatorSpatialAnchorCore : ValidatedMonoBehaviour
    {
        #region Properties

        [Header("References")]
        [SerializeField, Self] private SpatialAnchorCoreBuildingBlock anchorCore;
        [SerializeField, Scene] private PopupService popupService;
        [SerializeField] private OperationOperatorBehaviour operationOperatorBehaviour;
        
        [Header("Anchor"), SerializeField, Tooltip("This prefab will get instantiated every time the user creates a new SpatialAnchor")]
        public GameObject anchorPrefab;

        [Header("Scriptable Object"), SerializeField] private RuntimeDataForOperator runtimeDataForOperator;

        #endregion

        #region Events
        
        private void OnEnable() => anchorCore.OnAnchorsLoadCompleted.AddListener(HandleAnchorLoadCompleted);
        private void OnDisable() => anchorCore.OnAnchorsLoadCompleted.RemoveListener(HandleAnchorLoadCompleted);
        
        #endregion

        private void Start()
        {
            runtimeDataForOperator.Clear();
            runtimeDataForOperator.Operation = Operation.Read(Application.persistentDataPath);
            
            // Start process
            LoadSavedSpatialAnchorToOperatorScene();
        }

        private void LoadSavedSpatialAnchorToOperatorScene()
        {
            if(runtimeDataForOperator.Operation.AnchorUuid == Guid.Empty)
            {
                popupService.SendMessageToUser(AnchorLogMessages.anchorNotFoundOnDatabase, EPopupType.Warning);
                return;
            }

            var guids = new List<Guid> { runtimeDataForOperator.Operation.AnchorUuid };
            
            popupService.SendMessageToUser(AnchorLogMessages.tryingToFindAnchor, EPopupType.Info);
            anchorCore.LoadAndInstantiateAnchors(anchorPrefab, guids);
        }
        
        private async void HandleAnchorLoadCompleted(List<OVRSpatialAnchor> anchors)
        {
            if(anchors.Count == 0)
            {
                Debug.Log("No anchor found");
                return;
            }
            
            runtimeDataForOperator.OVRSpatialAnchor = anchors[0];
            runtimeDataForOperator.SpatialAnchor = runtimeDataForOperator.OVRSpatialAnchor.GetComponent<SpatialAnchor>();

            runtimeDataForOperator.SpatialAnchor.SetSpatialAnchorData(AnchorLogMessages.anchorLocalized, runtimeDataForOperator.OVRSpatialAnchor.Uuid.ToString());
            
            await operationOperatorBehaviour.GetStepsForOperation();
        }
    }
}