using System.Collections.Generic;
using Database.Settings;
using Helper;
using KBCore.Refs;
using SceneBehaviours.OperationOperator;
using UnityEngine;

namespace PickPositions
{
    public class InteractionManager : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private Renderer rend;
        [SerializeField]  private OperationOperatorBehaviour operationOperatorBehaviour;
        [SerializeField, Self] private PickPosition pickPosition;
        [SerializeField, Scene] private PopupManager popupManager;

        [Header("Game Objects")]
        [SerializeField] private List<Material> materials = new();

        [Header("Sounds")]
        [SerializeField, Self] private AudioSource audioSource;
        [SerializeField] private AudioClip success;
        [SerializeField] private AudioClip error;
        
        private bool isAlreadyTriggered;

        private const string HandTag = "Hands";

        private void Start()
        {
            isAlreadyTriggered = false;
            operationOperatorBehaviour = FindFirstObjectByType<OperationOperatorBehaviour>();
            popupManager = FindFirstObjectByType<PopupManager>();
            ConfigureColor();
        }

        private void ConfigureColor()
        {
            rend.material = RuntimeData.selectedStepToOperate.StepIndex == pickPosition.runtimeIndex + 1 ? materials[3] : materials[0];
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag(HandTag))
            {
                popupManager.SendMessageToUser($"Colidiu com {other.gameObject.tag}", PopupType.Warning);
            }
            
            popupManager.SendMessageToUser($"Colidiu com {other.gameObject.tag}", PopupType.Info);

            if (RuntimeData.selectedStepToOperate.StepIndex == pickPosition.runtimeIndex + 1)
            {
                rend.material = materials[1];
                audioSource.loop = false;
                audioSource.clip = success;
                audioSource.Play();

                if (!isAlreadyTriggered)
                {
                    operationOperatorBehaviour.UnlockNextButton();
                }

                isAlreadyTriggered = true;
            }
            else
            {
                rend.material = materials[2];
                audioSource.loop = true;
                audioSource.clip = error;
                audioSource.Play();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.CompareTag(HandTag)) return;
            
            rend.material = RuntimeData.selectedStepToOperate.StepIndex == pickPosition.runtimeIndex + 1 ? materials[3] : materials[0];
            audioSource.Stop();
        }
    }
}