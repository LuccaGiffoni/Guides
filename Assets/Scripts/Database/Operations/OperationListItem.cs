using Database.Entities;
using Database.Settings;
using UnityEngine;
using KBCore.Refs;
using Scene;
using TMPro;

namespace Database.Operations
{
    public class OperationListItem : ValidatedMonoBehaviour
    {
        private Operation _operation;
        private SceneTransitionManager _sceneTransitionManager;
        
        [Header("UI Elements"), Tooltip("These elements will be updated at runtime")]
        [SerializeField] private TextMeshProUGUI id;
        [SerializeField] private TextMeshProUGUI description;
            
        public void SetOperation(Operation operation, SceneTransitionManager sceneTransitionManager)
        {
            _operation = operation;
            _sceneTransitionManager = sceneTransitionManager;
            
            id.text = operation.OperationID.ToString();
            description.text = operation.Description;
        }

        public void SelectOperation()
        {
            ManagerRuntimeData.SaveOperation(_operation);
            _sceneTransitionManager.AutomaticallyLoadNextScene();
        }
    }
}
