using System.Collections.Generic;
using Data.Enums;
using Data.ScriptableObjects;
using KBCore.Refs;
using SceneBehaviours.Operator;
using StepButtons;
using TMPro;
using UnityEngine;

namespace PickPositions.Roles
{
    public class OperatorPickPosition : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Scene] private OperationOperatorBehaviour operationOperatorBehaviour;

        public int stepIndex { get; private set; } = 0;
        public int stepId { get; private set; }
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

        [Header("Materials")]
        [SerializeField] private Material normal;
        [SerializeField] private Material target;
        [SerializeField] private Material wrong;
        [SerializeField] private Material right;
        
        [Header("Runtime Data"), SerializeField] private RuntimeDataForOperator runtimeDataForOperator;

        private Outline _outline;
        private const string HandTag = "Hands";

        private void Start()
        {
            operationOperatorBehaviour = FindFirstObjectByType<OperationOperatorBehaviour>();
            runtimeDataForOperator = Resources.Load<RuntimeDataForOperator>("RuntimeDataOps");

            _outline = gameObject.GetComponent<Outline>();
            
            SetInteractionState((runtimeDataForOperator.Index + 1) == stepIndex
                ? EInteractionState.Target
                : EInteractionState.Normal);
        }

        public void SetPickPosition(int index, int id, Vector3 scale, Vector3 position, Quaternion rotation)
        {
            gameObject.transform.localPosition = position;
            gameObject.transform.localRotation = rotation;
            gameObject.transform.localScale = scale;
            
            stepIndex = index;
            stepId = id;
            isSaved = false;
        }
        
        public void SetInteractionState(EInteractionState eInteractionState)
        {
            switch (eInteractionState)
            {
                case EInteractionState.Normal:
                    rend.material = normal;
                    _outline.OutlineColor = Color.grey;
                    break;
                case EInteractionState.Target:
                    rend.material = target;
                    _outline.OutlineColor = Color.blue;
                    break;
                case EInteractionState.Right:
                    rend.material = right;
                    _outline.OutlineColor = Color.green;
                    SetAudio(success, false);
                    break;
                case EInteractionState.Wrong:
                    rend.material = wrong;
                    _outline.OutlineColor = Color.red;
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

            if (runtimeDataForOperator.ActiveStep.StepIndex == stepIndex)
            {
                SetInteractionState(EInteractionState.Right);
                
                if (isAlreadyTriggered) return;

                operationOperatorBehaviour.UnlockNextButton(stepIndex);
                isAlreadyTriggered = true;
                
                // Add right
                runtimeDataForOperator.Steps.Steps[stepIndex].Success++;
            }
            else
            {
                SetInteractionState(EInteractionState.Wrong);

                // Add wrong
                runtimeDataForOperator.Steps.Steps[stepIndex].Errors++;
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.CompareTag(HandTag)) return;

            SetInteractionState(runtimeDataForOperator.ActiveStep.StepIndex == stepIndex
                ? EInteractionState.Target
                : EInteractionState.Normal);
            
            audioSource.Stop();
        }
    }
}