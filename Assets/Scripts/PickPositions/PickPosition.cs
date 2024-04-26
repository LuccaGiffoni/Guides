using System;
using Database.Settings;
using KBCore.Refs;
using SceneBehaviours.OperationManager;
using UnityEngine;

namespace PickPositions
{
    public class PickPosition : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Self] private Transform localTransform;
        
        [Header("Runtime Information")]
        public Vector3 position;
        public Vector3 scale;
        public Quaternion rotation;
        
        public int runtimeIndex;

        public void SetNewParametersForPickPosition(int index)
        {
            position = localTransform.position - RuntimeData.activeAnchor.transform.position;
            rotation = localTransform.localRotation;
            scale = localTransform.localScale;
            runtimeIndex = index;
        }
    }
}