using System;
using System.Threading.Tasks;
using Data.Methods;
using Data.Settings;
using EventSystem;
using EventSystem.Enums;
using Helper;
using KBCore.Refs;
using Language;
using Responses;
using SceneBehaviours.OperationManager;
using UnityEngine;

namespace Anchor
{
    public class SpatialAnchorDatabase : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Scene] private PopupManager popupManager;
        [SerializeField, Scene] private OperationManagerBehaviour operationManagerBehaviour;
        
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

                var activeAnchor = FindFirstObjectByType<OVRSpatialAnchor>();
                if (activeAnchor == null)
                {
                    popupManager.SendMessageToUser(AnchorLogMessages.savedAnchorIsNotLoaded, PopupType.Error);
                    return;
                }
                
                var created = Response<OVRSpatialAnchor>.Success(activeAnchor, AnchorLogMessages.anchorLocalized);
                EventManager.AnchorEvents.OnAnchorLoaded.Get(EChannels.Anchor).Invoke(created);
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