using System;
using System.Threading.Tasks;
using Database.Methods;
using Database.Settings;
using Helper;
using KBCore.Refs;
using Language;
using UnityEngine;

namespace Anchor
{
    public class SpatialAnchorDatabase : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Scene] private PopupManager popupManager;
        
        public async void SaveSpatialAnchor() => await SaveSpatialAnchorToDatabase();
        private async Task SaveSpatialAnchorToDatabase()
        {
            try
            {
                var task = await Post.UpdateOperationAnchorUuidAsync(ManagerRuntimeData.selectedOperation.OperationID,
                    ManagerRuntimeData.activeAnchor.Uuid);
                
                if (!task) return;

                popupManager.SendMessageToUser(AnchorLogMessages.anchorSavedToDatabase, PopupType.Info);
                ManagerRuntimeData.activeAnchor.GetComponent<SpatialAnchor>().SetSpatialAnchorData(AnchorLogMessages.anchorSavedToDatabase,
                    ManagerRuntimeData.activeAnchor.Uuid.ToString());
            }
            catch (Exception e) { popupManager.SendMessageToUser(AnchorLogMessages.LogErrorWhileSavingAnchor(e.Message), PopupType.Error); }
        }

        public async Task ClearSpatialAnchorFromDatabase()
        {
            try
            {
                var result = await Post.ClearOperationAnchorUuidAsync(ManagerRuntimeData.selectedOperation.OperationID);

                if (result)
                {
                    popupManager.SendMessageToUser(AnchorLogMessages.anchorClearedFromDatabase, PopupType.Info);
                    ManagerRuntimeData.activeAnchor = null;
                }
            }
            catch (Exception e) { popupManager.SendMessageToUser(AnchorLogMessages.LogErrorWhileClearingAnchor(e.Message), PopupType.Error); }
        }
    }
}