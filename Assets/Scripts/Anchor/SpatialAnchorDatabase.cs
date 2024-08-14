using System;
using System.Threading.Tasks;
using Data.Database;
using Data.Enums;
using Data.Responses;
using Data.ScriptableObjects;
using EventSystem;
using KBCore.Refs;
using Messages;
using SceneBehaviours.Manager;
using Services.Implementations;
using UnityEngine;

namespace Anchor
{
    public class SpatialAnchorDatabase : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Scene] private PopupService popupService;
        [SerializeField, Scene] private OperationManagerBehaviour operationManagerBehaviour;
        
        [Header("Runtime Data"), SerializeField] private RuntimeDataForManager runtimeDataForManager;

        public async void SaveSpatialAnchor() => await SaveSpatialAnchorToDatabase();
        private async Task SaveSpatialAnchorToDatabase()
        {
            try
            {
                var task = await Post.UpdateOperationAnchorUuidAsync(runtimeDataForManager.Operation.OperationID, runtimeDataForManager.OVRSpatialAnchor.Uuid);

                if (!task) return;

                popupService.SendMessageToUser(AnchorLogMessages.anchorSavedToDatabase, EPopupType.Info);
                runtimeDataForManager.OVRSpatialAnchor.GetComponent<SpatialAnchor>().SetSpatialAnchorData(
                    AnchorLogMessages.anchorSavedToDatabase,
                    runtimeDataForManager.OVRSpatialAnchor.Uuid.ToString());

                var created = Response<OVRSpatialAnchor>.Success(runtimeDataForManager.OVRSpatialAnchor, AnchorLogMessages.anchorLocalized);
                EventManager.AnchorEvents.OnAnchorLoaded.Get(EChannels.Anchor).Invoke(created);
            }
            catch (Exception e)
            {
                popupService.SendMessageToUser(AnchorLogMessages.LogErrorWhileSavingAnchor(e.Message), EPopupType.Error);
                Debug.Log(e);
            }
        }

        public async Task ClearSpatialAnchorFromDatabase()
        {
            try
            {
                var result = await Post.ClearOperationAnchorUuidAsync(runtimeDataForManager.Operation.OperationID);

                if (result)
                {
                    popupService.SendMessageToUser(AnchorLogMessages.anchorClearedFromDatabase, EPopupType.Info);
                    runtimeDataForManager.OVRSpatialAnchor = null;
                    runtimeDataForManager.SpatialAnchor = null;
                }
            }
            catch (Exception e) { popupService.SendMessageToUser(AnchorLogMessages.LogErrorWhileClearingAnchor(e.Message), EPopupType.Error); }
        }
    }
}