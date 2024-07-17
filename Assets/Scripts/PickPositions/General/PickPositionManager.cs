using Data.Database;
using Data.Entities;
using Data.Enums;
using Data.Runtime;
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

        public async void DeleteActivePickPosition()
        {
            var activePickPosition = ManagerRuntimeData.ReturnActivePickPosition();
            
            if(activePickPosition)
                Destroy(activePickPosition.gameObject);
            else
            {
                popupService.SendMessageToUser("Não há nenhuma âncora ativa para excluir." , EPopupType.Info);
                return;
            }
            
            pickPositionCreator.pickPositionOnEditMode = null;
            
            var result = ManagerRuntimeData.RemoveActivePickPosition();
            if (result)
            {
                popupService.SendMessageToUser(PickPositionLogMessages.activePickPositionRemoved, EPopupType.Info);
                await Post.ClearPickPositionFromDatabase(ManagerRuntimeData.selectedStep.StepID);
            }
            else popupService.SendMessageToUser(PickPositionLogMessages.activePickPositionNotRemoved, EPopupType.Error);
        }

        public async void SaveActivePickPosition()
        {
            var steps = StepList.Read(Application.persistentDataPath).Steps;
            
            foreach (var pickPosition in ManagerRuntimeData.pickPositionsOnScene)
            {
                var result = await Post.SavePickPositionToDatabase(steps[pickPosition.stepIndex - 1].StepID, pickPosition.gameObject.transform);

                if (string.IsNullOrEmpty(result)) popupService.SendMessageToUser(PickPositionLogMessages.pickPositionSaved, EPopupType.Info);
                else popupService.SendMessageToUser(PickPositionLogMessages.LogErrorWhileSavingPickPosition(result), EPopupType.Error);
            }
        }
    }
}