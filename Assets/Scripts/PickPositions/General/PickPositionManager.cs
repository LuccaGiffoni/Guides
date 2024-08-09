using System.Linq;
using Data.Database;
using Data.Entities;
using Data.Enums;
using Data.Runtime;
using Data.ScriptableObjects;
using KBCore.Refs;
using Messages;
using Services.Implementations;
using UnityEngine;
using UnityEngine.Serialization;

namespace PickPositions.General
{
    public class PickPositionManager : ValidatedMonoBehaviour
    {
        [FormerlySerializedAs("popupManager")] [Header("References"), SerializeField] private PopupService popupService;
        [Header("References"), SerializeField] private PickPositionCreator pickPositionCreator;

        [Header("Runtime Data"), SerializeField] private RuntimeDataForManager runtimeDataForManager;

        public async void DeleteActivePickPosition()
        {
            var activePickPosition = ManagerRuntimeData.ReturnActivePickPosition();
            
            if(activePickPosition)
                Destroy(activePickPosition.gameObject);
            else
            {
                popupService.SendMessageToUser("Não há nenhum PickPosition (ativo) para excluir." , EPopupType.Info);
                return;
            }
            
            pickPositionCreator.pickPositionOnEditMode = null;
            
            var result = ManagerRuntimeData.RemoveActivePickPosition();
            if (result)
            {
                popupService.SendMessageToUser(PickPositionLogMessages.activePickPositionRemoved, EPopupType.Info);
                await Post.ClearPickPositionFromDatabase(activePickPosition.stepId);
            }
            else popupService.SendMessageToUser(PickPositionLogMessages.activePickPositionNotRemoved, EPopupType.Error);
        }

        private bool isSaving = false;
        
        public async void SaveActivePickPosition()
        {
            if (isSaving) return;
            
            foreach (var pickPosition in runtimeDataForManager.PickPositions)
            {
                isSaving = true;
                
                var result = await Post.SavePickPositionToDatabase(pickPosition.stepId, pickPosition.gameObject.transform);

                if (string.IsNullOrEmpty(result)) popupService.SendMessageToUser(PickPositionLogMessages.pickPositionSaved, EPopupType.Info);
                else popupService.SendMessageToUser(PickPositionLogMessages.LogErrorWhileSavingPickPosition(result), EPopupType.Error);
            }

            isSaving = false;
        }
    }
}