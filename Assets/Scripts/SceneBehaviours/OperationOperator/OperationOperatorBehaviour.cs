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
using UnityEngine.UI;

namespace SceneBehaviours.OperationOperator
{
    public class OperationOperatorBehaviour : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Scene] private PopupManager popupManager;
        [SerializeField] private PickPositionLoader pickPositionLoader;
        [SerializeField] private SceneTransitionManager sceneTransitionManager;
        [SerializeField] private OperatorSpatialAnchorCore spatialAnchorCore;

        [Header("Button Settings")]
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private Transform buttonParent;
        
        [Header("User Interface")]
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI operationNameText; 
        [SerializeField] private TextMeshProUGUI stepNumberText;

        private void Start() => spatialAnchorCore.LoadSavedSpatialAnchorToOperatorScene();
        
        public async Task GetStepsForOperation()
        {
            var receivedSteps = await Get.GetStepsForOperationAsync(RuntimeData.selectedOperationToOperate.OperationID, popupManager);

            if(receivedSteps == null || receivedSteps.Steps.Count == 0) return;
            
            RuntimeData.SaveReceivedStepsToOperate(receivedSteps.Steps);
            RuntimeData.selectedStepToOperate = RuntimeData.steps.Steps[0];
            
            CreateButtons();
            UpdatePanelInformation();
        }

        private void CreateButtons()
        {
            foreach (var step in RuntimeData.steps.Steps)
            {
                var button = Instantiate(buttonPrefab, buttonParent);
                var stepButton = button.GetComponentInChildren<OperatorStepButton>();

                if (step.StepIndex == 1) button.GetComponent<Button>().interactable = true;
                else button.GetComponent<Button>().interactable = false;
                
                stepButton.stepIndex = step.StepIndex;
                stepButton.stepNumberText.text = step.StepIndex.ToString();
                
                RuntimeData.stepButtons.Add(button);
            }

            pickPositionLoader.CreateAllPickPositionsInstances();
        }
        
        public void UpdatePanelInformation()
        {
            descriptionText.text = RuntimeData.selectedStepToOperate.Description;
            operationNameText.text = RuntimeData.selectedOperationToOperate.Description;
            stepNumberText.text = $"Passo {RuntimeData.selectedStepToOperate.StepIndex.ToString()} de {RuntimeData.steps.Steps.Count}";
        }
        
        public async void UnlockNextButton()
        {
            await Task.Delay((int)RuntimeData.selectedStepToOperate.AssemblyTime * 1000);
            RuntimeData.stepButtons[RuntimeData.selectedStepToOperate.StepIndex - 1].GetComponent<Button>().interactable = true;
        }
        
        public void SaveAndExit()
        {
            RuntimeData.selectedStepToOperate = null;
            RuntimeData.selectedOperationToOperate = null;
            RuntimeData.stepButtons.Clear();
            
            sceneTransitionManager.LoadSceneByIndex(0);
        }
        
        public void MoveToStep(int index)
        {
            RuntimeData.selectedStepToOperate = RuntimeData.steps.Steps[index - 1];
            UpdatePanelInformation();
        }
    }
}