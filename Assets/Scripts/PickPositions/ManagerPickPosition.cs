using System.Collections.Generic;
using Database.Settings;
using KBCore.Refs;
using SceneBehaviours.OperationManager;
using SceneBehaviours.OperationOperator;
using TMPro;
using UnityEngine;

namespace PickPositions
{
    public class ManagerPickPosition : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Scene] private OperationManagerBehaviour operationManagerBehaviour;
        
        [Header("Properties")]
        public int stepIndex { get; private set; }
        public bool isSaved { get; set; }

        [Header("Initial Scale")]
        [SerializeField, Range(0.01f, 2f)] private float initialScale;
        
        [Header("User Interface")]
        [SerializeField, Self, Tooltip("Pick Position's instance's renderer")] private Renderer rend;
        [SerializeField, Tooltip("All the TextMeshProUGUI that display OperatorPickPosition's index")] private List<TextMeshProUGUI> facesText = new();


        public void SetDefaultPickPosition(int index)
        {
            gameObject.transform.localScale = new Vector3(initialScale, initialScale, initialScale);
            
            foreach (var text in facesText) text.text = stepIndex.ToString();

            stepIndex = index;
            isSaved = false;
        }
    }
}