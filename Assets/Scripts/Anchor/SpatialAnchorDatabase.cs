using System;
using System.Threading.Tasks;
using Data.Database;
using Data.Enums;
using Data.Responses;
using Data.Runtime;
using Data.Settings;
using EventSystem;
using Helper;
using KBCore.Refs;
using Messages;
using SceneBehaviours.Manager;
using Services.Implementations;
using UnityEngine;
using UnityEngine.Serialization;

namespace Anchor
{
    public class SpatialAnchorDatabase : ValidatedMonoBehaviour
    {
        [FormerlySerializedAs("popupManager")]
        [Header("References")]
        [SerializeField, Scene] private PopupService popupService;
        [SerializeField, Scene] private OperationManagerBehaviour operationManagerBehaviour;
        
        public async void SaveSpatialAnchor() => await SaveSpatialAnchorToDatabase();
        private async Task SaveSpatialAnchorToDatabase()
        {
            try
            {
                var task = await Post.UpdateOperationAnchorUuidAsync(ManagerRuntimeData.selectedOperation.OperationID,
                    ManagerRuntimeData.activeAnchor.Uuid);
                
                if (!task) return;

                popupService.SendMessageToUser(AnchorLogMessages.anchorSavedToDatabase, EPopupType.Info);
                ManagerRuntimeData.activeAnchor.GetComponent<SpatialAnchor>().SetSpatialAnchorData(AnchorLogMessages.anchorSavedToDatabase,
                    ManagerRuntimeData.activeAnchor.Uuid.ToString());

                var activeAnchor = FindFirstObjectByType<OVRSpatialAnchor>();
                if (activeAnchor == null)
                {
                    popupService.SendMessageToUser(AnchorLogMessages.savedAnchorIsNotLoaded, EPopupType.Error);
                    return;
                }
                
                var created = Response<OVRSpatialAnchor>.Success(activeAnchor, AnchorLogMessages.anchorLocalized);
                EventManager.AnchorEvents.OnAnchorLoaded.Get(EChannels.Anchor).Invoke(created);
            }
            catch (Exception e) { popupService.SendMessageToUser(AnchorLogMessages.LogErrorWhileSavingAnchor(e.Message), EPopupType.Error); }
        }

        public async Task ClearSpatialAnchorFromDatabase()
        {
            try
            {
                var result = await Post.ClearOperationAnchorUuidAsync(ManagerRuntimeData.selectedOperation.OperationID);

                if (result)
                {
                    popupService.SendMessageToUser(AnchorLogMessages.anchorClearedFromDatabase, EPopupType.Info);
                    ManagerRuntimeData.activeAnchor = null;
                }
            }
            catch (Exception e) { popupService.SendMessageToUser(AnchorLogMessages.LogErrorWhileClearingAnchor(e.Message), EPopupType.Error); }
        }
    }
}