using System;
using Data.Runtime;
using KBCore.Refs;
using TMPro;
using Transitions;
using UnityEngine;
using Utils;

namespace Data.Entities
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
            try
            {
                OperatorRuntimeData.SaveOperation(operation);
                var response = operation.Save(Application.persistentDataPath, OperationType.Operator);

                if (response.Success)
                    sceneTransitionManager.AutomaticallyLoadNextScene();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }
}
