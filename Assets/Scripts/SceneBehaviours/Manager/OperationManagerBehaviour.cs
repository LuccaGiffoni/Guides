using Data.Database;
using Data.Entities;
using Data.Enums;
using Data.Responses;
using Data.ScriptableObjects;
using EventSystem;
using KBCore.Refs;
using Messages;
using Services.Implementations;
using Transitions;
using UnityEngine;
using UnityEngine.Serialization;

namespace SceneBehaviours.Manager
{
    public class OperationManagerBehaviour : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Scene] private PopupService popupService;
        [SerializeField, Scene] private SceneTransitionManager sceneTransitionManager;

        [Header("Scriptable Object"), SerializeField] private RuntimeDataForManager runtimeDataForManager;
        
        #region Listeners

        private void OnEnable()
        {
            EventManager.AnchorEvents.OnAnchorLoaded.Get(EChannels.Anchor).AddListener(GetStepsForOperation);
        }

        private void OnDisable()
        {
            EventManager.AnchorEvents.OnAnchorLoaded.Get(EChannels.Anchor).RemoveListener(GetStepsForOperation);
        }

        #endregion

        private async void GetStepsForOperation(Response<OVRSpatialAnchor> receivedResponse)
        {
            var i = 1;

            while (true)
            {
                runtimeDataForManager.Steps = await Get.GetStepsForOperationAsync(runtimeDataForManager.Operation.OperationID, popupService);
                
                if (runtimeDataForManager.Steps.Steps.Count > 0)
                {
                    popupService.SendMessageToUser(DatabaseLogMessages.ReturnedSteps(runtimeDataForManager.Steps.Steps.Count), EPopupType.Info);
                    
                    var successResponse = Response<StepList>.Success(runtimeDataForManager.Steps, DatabaseLogMessages.ReturnedSteps(runtimeDataForManager.Steps.Steps.Count));
                    EventManager.DatabaseEvents.OnStepsLoaded.Get(EChannels.Database).Invoke(successResponse);
                    runtimeDataForManager.Index = 1;
                }
                else
                {
                    if (i < 4)
                    {
                        popupService.SendMessageToUser(DatabaseLogMessages.NoneStepFoundOnDatabase + $" Tentando novamente [{i}]...", EPopupType.Warning);
                        i++;
                        
                        continue;
                    }

                    popupService.SendMessageToUser(DatabaseLogMessages.NoneStepFoundOnDatabase + $" Limite de tentativas atingido!", EPopupType.Error);
                }

                break;
            }
        }
        
        public void SaveAndExit()
        {
            runtimeDataForManager.Clear();
            sceneTransitionManager.LoadSceneByIndex(0);
        }
    }
}