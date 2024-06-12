using TMPro;
using UnityEngine.Networking;
using UnityEngine;
using System.Collections;
using System.Globalization;
using UnityEngine.XR.Interaction.Toolkit;

public class ReferenceZone : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private XRGrabInteractable xRGrabInteractable;

    public void ChangeReferenceZoneVisibility()
    {
        if (meshRenderer.enabled)
        {
            meshRenderer.enabled = false;
            xRGrabInteractable.enabled = false;
        }
        else
        {
            meshRenderer.enabled = true;
            xRGrabInteractable.enabled = true;
        }
    }
}