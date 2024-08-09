using System.Collections.Generic;
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

        [FormerlySerializedAs("runtimeData")] [SerializeField] private RuntimeDataForManager runtimeDataForManager;
    
        public void SetCreativeModeForAnchor()
        {
            foreach (var objects in pickPositionObjects) objects.SetActive(false);
            foreach (var objects in anchorObjects) objects.SetActive(true);
            defaultControllerMapper.enabled = false;

            var response = Response<EManagerState>.Success(EManagerState.Anchor);
            EventManager.StepEvents.OnCreativeModeChanged.Get(EChannels.Step).Invoke(response);
            
            runtimeDataForManager.CreativeMode = EManagerState.Anchor;
        }

        public void SetCreativeModeForPickPosition()
        {
            foreach (var objects in pickPositionObjects) objects.SetActive(true);
            foreach (var objects in anchorObjects) objects.SetActive(false);
            defaultControllerMapper.enabled = false;

            var response = Response<EManagerState>.Success(EManagerState.PickPosition);
            EventManager.StepEvents.OnCreativeModeChanged.Get(EChannels.Step).Invoke(response);
            
            runtimeDataForManager.CreativeMode = EManagerState.PickPosition;
        }

        public void SetNoneCreativeMode()
        {
            foreach (var objects in pickPositionObjects) objects.SetActive(false);
            foreach (var objects in anchorObjects) objects.SetActive(false);
            defaultControllerMapper.enabled = true;
            
            var response = Response<EManagerState>.Success(EManagerState.None);
            EventManager.StepEvents.OnCreativeModeChanged.Get(EChannels.Step).Invoke(response);
            
            runtimeDataForManager.CreativeMode = EManagerState.None;
        }
    }
}