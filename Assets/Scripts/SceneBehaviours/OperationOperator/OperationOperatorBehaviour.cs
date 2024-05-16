using System;
using System.Threading.Tasks;
using Anchor;
using Database.Methods;
using Database.Settings;
using Helper;
using KBCore.Refs;
using PickPositions;
using Scene;
using SceneBehaviours.StepButtons;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SceneBehaviours.OperationOperator
{
    public class OperationOperatorBehaviour : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Scene] private PopupManager popupManager;
        [SerializeField] private OperatorPickPositionLoader operatorPickPositionLoader;
        [SerializeField] private SceneTransitionManager sceneTransitionManager;

        [Header("Button Settings")]
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private Transform buttonParent;
        
        [Header("User Interface")]
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI operationNameText; 
        [SerializeField] private TextMeshProUGUI stepNumberText;

        public UnityEvent onStepsReceived = new();

        public async Task GetStepsForOperation()
        {
            var receivedSteps = await Get.GetStepsForOperationAsync(OperatorRuntimeData.selectedOperation.OperationID, popupManager);

            if(receivedSteps == null || receivedSteps.Steps.Count == 0) return;
            onStepsReceived.Invoke();
            
            OperatorRuntimeData.SaveSteps(receivedSteps.Steps);
            
            CreateButtons();
        }

        private void CreateButtons()
        {
            foreach (var step in ManagerRuntimeData.steps.Steps)
            {
                var button = Instantiate(buttonPrefab, buttonParent);
                var stepButton = button.GetComponentInChildren<OperatorStepButton>();

                button.GetComponent<Button>().interactable = step.StepIndex == 1;
                
                stepButton.stepIndex = step.StepIndex;
                stepButton.stepNumberText.text = step.StepIndex.ToString();
                
                OperatorRuntimeData.stepButtons.Add(button);
            }
            
            UpdatePanelInformation();
        }
        
        public void UpdatePanelInformation()
        {
            descriptionText.text = OperatorRuntimeData.selectedStep.Description;
            operationNameText.text = OperatorRuntimeData.selectedOperation.Description;
            stepNumberText.text = $"Passo {OperatorRuntimeData.selectedStep.StepIndex.ToString()} de {ManagerRuntimeData.steps.Steps.Count}";
        }
        
        public async void UnlockNextButton()
        {
            await Task.Delay((int)OperatorRuntimeData.selectedStep.AssemblyTime * 1000);

            OperatorRuntimeData.stepButtons[OperatorRuntimeData.selectedStep.StepIndex].TryGetComponent<Button>(out var stepButton);
            stepButton.interactable = true;
            
            MoveToStep(OperatorRuntimeData.selectedStep.StepIndex);
        }
        
        public void SaveAndExit()
        {
            OperatorRuntimeData.selectedStep = null;
            OperatorRuntimeData.selectedOperation = null;
            OperatorRuntimeData.stepButtons.Clear();
            
            sceneTransitionManager.LoadSceneByIndex(0);
        }
        
        public void MoveToStep(int index)
        {
            OperatorRuntimeData.selectedStep = OperatorRuntimeData.steps.Steps[index - 1];
            UpdatePanelInformation();
        }
    }
}