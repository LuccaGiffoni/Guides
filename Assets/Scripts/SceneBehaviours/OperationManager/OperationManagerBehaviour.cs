using System.Threading.Tasks;
using Database.Methods;
using Database.Settings;
using Helper;
using KBCore.Refs;
using PickPositions;
using Scene;
using SceneBehaviours.StepButtons;
using TMPro;
using UnityEngine;

namespace SceneBehaviours.OperationManager
{
    public class OperationManagerBehaviour : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Scene] private PopupManager popupManager;
        [SerializeField] private PickPositionCreator pickPositionCreator;
        [SerializeField] private PickPositionLoader pickPositionLoader;
        [SerializeField] private SceneTransitionManager sceneTransitionManager;

        [Header("Button Settings")]
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private Transform buttonParent;
        
        [Header("User Interface")]
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI operationNameText; 
        [SerializeField] private TextMeshProUGUI stepNumberText;
    
        private async void Start() => await GetStepsForOperation();
        
        private async Task GetStepsForOperation()
        {
            var receivedSteps = await Get.GetStepsForOperationAsync(RuntimeData.selectedOperation.OperationID, popupManager);

            if(receivedSteps == null || receivedSteps.Steps.Count == 0) return;
            
            RuntimeData.SaveReceivedSteps(receivedSteps.Steps);
            RuntimeData.selectedStep = RuntimeData.steps.Steps[0];
            
            CreateButtons();
            ConfigureUserInterface();
        }

        public void ConfigureUserInterface()
        {
            CreatePickPositionForStep();
            UpdatePanelInformation();
        }
        
        private void CreateButtons()
        {
            RuntimeData.stepButtons.Clear();

            foreach (var step in RuntimeData.steps.Steps)
            {
                var button = Instantiate(buttonPrefab, buttonParent);
                var stepButton = button.GetComponentInChildren<StepButton>();
                
                stepButton.stepIndex = step.StepIndex;
                stepButton.stepNumberText.text = step.StepIndex.ToString();

                RuntimeData.stepButtons.Add(button);
            }
        }
        
        public void CreatePickPositionForStep()
        {
            if (RuntimeData.activeAnchor == null) return;
            if (RuntimeData.selectedStep.PX == 0 && RuntimeData.selectedStep.PY == 0 && RuntimeData.selectedStep.PZ == 0 &&
                RuntimeData.selectedStep.RX == 0 && RuntimeData.selectedStep.RY == 0 && RuntimeData.selectedStep.RZ == 0 && RuntimeData.selectedStep.RW == 0) return;

            var pickPosition = pickPositionLoader.CreatePickPositionInstance();
            RuntimeData.activePickPosition = pickPosition;
        }
        
        private void UpdatePanelInformation()
        {
            descriptionText.text = RuntimeData.selectedStep.Description;
            operationNameText.text = RuntimeData.selectedOperation.Description;
            stepNumberText.text = $"Passo {RuntimeData.selectedStep.StepIndex.ToString()} de {RuntimeData.steps.Steps.Count}";
        }

        public void MoveToStep(int index)
        {
            RuntimeData.selectedStep = RuntimeData.steps.Steps[index - 1];
            
            pickPositionLoader.localLoadedPickPosition = null;
            Destroy(pickPositionLoader.localLoadedPickPosition);
            RuntimeData.activePickPosition = null;
            
            UpdatePanelInformation();
            CreatePickPositionForStep();
        }

        public void SaveAndExit()
        {
            RuntimeData.selectedStep = null;
            RuntimeData.selectedOperation = null;
            RuntimeData.stepButtons.Clear();
            RuntimeData.steps.Steps.Clear();
            
            sceneTransitionManager.LoadSceneByIndex(0);
        }
    }
}