using System.Linq;
using Data.Enums;
using Data.ScriptableObjects;
using KBCore.Refs;
using SceneBehaviours.Operator;
using UnityEngine;

namespace PickPositions.Roles
{
    public class OperatorPickPosition : ValidatedMonoBehaviour
    {
        #region Fields and Properties

        [Header("References")]
        [SerializeField, Scene] private OperationOperatorBehaviour operationOperatorBehaviour;

        public int StepIndex { get; private set; } = 0;
        public int StepId { get; private set; }
        public bool IsSaved { get; set; }
        private bool IsAlreadyTriggered { get; set; }

        [Header("Audio Settings")]
        [SerializeField, Self] private AudioSource audioSource;
        [SerializeField] private AudioClip successClip;
        [SerializeField] private AudioClip errorClip;

        [Header("Initial Scale")]
        [SerializeField, Range(0.01f, 2f)] private float initialScale;

        [Header("User Interface")]
        [SerializeField, Self, Tooltip("Pick Position's instance's renderer")] private Renderer renderer;

        [Header("Materials")]
        [SerializeField] private Material normalMaterial;
        [SerializeField] private Material targetMaterial;
        [SerializeField] private Material wrongMaterial;
        [SerializeField] private Material rightMaterial;

        [Header("Runtime Data")]
        [SerializeField] private RuntimeDataForOperator runtimeData;

        private Outline outline;
        private const string HandTag = "Hands";

        #endregion

        #region Unity Lifecycle

        private void Start()
        {
            InitializeComponents();
            SetInitialInteractionState();
        }

        #endregion

        #region Public Methods

        public void SetPickPosition(int index, int id, Vector3 scale, Vector3 position, Quaternion rotation)
        {
            transform.localPosition = position;
            transform.localRotation = rotation;
            transform.localScale = scale;

            StepIndex = index;
            StepId = id;
            IsSaved = false;
        }

        public void SetInteractionState(EInteractionState state)
        {
            switch (state)
            {
                case EInteractionState.Normal:
                    ApplyMaterialAndOutline(normalMaterial, Color.grey);
                    break;
                case EInteractionState.Target:
                    ApplyMaterialAndOutline(targetMaterial, Color.blue);
                    break;
                case EInteractionState.Right:
                    ApplyMaterialAndOutline(rightMaterial, Color.green);
                    PlayAudio(successClip, loop: false);
                    break;
                case EInteractionState.Wrong:
                    ApplyMaterialAndOutline(wrongMaterial, Color.red);
                    PlayAudio(errorClip, loop: true);
                    break;
                default:
                    Debug.LogWarning($"Unhandled EInteractionState: {state}");
                    break;
            }
        }

        #endregion

        #region Private Methods

        private void InitializeComponents()
        {
            operationOperatorBehaviour = FindFirstObjectByType<OperationOperatorBehaviour>();
            runtimeData = Resources.Load<RuntimeDataForOperator>("RuntimeDataOps");
            outline = GetComponent<Outline>();
        }

        private void SetInitialInteractionState()
        {
            if (!runtimeData.ActiveStep.CopyHologramFromStep.HasValue) return;
            
            var isTargetState = IsNextStepTarget(runtimeData.ActiveStep.CopyHologramFromStep.Value);
            SetInteractionState(isTargetState ? EInteractionState.Target : EInteractionState.Normal);
        }

        private bool IsNextStepTarget(int hologramCopyStep)
        {
            if (hologramCopyStep == 0) return runtimeData.Index + 1 == StepIndex;
            
            var stepToCopy = runtimeData.Steps.Steps.FirstOrDefault(x => x.StepIndex == hologramCopyStep);
            return stepToCopy != null && stepToCopy.StepIndex + 1 == StepIndex;
        }

        private void ApplyMaterialAndOutline(Material material, Color outlineColor)
        {
            renderer.material = material;
            outline.OutlineColor = outlineColor;
        }

        private void PlayAudio(AudioClip clip, bool loop)
        {
            audioSource.loop = loop;
            audioSource.clip = clip;
            audioSource.Play();
        }

        #endregion

        #region Event Handlers

        private void OnTriggerEnter(Collider other)
        {
            if (!IsHand(other)) return;

            if (runtimeData.ActiveStep.StepIndex == StepIndex)
            {
                HandleCorrectStep();
            }
            else
            {
                HandleIncorrectStep();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsHand(other)) return;

            SetInitialInteractionState();
            audioSource.Stop();
        }

        private bool IsHand(Collider other) => other.CompareTag(HandTag);

        private void HandleCorrectStep()
        {
            SetInteractionState(EInteractionState.Right);

            if (IsAlreadyTriggered) return;

            operationOperatorBehaviour.UnlockNextButton(StepIndex);
            IsAlreadyTriggered = true;

            runtimeData.Steps.Steps[StepIndex].Success++;
        }

        private void HandleIncorrectStep()
        {
            SetInteractionState(EInteractionState.Wrong);
            runtimeData.Steps.Steps[StepIndex].Errors++;
        }

        #endregion
    }
}
