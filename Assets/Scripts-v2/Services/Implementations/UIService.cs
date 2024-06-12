using Scripts_v2.Data.Models;
using Scripts_v2.Services.Interfaces;
using Scripts_v2.Utilities;
using TMPro;
using UnityEngine;

namespace Scripts_v2.Services.Implementations
{
    public class UIService : IUIService
    {
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI operationNameText;
        [SerializeField] private TextMeshProUGUI stepNumberText;

        public void UpdateUIForOperation(Operation operation)
        {
            operationNameText.text = operation.Description;
        }

        public void UpdateUIForStep(Step step)
        {
            descriptionText.text = step.Description;
            stepNumberText.text = $"Step {step.StepIndex}";
        }

        public void DisplayMessage(string message, PopupType type)
        {
            // Implement message display logic (e.g., using a popup manager)
        }
    }
}