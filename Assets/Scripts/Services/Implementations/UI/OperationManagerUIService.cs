using Data.Entities;
using Data.Enums;
using Data.Responses;
using Data.ScriptableObjects;
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
        
        [Header("Runtime Data"), SerializeField] private RuntimeDataForManager runtimeDataForManager;

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
            runtimeDataForManager.StepButtons.Clear();

            foreach (var step in runtimeDataForManager.Steps.Steps)
            {
                var button = Instantiate(buttonPrefab, buttonParent);
                var stepButton = button.GetComponentInChildren<StepButton>();
                stepButton.Init(step.StepIndex);
                runtimeDataForManager.StepButtons.Add(button);
            }

            operationNameText.text = runtimeDataForManager.Operation.Description;
            descriptionText.text = runtimeDataForManager.Steps.Steps[0].Description;
            stepNumberText.text = $"Passo {runtimeDataForManager.Steps.Steps[0].StepIndex.ToString()} de {runtimeDataForManager.Steps.Steps.Count}";

            stepNumberText.text += runtimeDataForManager.ActivePickPosition
                ? "\n" + runtimeDataForManager.ActivePickPosition.gameObject.name
                : "\nThere's no Active PickPosition...";
        }
        
        public void Refresh(Response<int> response)
        {
            descriptionText.text = runtimeDataForManager.Steps.Steps[response.data - 1].Description;
            stepNumberText.text = $"Passo {runtimeDataForManager.Steps.Steps[response.data - 1].StepIndex.ToString()} de {runtimeDataForManager.Steps.Steps.Count}";

            stepNumberText.text += runtimeDataForManager.ActivePickPosition
                ? "\n" + runtimeDataForManager.ActivePickPosition.gameObject.name
                : "\nThere's no Active PickPosition...";
        }
    }
}