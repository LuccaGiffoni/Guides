using Data.Entities;
using Data.Methods;
using Data.Settings;
using EventSystem;
using EventSystem.Enums;
using Helper;
using KBCore.Refs;
using Language;
using PickPositions;
using Responses;
using Scene;
using TMPro;
using UnityEngine;

namespace SceneBehaviours.OperationManager
{
    public class OperationManagerBehaviour : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Scene] private PopupManager popupManager;
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
                steps = await Get.GetStepsForOperationAsync(Operation.Read(Application.persistentDataPath).OperationID, popupManager);
                
                if (steps.Steps.Count > 0)
                {
                    popupManager.SendMessageToUser(DatabaseLogMessages.ReturnedSteps(steps.Steps.Count), PopupType.Info);
                    steps.Save(Application.persistentDataPath);

                    var successResponse = Response<StepList>.Success(steps, DatabaseLogMessages.ReturnedSteps(steps.Steps.Count));
                    EventManager.DatabaseEvents.OnStepsLoaded.Get(EChannels.Database).Invoke(successResponse);
                }
                else
                {
                    if (i < 4)
                    {
                        popupManager.SendMessageToUser(DatabaseLogMessages.NoneStepFoundOnDatabase + $" Tentando novamente [{i}]...", PopupType.Warning);
                        i++;
                        
                        continue;
                    }

                    popupManager.SendMessageToUser(DatabaseLogMessages.NoneStepFoundOnDatabase + $" Limite de tentativas atingido!", PopupType.Error);
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