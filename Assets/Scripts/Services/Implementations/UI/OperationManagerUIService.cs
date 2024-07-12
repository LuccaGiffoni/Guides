using Data.Entities;
using Data.Enums;
using Data.Responses;
using Data.Runtime;
using Data.Settings;
using EventSystem;
using Services.Interfaces;
using StepButtons;
using TMPro;
using UnityEngine;

namespace Services.Implementations.UI
{
    public class OperationManagerUIService : MonoBehaviour, IUIService
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
            EventManager.StepEvents.OnStepChanged.Get(EChannels.Step).AddListener(Refresh);
            EventManager.DatabaseEvents.OnStepsLoaded.Get(EChannels.Database).AddListener(Init);
        }

        private void OnDisable()
        {
            EventManager.StepEvents.OnStepChanged.Get(EChannels.Step).RemoveListener(Refresh);
            EventManager.DatabaseEvents.OnStepsLoaded.Get(EChannels.Database).RemoveListener(Init);
        }

        #endregion

        public void Init(Response<StepList> response)
        {
            // CHANGE THIS LATER
            ManagerRuntimeData.stepButtons.Clear();
            Debug.Log("Creating buttons");
            steps = response.data;
            foreach (var step in steps.Steps)
            {
                var button = Instantiate(buttonPrefab, buttonParent);
                var stepButton = button.GetComponentInChildren<StepButton>();
                
                stepButton.stepIndex = step.StepIndex;
                stepButton.stepNumberText.text = step.StepIndex.ToString();

                ManagerRuntimeData.stepButtons.Add(button);
            }

            operationNameText.text = Operation.Read(Application.persistentDataPath).Description;
            descriptionText.text = steps.Steps[0].Description;
            stepNumberText.text = $"Passo {steps.Steps[0].StepIndex.ToString()} de {steps.Steps.Count}";

            // CHANGE IT LATER TO READ THE CREATED PICK POSITIONS
            stepNumberText.text += ManagerRuntimeData.ReturnActivePickPosition()
                ? "\n" + ManagerRuntimeData.ReturnActivePickPosition().gameObject.name
                : "\nThere's no Active PickPosition...";
        }
        
        public void Refresh(Response<int> response)
        {
            descriptionText.text = steps.Steps[response.data - 1].Description;
            stepNumberText.text = $"Passo {steps.Steps[response.data - 1].StepIndex.ToString()} de {steps.Steps.Count}";

            // CHANGE IT LATER TO READ THE CREATED PICK POSITIONS
            stepNumberText.text += ManagerRuntimeData.ReturnActivePickPosition()
                ? "\n" + ManagerRuntimeData.ReturnActivePickPosition().gameObject.name
                : "\nThere's no Active PickPosition...";
        }
    }
}