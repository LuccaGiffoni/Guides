using System.Collections.Generic;
using Data.Enums;
using Data.Responses;
using Data.Runtime;
using Data.Settings;
using EventSystem;
using Meta.XR.BuildingBlocks;
using UnityEngine;

namespace SceneBehaviours.Manager
{
    public class OperationManagerCreativeMode : MonoBehaviour
    {
        [SerializeField] private List<GameObject> pickPositionObjects; 
        [SerializeField] private List<GameObject> anchorObjects;
        [SerializeField] private ControllerButtonsMapper defaultControllerMapper;
    
        public void SetCreativeModeForAnchor()
        {
            foreach (var objects in pickPositionObjects) objects.SetActive(false);
            foreach (var objects in anchorObjects) objects.SetActive(true);
            defaultControllerMapper.enabled = false;

            var response = Response<EManagerState>.Success(EManagerState.Anchor);
            EventManager.StepEvents.OnCreativeModeChanged.Get(EChannels.Step).Invoke(response);
        }

        public void SetCreativeModeForPickPosition()
        {
            foreach (var objects in pickPositionObjects) objects.SetActive(true);
            foreach (var objects in anchorObjects) objects.SetActive(false);
            defaultControllerMapper.enabled = false;

            var response = Response<EManagerState>.Success(EManagerState.PickPosition);
            EventManager.StepEvents.OnCreativeModeChanged.Get(EChannels.Step).Invoke(response);
        }

        public void SetNoneCreativeMode()
        {
            foreach (var objects in pickPositionObjects) objects.SetActive(false);
            foreach (var objects in anchorObjects) objects.SetActive(false);
            defaultControllerMapper.enabled = true;
            
            ManagerRuntimeData.currentCreativeMode = EManagerState.None;

            var response = Response<EManagerState>.Success(EManagerState.None);
            EventManager.StepEvents.OnCreativeModeChanged.Get(EChannels.Step).Invoke(response);
        }
    }
}