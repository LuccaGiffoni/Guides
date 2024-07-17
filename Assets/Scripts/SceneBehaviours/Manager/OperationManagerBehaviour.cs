using Data.Database;
using Data.Entities;
using Data.Enums;
using Data.Responses;
using Data.Runtime;
using Data.Settings;
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
        [FormerlySerializedAs("popupManager")]
        [Header("References")]
        [SerializeField, Scene] private PopupService popupService;
        [SerializeField, Scene] private SceneTransitionManager sceneTransitionManager;

        private StepList steps;
        
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
                Debug.Log("Reading steps...");
                steps = await Get.GetStepsForOperationAsync(Operation.Read(Application.persistentDataPath).OperationID, popupService);
                
                if (steps.Steps.Count > 0)
                {
                    popupService.SendMessageToUser(DatabaseLogMessages.ReturnedSteps(steps.Steps.Count), EPopupType.Info);
                    steps.Save(Application.persistentDataPath);

                    var successResponse = Response<StepList>.Success(steps, DatabaseLogMessages.ReturnedSteps(steps.Steps.Count));
                    EventManager.DatabaseEvents.OnStepsLoaded.Get(EChannels.Database).Invoke(successResponse);
                    
                    Debug.Log("Steps loaded and event triggered!");
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
            ManagerRuntimeData.ClearData();
            sceneTransitionManager.LoadSceneByIndex(0);
        }
    }
}