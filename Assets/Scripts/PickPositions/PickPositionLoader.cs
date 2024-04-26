using System.Collections.Generic;
using Database.Settings;
using UnityEngine;

namespace PickPositions
{
    public class PickPositionLoader : MonoBehaviour
    {
        [SerializeField] private GameObject managerPickPosition;
        [SerializeField] private GameObject operatorPickPosition;

        public GameObject localLoadedPickPosition;
        
        public GameObject CreatePickPositionInstance()
        {
            if (RuntimeData.selectedStep.SX <= 0 && RuntimeData.selectedStep.SY <= 0 &&
                RuntimeData.selectedStep.SZ <= 0) return null;
                
            var position = new Vector3(RuntimeData.selectedStep.PX + RuntimeData.activeAnchor.transform.position.x, 
                RuntimeData.selectedStep.PY + RuntimeData.activeAnchor.transform.position.y, RuntimeData.selectedStep.PZ + RuntimeData.activeAnchor.transform.position.z);
            var rotation = Quaternion.Euler(RuntimeData.selectedStep.RX, RuntimeData.selectedStep.RY, RuntimeData.selectedStep.RZ);
            var scale = new Vector3(RuntimeData.selectedStep.SX, RuntimeData.selectedStep.SY, RuntimeData.selectedStep.SZ);
            
            var pickPositionInstance = Instantiate(managerPickPosition, position, rotation);
            pickPositionInstance.transform.localScale = scale;

            localLoadedPickPosition = pickPositionInstance;
            
            return pickPositionInstance;
        }

        public List<GameObject> CreateAllPickPositionsInstances()
        {
            var pickPositions = new List<GameObject>();
            
            foreach (var step in RuntimeData.steps.Steps)
            {
                // var position = new Vector3(step.PX + RuntimeData.activeAnchor.transform.position.x, 
                //     step.PY + RuntimeData.activeAnchor.transform.position.y, step.PZ + RuntimeData.activeAnchor.transform.position.z);
                // var rotation = Quaternion.Euler(step.RX, step.RY, step.RZ);
                // var scale = new Vector3(step.SX, step.SY, step.SZ);
                
                var position = new Vector3(step.PX, step.PY , step.PZ);
                var rotation = Quaternion.Euler(step.RX, step.RY, step.RZ);
                var scale = new Vector3(step.SX, step.SY, step.SZ);
            
                var pickPositionInstance = Instantiate(operatorPickPosition, position, rotation);
                pickPositionInstance.transform.localScale = scale;
                pickPositionInstance.GetComponent<PickPosition>().runtimeIndex = step.StepIndex - 1;
                
                pickPositions.Add(pickPositionInstance);
            }

            return pickPositions;
        }
    }
}