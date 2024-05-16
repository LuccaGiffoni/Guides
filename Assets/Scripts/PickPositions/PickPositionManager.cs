using Database.Methods;
using Database.Settings;
using Helper;
using KBCore.Refs;
using Language;
using UnityEngine;

namespace PickPositions
{
    public class PickPositionManager : ValidatedMonoBehaviour
    {
        [Header("References"), Scene] private PopupManager popupManager;
        
        public void DeleteActivePickPosition()
        {
            var activePickPosition = ManagerRuntimeData.ReturnActivePickPosition();
            Destroy(activePickPosition.gameObject);

            var result = ManagerRuntimeData.RemoveActivePickPosition();
            if (result) popupManager.SendMessageToUser(PickPositionLogMessages.activePickPositionRemoved, PopupType.Info);
            else popupManager.SendMessageToUser(PickPositionLogMessages.activePickPositionNotRemoved, PopupType.Error);
        }

        public async void SaveActivePickPosition()
        {
            var result = await Post.SavePickPositionToDatabase(ManagerRuntimeData.selectedStep.StepID,
                ManagerRuntimeData.ReturnActivePickPosition().gameObject.transform);
            
            if(string.IsNullOrEmpty(result)) popupManager.SendMessageToUser(PickPositionLogMessages.pickPositionSaved, PopupType.Info);
            else popupManager.SendMessageToUser(PickPositionLogMessages.LogErrorWhileSavingPickPosition(result), PopupType.Error);
        }
    }
}