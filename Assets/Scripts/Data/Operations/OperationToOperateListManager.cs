using System.Collections.Generic;
using Data.Entities;
using Data.Methods;
using Data.Settings;
using Helper;
using KBCore.Refs;
using Scene;
using UnityEngine;

namespace Data.Operations
{
    public class OperationToOperateListManager : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Scene] private SceneTransitionManager sceneTransitionManager;
        [SerializeField, Scene] private PopupManager popupManager;
        
        [Header("Operation List"), Tooltip("The list of operations that are currently in the database.")]
        [SerializeField] private Transform operationList;
        [SerializeField] private GameObject operationPrefab;

        private async void Start()
        {
            ConnectionSettings.ConfigureAPIUrl();
            var result = await Get.GetAllOperationsAsync(popupManager); 
            
            ConfigureOperationList(result.Operations);
        }
        
        private void ConfigureOperationList(List<Operation> operationsList)
        {
            foreach (var operation in operationsList)
            {
                var operationInstance = Instantiate(operationPrefab, operationList);
                operationInstance.GetComponent<OperatorOperationListItem>().SetOperation(operation, sceneTransitionManager);
            }
        }
    }
}