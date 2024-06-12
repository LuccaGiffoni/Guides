using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Database.Methods;
using Database.Settings;
using Helper;
using KBCore.Refs;
using Language;
using PickPositions;
using Scene;
using SceneBehaviours.StepButtons;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace SceneBehaviours.OperationManager
{
    public class OperationManagerBehaviour : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Scene] private PopupManager popupManager;
        [SerializeField] private ManagerPickPositionLoader managerPickPositionLoader;
        [SerializeField] private SceneTransitionManager sceneTransitionManager;

        [Header("Button Settings")]
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private Transform buttonParent;
        
        [Header("User Interface")]
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI operationNameText; 
        [SerializeField] private TextMeshProUGUI stepNumberText;

        public async Task GetStepsForOperation()
        {
            var i = 1;

            while (true)
            {
                var localReceivedSteps = await Get.GetStepsForOperationAsync(ManagerRuntimeData.selectedOperation.OperationID, popupManager);

                if (localReceivedSteps.Steps.Count > 0)
                {
                    popupManager.SendMessageToUser(DatabaseLogMessages.ReturnedSteps(localReceivedSteps.Steps.Count), PopupType.Info);
                    ManagerRuntimeData.SaveSteps(localReceivedSteps.Steps);

                    CreateButtons();
                }
                else
                {
                    if (i < 4)
                    {
                        popupManager.SendMessageToUser(DatabaseLogMessages.NoneStepFoundOnDatabase + $" Tentando novamente [{i}]...", PopupType.Warning);
                        i++;
                        
                        continue;
                    }
                }

                break;
            }
        }

        private void CreateButtons()
        {
            ManagerRuntimeData.stepButtons.Clear();

            foreach (var step in ManagerRuntimeData.steps.Steps)
            {
                var button = Instantiate(buttonPrefab, buttonParent);
                var stepButton = button.GetComponentInChildren<StepButton>();
                
                stepButton.stepIndex = step.StepIndex;
                stepButton.stepNumberText.text = step.StepIndex.ToString();

                ManagerRuntimeData.stepButtons.Add(button);
            }
            
            UpdatePanelInformation();
        }
        
        public void UpdatePanelInformation()
        {
            descriptionText.text = ManagerRuntimeData.selectedStep.Description;
            operationNameText.text = ManagerRuntimeData.selectedOperation.Description;
            stepNumberText.text = $"Passo {ManagerRuntimeData.selectedStep.StepIndex.ToString()} de {ManagerRuntimeData.steps.Steps.Count}";

            stepNumberText.text += ManagerRuntimeData.ReturnActivePickPosition()
                ? "\n" + ManagerRuntimeData.ReturnActivePickPosition().gameObject.name
                : "\nThere's no Active PickPosition...";
        }

        public void MoveToStep(int index)
        {
            ManagerRuntimeData.selectedStep = ManagerRuntimeData.steps.Steps[index - 1];
            UpdatePanelInformation();
        }

        public void SaveAndExit()
        {
            ManagerRuntimeData.ClearData();
            sceneTransitionManager.LoadSceneByIndex(0);
        }
    }
}