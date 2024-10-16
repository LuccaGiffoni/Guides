using Data.Database;
using Data.Enums;
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
        [Header("References")]
        [SerializeField] private PopupService popupService;
        [SerializeField] private PickPositionCreator pickPositionCreator;
        
        [Header("Runtime Data")]
        [SerializeField] private RuntimeDataForManager runtimeDataForManager;
        
        private bool isSaving = false;

        public async void DeleteActivePickPosition()
        {
            if(runtimeDataForManager.ActivePickPosition)
                Destroy(runtimeDataForManager.ActivePickPosition.gameObject);
            else
            {
                popupService.SendMessageToUser("Não há nenhum PickPosition (ativo) para excluir." , EPopupType.Info);
                return;
            }

            var result = runtimeDataForManager.PickPositions.Remove(runtimeDataForManager.ActivePickPosition);
            
            if (result)
            {
                popupService.SendMessageToUser(PickPositionLogMessages.activePickPositionRemoved, EPopupType.Info);
                await Post.ClearPickPositionFromDatabase(runtimeDataForManager.ActivePickPosition.stepId);
            }
            else popupService.SendMessageToUser(PickPositionLogMessages.activePickPositionNotRemoved, EPopupType.Error);
        }
        
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