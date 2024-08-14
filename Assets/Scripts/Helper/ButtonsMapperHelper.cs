using System;
using System.Collections.Generic;
using Data.Enums;
using Data.Responses;
using Data.ScriptableObjects;
using EventSystem;
using KBCore.Refs;
using Meta.XR.BuildingBlocks;
using UnityEngine;

namespace Helper
{
    public class ButtonsMapperHelper : ValidatedMonoBehaviour
    {
        [SerializeField] private SpatialAnchorSpawnerBuildingBlock spatialAnchorSpawnerBuildingBlock;
        
        [Header("Buttons Mapper")]
        [SerializeField, Tooltip("List for all the mappers for the Anchor's features.")] private List<GameObject> anchorMapper = new();
        [SerializeField, Tooltip("List for all the mappers for the Pick Position's features.")] private List<GameObject> pickPositionMapper = new();
        [SerializeField, Tooltip("List for all the mappers for default features.")] private List<GameObject> defaultMapper = new();

        [Header("Runtime Data"), SerializeField] private RuntimeDataForManager runtimeDataForManager;

        private bool isMapperVisible;
        
        private void OnEnable() => EventManager.StepEvents.OnCreativeModeChanged.Get(EChannels.Step).AddListener(HandleMappersVisibility);
        private void OnDisable() => EventManager.StepEvents.OnCreativeModeChanged.Get(EChannels.Step).RemoveListener(HandleMappersVisibility);

        public void HandleVisibility()
        {
            switch (runtimeDataForManager.CreativeMode)
            {
                case EManagerState.None:
                    ChangeVisibilityBasedOnActualVisibility(defaultMapper);
                    break;
                case EManagerState.Anchor:
                    ChangeVisibilityBasedOnActualVisibility(anchorMapper);
                    break;
                case EManagerState.PickPosition:
                    ChangeVisibilityBasedOnActualVisibility(pickPositionMapper);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void HandleMappersVisibility(Response<EManagerState> response)
        {
            if (!response.isSuccess) return;
            
            switch (response.data)
            {
                case EManagerState.None:
                    ChangeVisibilityBasedOnActualVisibility(defaultMapper);
                    break;
                case EManagerState.Anchor:
                    ChangeVisibilityBasedOnActualVisibility(anchorMapper);
                    break;
                case EManagerState.PickPosition:
                    ChangeVisibilityBasedOnActualVisibility(pickPositionMapper);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void ChangeVisibilityBasedOnActualVisibility(List<GameObject> mapperParent)
        {
            SetAllHelpersInactive();
            
            if (isMapperVisible)
            {
                mapperParent.ForEach(mapper => mapper.SetActive(false));
                isMapperVisible = false;
            }
            else
            {
                mapperParent.ForEach(mapper => mapper.SetActive(true));
                isMapperVisible = true;
            }
        }
        
        private void SetAllHelpersInactive()
        {
            anchorMapper.ForEach(mapper => mapper.SetActive(false));
            pickPositionMapper.ForEach(mapper => mapper.SetActive(false));
            defaultMapper.ForEach(mapper => mapper.SetActive(false));
        }
    }
}