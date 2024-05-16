using System.Collections.Generic;
using Database.Settings;
using Meta.XR.BuildingBlocks;
using UnityEngine;
using UnityEngine.Events;

namespace SceneBehaviours.OperationManager
{
    public class OperationManagerCreativeMode : MonoBehaviour
    {
        [SerializeField] private List<GameObject> pickPositionObjects; 
        [SerializeField] private List<GameObject> anchorObjects;
        [SerializeField] private ControllerButtonsMapper defaultControllerMapper;
    
        public UnityEvent OnCreativeModeChange { get; set; } = new UnityEvent();

        public void SetCreativeModeForAnchor()
        {
            foreach (var objects in pickPositionObjects) objects.SetActive(false);
            foreach (var objects in anchorObjects) objects.SetActive(true);
            defaultControllerMapper.enabled = false;
            
            ManagerRuntimeData.currentCreativeMode = OperationManagerState.Anchor;
            
            OnCreativeModeChange?.Invoke();
        }

        public void SetCreativeModeForPickPosition()
        {
            foreach (var objects in pickPositionObjects) objects.SetActive(true);
            foreach (var objects in anchorObjects) objects.SetActive(false);
            defaultControllerMapper.enabled = false;

            ManagerRuntimeData.currentCreativeMode = OperationManagerState.PickPosition;

            OnCreativeModeChange?.Invoke();
        }

        public void SetNoneCreativeMode()
        {
            foreach (var objects in pickPositionObjects) objects.SetActive(false);
            foreach (var objects in anchorObjects) objects.SetActive(false);
            defaultControllerMapper.enabled = true;
            
            ManagerRuntimeData.currentCreativeMode = OperationManagerState.None;

            OnCreativeModeChange?.Invoke(); 
        }
    }
}