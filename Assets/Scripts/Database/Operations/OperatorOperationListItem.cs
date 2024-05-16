using Database.Entities;
using Database.Settings;
using UnityEngine;
using KBCore.Refs;
using Scene;
using TMPro;

namespace Database.Operations
{
    public class OperatorOperationListItem : ValidatedMonoBehaviour
    {
        private Operation operation;
        private SceneTransitionManager sceneTransitionManager;
        
        [Header("UI Elements"), Tooltip("These elements will be updated at runtime")]
        [SerializeField] private TextMeshProUGUI id;
        [SerializeField] private TextMeshProUGUI description;
            
        public void SetOperation(Operation op, SceneTransitionManager sceneTransition)
        {
            operation = op;
            sceneTransitionManager = sceneTransition;
            
            id.text = op.OperationID.ToString();
            description.text = op.Description;
        }

        public void SelectOperation()
        {
            OperatorRuntimeData.SaveOperation(operation);
            sceneTransitionManager.AutomaticallyLoadNextScene();
        }
    }
}
