using System.Threading.Tasks;
using Data.Database;
using Data.Runtime;
using Data.Settings;
using KBCore.Refs;
using PickPositions.Roles;
using Services.Implementations;
using StepButtons;
using TMPro;
using Transitions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SceneBehaviours.Operator
{
    public class OperationOperatorBehaviour : ValidatedMonoBehaviour
    {
        [FormerlySerializedAs("popupManager")]
        [Header("References")]
        [SerializeField] private PopupService popupService;
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
            var receivedSteps = await Get.GetStepsForOperationAsync(OperatorRuntimeData.selectedOperation.OperationID, popupService);

            if(receivedSteps == null || receivedSteps.Steps.Count == 0) return;
            
            OperatorRuntimeData.SaveSteps(receivedSteps.Steps);
            onStepsReceived.Invoke();
            
            CreateButtons();
        }

        private void CreateButtons()
        {
            foreach (var step in OperatorRuntimeData.steps.Steps)
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
            stepNumberText.text = $"Passo {OperatorRuntimeData.selectedStep.StepIndex.ToString()} de {OperatorRuntimeData.steps.Steps.Count}";
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