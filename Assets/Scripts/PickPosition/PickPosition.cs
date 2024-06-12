using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PickPosition : MonoBehaviour
{
    public int runtimeIndex;
    public Vector3 position;
    public Vector3 scale;
    public Quaternion rotation;

    private Transform referenceZone;
    private PickPositionManager pickPositionManager;
    private TextMeshProUGUI debugger;

    public void Start()
    {
        SetPickPositionOnCreation();
    }

    public void SetPickPositionOnCreation()
    {
        referenceZone = GameObject.FindGameObjectWithTag("ReferenceZone").GetComponent<Transform>();
        pickPositionManager = GameObject.FindGameObjectWithTag("PickPositionManager").GetComponent<PickPositionManager>();
        debugger = GameObject.FindGameObjectWithTag("debugger").GetComponent<TextMeshProUGUI>();

        debugger.text += "Configurando o PicKPosition";
        pickPositionManager.runtimePickPosition = gameObject.GetComponent<PickPosition>();

        SetNewParametersForPickPosition();
    }

    public void ConfigurePickPosition()
    {
        transform.SetPositionAndRotation(position, rotation);
        transform.localScale = scale;
    }

    public void SetNewParametersForPickPosition()
    {
        position = transform.position - referenceZone.position;
        rotation = transform.localRotation;
        scale = transform.localScale;
    }
}