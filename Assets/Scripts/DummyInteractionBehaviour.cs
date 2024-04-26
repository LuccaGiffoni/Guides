using System;
using KBCore.Refs;
using UnityEngine;

public class DummyInteractionBehaviour : ValidatedMonoBehaviour
{
    [SerializeField, Self] private Renderer _renderer;
    
    [Header("Materials")]
    [SerializeField] private Material defaultState;
    [SerializeField] private Material selectedState;

    private void OnCollisionEnter(Collision other)
    {
        _renderer.material = selectedState;
        Debug.Log(other.gameObject.name);
    }

    private void OnCollisionExit(Collision other)
    {
        _renderer.material = defaultState;
        Debug.Log(other.gameObject.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
    }
    
    private void OnTriggerExit(Collider other)
    {
        Debug.Log(other.gameObject.name);
    }
}