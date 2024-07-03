using Data.Methods;
using Data.Settings;
using Helper;
using KBCore.Refs;
using Language;
using UnityEngine;

namespace PickPositions
{
    public class PickPositionManager : ValidatedMonoBehaviour
    {
        [Header("References"), SerializeField] private PopupManager popupManager;
        [Header("References"), SerializeField] private PickPositionCreator pickPositionCreator;

        public async void DeleteActivePickPosition()
        {
            var activePickPosition = ManagerRuntimeData.ReturnActivePickPosition();
            
            if(activePickPosition)
                Destroy(activePickPosition.gameObject);
            else
            {
                popupManager.SendMessageToUser("Não há nenhuma âncora ativa para excluir." , PopupType.Info);
                return;
            }
            
            pickPositionCreator.pickPositionOnEditMode = null;
            
            var result = ManagerRuntimeData.RemoveActivePickPosition();
            if (result)
            {
                popupManager.SendMessageToUser(PickPositionLogMessages.activePickPositionRemoved, PopupType.Info);
                await Post.ClearPickPositionFromDatabase(ManagerRuntimeData.selectedStep.StepID);
            }
            else popupManager.SendMessageToUser(PickPositionLogMessages.activePickPositionNotRemoved, PopupType.Error);
        }

        public async void SaveActivePickPosition()
        {
            foreach (var pickPosition in ManagerRuntimeData.pickPositionsOnScene)
            {
                var result = await Post.SavePickPositionToDatabase(ManagerRuntimeData.steps.Steps[pickPosition.stepIndex - 1].StepID,
                    pickPosition.gameObject.transform);

                if (string.IsNullOrEmpty(result))
                {
                    popupManager.SendMessageToUser(PickPositionLogMessages.pickPositionSaved, PopupType.Info);
                }
                else popupManager.SendMessageToUser(PickPositionLogMessages.LogErrorWhileSavingPickPosition(result), PopupType.Error);
            }
        }
    }
}