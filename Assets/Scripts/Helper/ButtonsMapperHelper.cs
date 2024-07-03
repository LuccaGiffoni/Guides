using System.Collections.Generic;
using Data.Settings;
using KBCore.Refs;
using Meta.XR.BuildingBlocks;
using SceneBehaviours.OperationManager;
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

        [SerializeField] private OperationManagerCreativeMode operationManagerCreativeMode;

        private bool _isMapperVisible = false;
        
        private void OnEnable() => operationManagerCreativeMode.OnCreativeModeChange.AddListener(HandleMappersVisibility);
        private void OnDisable() => operationManagerCreativeMode.OnCreativeModeChange.RemoveListener(HandleMappersVisibility);

        public void HandleMappersVisibility()
        {
            switch (ManagerRuntimeData.currentCreativeMode)
            {
                case OperationManagerState.None:
                    ChangeVisibilityBasedOnActualVisibility(defaultMapper);
                    break;
                case OperationManagerState.Anchor:
                    ChangeVisibilityBasedOnActualVisibility(anchorMapper);
                    break;
                case OperationManagerState.PickPosition:
                    ChangeVisibilityBasedOnActualVisibility(pickPositionMapper);
                    break;
            }
        }
        
        private void ChangeVisibilityBasedOnActualVisibility(List<GameObject> mapperParent)
        {
            SetAllHelpersInactive();
            
            if (_isMapperVisible)
            {
                mapperParent.ForEach(mapper => mapper.SetActive(false));
                _isMapperVisible = false;
            }
            else
            {
                mapperParent.ForEach(mapper => mapper.SetActive(true));
                _isMapperVisible = true;
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