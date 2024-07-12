using System.Collections.Generic;
using KBCore.Refs;
using SceneBehaviours.Manager;
using TMPro;
using UnityEngine;

namespace PickPositions.Roles
{
    public class ManagerPickPosition : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField] private OperationManagerBehaviour operationManagerBehaviour;
        
        [Header("Properties")]
        public int stepIndex { get; private set; }
        public bool isSaved { get; set; }

        [Header("Initial Scale")]
        [SerializeField, Range(0.01f, 2f)] private float initialScale;
        
        [Header("User Interface")]
        [SerializeField, Self, Tooltip("Pick Position's instance's renderer")] private Renderer rend;
        [SerializeField, Tooltip("All the TextMeshProUGUI that display OperatorPickPosition's index")] private List<TextMeshProUGUI> facesText = new();

        private void Start()
        {
            operationManagerBehaviour = FindFirstObjectByType<OperationManagerBehaviour>();
        }

        public void SetDefaultPickPosition(int index)
        {
            gameObject.transform.localScale = new Vector3(initialScale, initialScale, initialScale);
            
            stepIndex = index;
            isSaved = false;
            
            foreach (var text in facesText) text.text = stepIndex.ToString();
        }

        public void SetPickPosition(int index, Vector3 scale, Vector3 position, Quaternion rotation)
        {
            //gameObject.transform.SetLocalPositionAndRotation(position, rotation);
            gameObject.transform.localScale = scale;
            
            stepIndex = index;
            isSaved = false;
            
            foreach (var text in facesText) text.text = stepIndex.ToString();
        }
    }
}