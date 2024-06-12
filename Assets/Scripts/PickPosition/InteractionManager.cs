using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class InteractionManager : MonoBehaviour
{
    private OperateOperation operateOperation;
    private Renderer rend;

    [Header("Game Objects")]
    [SerializeField] private PickPositionOperate pickPositionOperate;
    [SerializeField] private List<Material> materials = new();

    [Header("Sounds")]
    private AudioSource audioSource;
    [SerializeField] private AudioClip success;
    [SerializeField] private AudioClip error;

    private bool isAlreadyTriggered = false;

    private void Start()
    {
        isAlreadyTriggered = false;

        GameObject.FindGameObjectWithTag("API").TryGetComponent(out operateOperation);
        rend = GetComponent<MeshRenderer>();
        audioSource = GameObject.FindGameObjectWithTag("Comando").GetComponent<AudioSource>();

        ConfigureColor();
    }

    public void ConfigureColor()
    {
        if (operateOperation.ActualStep.StepIndex == pickPositionOperate.runtimeIndex + 1)
        {
            rend.material = materials[3];
        }
        else
        {
            rend.material = materials[0];
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Controller"))
        {
            if (operateOperation.ActualStep.StepIndex == pickPositionOperate.runtimeIndex + 1)
            {
                rend.material = materials[1];
                audioSource.loop = false;
                audioSource.clip = success;
                audioSource.Play();

                if (!isAlreadyTriggered)
                {
                    operateOperation.UnlockNextButton();
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
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Controller"))
        {
            if (operateOperation.ActualStep.StepIndex == pickPositionOperate.runtimeIndex + 1)
            {
                rend.material = materials[3];
            }
            else
            {
                rend.material = materials[0];
            }

            audioSource.Stop();
        }
    }
}