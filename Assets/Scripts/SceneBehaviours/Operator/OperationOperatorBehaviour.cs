using System.Threading.Tasks;
using Data.Database;
using Data.ScriptableObjects;
using KBCore.Refs;
using PickPositions.Roles;
using Services.Implementations;
using StepButtons;
using TMPro;
using Transitions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SceneBehaviours.Operator
{
    public class OperationOperatorBehaviour : ValidatedMonoBehaviour
    {
        #region Properties
        
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

        [Header("Scriptable Object"), SerializeField] private RuntimeDataForOperator runtimeDataForOperator;

        public UnityEvent onStepsReceived = new();

        #endregion
        
        public async Task GetStepsForOperation()
        {
            runtimeDataForOperator.Index = 0;
            runtimeDataForOperator.StepButtons.Clear();
            runtimeDataForOperator.Steps.Steps.Clear();
            
            var receivedSteps = await Get.GetStepsForOperationAsync(runtimeDataForOperator.Operation.OperationID, popupService);

            if(receivedSteps == null || receivedSteps.Steps.Count == 0) return;
            
            runtimeDataForOperator.Steps.Steps = receivedSteps.Steps;
            onStepsReceived.Invoke();
            
            CreateButtons();
        }

        private void CreateButtons()
        {
            foreach (var step in runtimeDataForOperator.Steps.Steps)
            {
                var button = Instantiate(buttonPrefab, buttonParent);
                var stepButton = button.GetComponentInChildren<OperatorStepButton>();

                button.GetComponent<Button>().interactable = step.StepIndex == 1;
                
                stepButton.stepIndex = step.StepIndex;
                stepButton.stepNumberText.text = step.StepIndex.ToString();
                
                runtimeDataForOperator.StepButtons.Add(button);
            }
            
            UpdatePanelInformation();
        }
        
        private void UpdatePanelInformation()
        {
            descriptionText.text = runtimeDataForOperator.ActiveStep.Description;
            operationNameText.text = runtimeDataForOperator.Operation.Description;
            stepNumberText.text = $"Passo {runtimeDataForOperator.ActiveStep.StepIndex.ToString()} de {runtimeDataForOperator.Steps.Steps.Count}";
        }
        
        public async void UnlockNextButton(int stepIndex)
        {
            await Task.Delay((int)runtimeDataForOperator.ActiveStep.AssemblyTime * 1000);

            if (stepIndex < runtimeDataForOperator.StepButtons.Count)
            {
                runtimeDataForOperator.StepButtons[stepIndex].TryGetComponent<Button>(out var stepButton);
                stepButton.interactable = true;

                MoveToStep(stepIndex);
            }
            else
            {
                // foreach (var step in runtimeDataForOperator.Steps.Steps)
                //     await Post.AddOperatorMetrics(step.StepID, step.Errors, step.Success);
                
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        
        public void SaveAndExit()
        {
            runtimeDataForOperator.Clear();
            sceneTransitionManager.LoadSceneByIndex(0);
        }
        
        public void MoveToStep(int index)
        {
            runtimeDataForOperator.Index = index;
            runtimeDataForOperator.SetCubes();
            
            UpdatePanelInformation();
        }
    }
}