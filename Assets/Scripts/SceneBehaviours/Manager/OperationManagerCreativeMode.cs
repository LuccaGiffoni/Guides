using System.Collections.Generic;
using Anchor;
using Data.Enums;
using Data.Responses;
using Data.ScriptableObjects;
using EventSystem;
using Meta.XR.BuildingBlocks;
using UnityEngine;
using UnityEngine.Serialization;

namespace SceneBehaviours.Manager
{
    public class OperationManagerCreativeMode : MonoBehaviour
    {
        [SerializeField] private List<GameObject> pickPositionObjects; 
        [SerializeField] private List<GameObject> anchorObjects;
        [SerializeField] private ControllerButtonsMapper defaultControllerMapper;

        [SerializeField] private RuntimeDataForManager runtimeDataForManager;
        [SerializeField] private SpatialAnchorSpawner anchorSpawner;
    
        public void SetCreativeModeForAnchor()
        {
            foreach (var objects in pickPositionObjects) objects.SetActive(false);
            foreach (var objects in anchorObjects) objects.SetActive(true);
            defaultControllerMapper.enabled = false;

            var response = Response<EManagerState>.Success(EManagerState.Anchor);
            anchorSpawner.SetAnchorVisibility(true);
            
            runtimeDataForManager.CreativeMode = EManagerState.Anchor;
            EventManager.StepEvents.OnCreativeModeChanged.Get(EChannels.Step).Invoke(response);
        }

        public void SetCreativeModeForPickPosition()
        {
            foreach (var objects in pickPositionObjects) objects.SetActive(true);
            foreach (var objects in anchorObjects) objects.SetActive(false);
            defaultControllerMapper.enabled = false;

            var response = Response<EManagerState>.Success(EManagerState.PickPosition);
            anchorSpawner.SetAnchorVisibility(false);

            runtimeDataForManager.CreativeMode = EManagerState.PickPosition;
            EventManager.StepEvents.OnCreativeModeChanged.Get(EChannels.Step).Invoke(response);
        }

        public void SetNoneCreativeMode()
        {
            foreach (var objects in pickPositionObjects) objects.SetActive(false);
            foreach (var objects in anchorObjects) objects.SetActive(false);
            defaultControllerMapper.enabled = true;
            
            var response = Response<EManagerState>.Success(EManagerState.None);
            anchorSpawner.SetAnchorVisibility(false);

            runtimeDataForManager.CreativeMode = EManagerState.None;
            EventManager.StepEvents.OnCreativeModeChanged.Get(EChannels.Step).Invoke(response);
        }
    }
}