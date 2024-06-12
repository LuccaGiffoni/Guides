using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PickPositionOperate : MonoBehaviour
{
    public int runtimeIndex;
    public Vector3 position;
    public Vector3 scale;
    public Quaternion rotation;

    public string description;

    private Transform referenceZone;
    private TextMeshProUGUI debugger;

    public void Start()
    {
        SetPickPositionOnCreation();
    }

    public void SetPickPositionOnCreation()
    {
        referenceZone = GameObject.FindGameObjectWithTag("ReferenceZone").GetComponent<Transform>();
        debugger = GameObject.FindGameObjectWithTag("debugger").GetComponent<TextMeshProUGUI>();

        debugger.text += "Configurando o PicKPosition";

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

        debugger.text += $"PickPosition: {position}, {rotation} & {scale}";
    }

    public void SetDescription(string description)
    {
        this.description = description;
    }
}