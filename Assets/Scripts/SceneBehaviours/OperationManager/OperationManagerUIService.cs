using Data.Entities;
using Data.Settings;
using EventSystem;
using EventSystem.Enums;
using Responses;
using SceneBehaviours.StepButtons;
using TMPro;
using UnityEngine;

namespace SceneBehaviours.OperationManager
{
    public class OperationManagerUIService : MonoBehaviour
    {
        [Header("Button Settings")]
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private Transform buttonParent;
        
        [Header("User Interface")]
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI operationNameText; 
        [SerializeField] private TextMeshProUGUI stepNumberText;
        
        private StepList steps;

        #region Listeners

        private void OnEnable()
        {
            EventManager.StepEvents.OnStepChanged.Get(EChannels.Step).AddListener(UpdatePanelInformation);
            EventManager.DatabaseEvents.OnStepsLoaded.Get(EChannels.Database).AddListener(CreateButtons);
        }

        private void OnDisable()
        {
            EventManager.StepEvents.OnStepChanged.Get(EChannels.Step).RemoveListener(UpdatePanelInformation);
            EventManager.DatabaseEvents.OnStepsLoaded.Get(EChannels.Database).RemoveListener(CreateButtons);
        }

        #endregion

        private void CreateButtons(Response<StepList> response)
        {
            // CHANGE THIS LATER
            ManagerRuntimeData.stepButtons.Clear();

            steps = response.data;
            foreach (var step in steps.Steps)
            {
                var button = Instantiate(buttonPrefab, buttonParent);
                var stepButton = button.GetComponentInChildren<StepButton>();
                
                stepButton.stepIndex = step.StepIndex;
                stepButton.stepNumberText.text = step.StepIndex.ToString();

                ManagerRuntimeData.stepButtons.Add(button);
            }
            
            UpdatePanelInformation();
        }
        
        // First-time init
        private void UpdatePanelInformation()
        {
            descriptionText.text = steps.Steps[0].Description;
            operationNameText.text = steps.Steps[0].Description;
            stepNumberText.text = $"Passo {steps.Steps[0].StepIndex.ToString()} de {steps.Steps.Count}";

            // CHANGE IT LATER TO READ THE CREATED PICK POSITIONS
            stepNumberText.text += ManagerRuntimeData.ReturnActivePickPosition()
                ? "\n" + ManagerRuntimeData.ReturnActivePickPosition().gameObject.name
                : "\nThere's no Active PickPosition...";
        }
        
        // After data's loaded
        private void UpdatePanelInformation(Response<int> response)
        {
            descriptionText.text = steps.Steps[response.data].Description;
            operationNameText.text = steps.Steps[response.data].Description;
            stepNumberText.text = $"Passo {steps.Steps[response.data].StepIndex.ToString()} de {steps.Steps.Count}";

            // CHANGE IT LATER TO READ THE CREATED PICK POSITIONS
            stepNumberText.text += ManagerRuntimeData.ReturnActivePickPosition()
                ? "\n" + ManagerRuntimeData.ReturnActivePickPosition().gameObject.name
                : "\nThere's no Active PickPosition...";
        }
    }
}