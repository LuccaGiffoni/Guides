using Data.Entities;
using KBCore.Refs;
using Scene;
using TMPro;
using UnityEngine;

namespace Data.Operations
{
    public class OperationListItem : ValidatedMonoBehaviour
    {
        private Operation operation;
        private SceneTransitionManager sceneTransitionManager;
        
        [Header("UI Elements"), Tooltip("These elements will be updated at runtime")]
        [SerializeField] private TextMeshProUGUI id;
        [SerializeField] private TextMeshProUGUI description;
            
        public void SetOperation(Operation op, SceneTransitionManager transitionManager)
        {
            operation = op;
            sceneTransitionManager = transitionManager;
            
            id.text = op.OperationID.ToString();
            description.text = op.Description;
        }

        public void SelectOperation()
        {
            // Save to local directory
            operation.Save(Application.persistentDataPath);
            
            // Load next scene
            sceneTransitionManager.AutomaticallyLoadNextScene();
        }
    }
}
