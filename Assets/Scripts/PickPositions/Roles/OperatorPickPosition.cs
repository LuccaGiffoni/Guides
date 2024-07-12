using System.Collections.Generic;
using Data.Enums;
using Data.Runtime;
using Data.Settings;
using KBCore.Refs;
using SceneBehaviours.Operator;
using TMPro;
using UnityEngine;

namespace PickPositions.Roles
{
    public class OperatorPickPosition : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Scene] private OperationOperatorBehaviour operationOperatorBehaviour;
        
        [Header("Properties")]
        public int stepIndex { get; private set; }
        public bool isSaved { get; set; }
        private bool isAlreadyTriggered { get; set; }
        
        [Header("Audio Settings")]
        [SerializeField, Self] private AudioSource audioSource;
        [SerializeField] private AudioClip success;
        [SerializeField] private AudioClip error;

        [Header("Initial Scale")]
        [SerializeField, Range(0.01f, 2f)] private float initialScale;
        
        [Header("User Interface")]
        [SerializeField, Self, Tooltip("Pick Position's instance's renderer")] private Renderer rend;
        [SerializeField, Tooltip("All the TextMeshProUGUI that display OperatorPickPosition's index")] private List<TextMeshProUGUI> facesText = new();

        [Header("Materials")]
        [SerializeField] private Material normal;
        [SerializeField] private Material target;
        [SerializeField] private Material wrong;
        [SerializeField] private Material right;

        private const string HandTag = "Hands";
        
        public void SetPickPosition(int index, Vector3 scale, Vector3 position, Quaternion rotation)
        {
            gameObject.transform.localPosition = position;
            gameObject.transform.rotation = rotation;
            gameObject.transform.localScale = scale;
            
            foreach (var text in facesText) text.text = stepIndex.ToString();

            stepIndex = index;
            isSaved = false;
        }
        
        private void SetInteractionState(EInteractionState eInteractionState)
        {
            switch (eInteractionState)
            {
                case EInteractionState.Normal:
                    rend.material = normal;
                    break;
                case EInteractionState.Target:
                    rend.material = target;
                    break;
                case EInteractionState.Right:
                    rend.material = right;
                    SetAudio(success, false);
                    break;
                case EInteractionState.Wrong:
                    rend.material = wrong;
                    SetAudio(error, true);
                    break;
                default:
                    rend.material = rend.material;
                    break;
            }
        }

        private void SetAudio(AudioClip clip, bool loop)
        {
            audioSource.loop = loop;
            audioSource.clip = clip;
            audioSource.Play();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag(HandTag)) return;

            if (OperatorRuntimeData.selectedStep.StepIndex == stepIndex)
            {
                SetInteractionState(EInteractionState.Right);

                if (isAlreadyTriggered) return;
                operationOperatorBehaviour.UnlockNextButton();
                isAlreadyTriggered = true;
            }
            else
            {
                SetInteractionState(EInteractionState.Wrong);
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.CompareTag(HandTag)) return;

            SetInteractionState(OperatorRuntimeData.selectedStep.StepIndex == stepIndex
                ? EInteractionState.Target
                : EInteractionState.Normal);
        }
    }
}