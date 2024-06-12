using Database.Settings;
using Helper;
using KBCore.Refs;
using Language;
using SceneBehaviours.OperationManager;
using SceneBehaviours.OperationOperator;
using UnityEngine;

namespace PickPositions
{
    public class ManagerPickPositionLoader : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField] private OperationManagerBehaviour operationManagerBehaviour;
        [SerializeField] private PickPositionCreator pickPositionCreator;
        [SerializeField, Scene] private PopupManager popupManager;
        
        [Header("Pick Positions")]
        [SerializeField] private GameObject pickPositionPrefab;

        public void CreateAllSavedPickPositions()
        {
            foreach (var step in ManagerRuntimeData.steps.Steps)
            {
                if (step.SX == 0 || step.SY == 0 || step.SZ == 0) return;
                
                var position = new Vector3(step.PX, step.PY , step.PZ);
                var rotation = Quaternion.Euler(step.RX, step.RY, step.RZ);
                var scale = new Vector3(step.SX, step.SY, step.SZ);
            
                var isPickPositionValid = Instantiate(pickPositionPrefab, position, Quaternion.identity,
                    ManagerRuntimeData.activeAnchor.transform).TryGetComponent(out ManagerPickPosition createdPickPosition);
                createdPickPosition.name = $"Step {step.StepIndex}";
                
                if (!isPickPositionValid)
                {
                    popupManager.SendMessageToUser(PickPositionLogMessages.pickPositionAlreadyCreatedOrLoaded, PopupType.Warning);
                    return;
                }

                createdPickPosition.SetPickPosition(step.StepIndex, scale, position, rotation);
                ManagerRuntimeData.pickPositionsOnScene.Add(createdPickPosition);
            }
        }
    }
}