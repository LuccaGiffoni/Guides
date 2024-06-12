using System;
using System.Collections.Generic;
using System.Linq;
using Scripts_v2.Data.Models;
using Scripts_v2.Data.Models.Enums;
using Scripts_v2.EventSystem;
using Scripts_v2.Utilities;
using UnityEngine;

namespace Scripts_v2.Scene.Controllers
{
    public class OperationManagerController : MonoBehaviour
    {
        [SerializeField] private GameObject anchorPrefab;
        [SerializeField] private GameObject pickPositionPrefab;
        [SerializeField] private Transform anchorParent;
        [SerializeField] private Transform pickPositionParent;

        private List<OVRSpatialAnchor> anchors = new List<OVRSpatialAnchor>();

        private void Start()
        {
            LoadSavedAnchor();
        }

        private void LoadSavedAnchor()
        {
            var savedAnchorUuid = GetSavedAnchorUuid();

            if (savedAnchorUuid != Guid.Empty)
            {
                LoadAnchor(savedAnchorUuid);
            }
        }

        private void LoadAnchor(Guid anchorUuid)
        {
            var loadedAnchor = Instantiate(anchorPrefab, anchorParent).GetComponent<OVRSpatialAnchor>();
            loadedAnchor.LoadAnchor(anchorUuid);

            anchors.Add(loadedAnchor);
        }

        private void CreateNewAnchor()
        {
            var newAnchor = Instantiate(anchorPrefab, anchorParent).GetComponent<OVRSpatialAnchor>();
            newAnchor.CreateAnchor();

            newAnchor.OnAnchorCreated += (anchor, result) =>
            {
                if (result == OVRSpatialAnchor.OperationResult.Success)
                {
                    anchors.Add(anchor);
                }
                else
                {
                    Debug.LogError("Failed to create anchor.");
                }
            };
        }

        public void CreatePickPosition(Vector3 position)
        {
            var newPickPosition = Instantiate(pickPositionPrefab, position, Quaternion.identity, pickPositionParent);
            var pickPositionData = new PickPositionData()
            {
                Position = newPickPosition.transform.position,
                Rotation = newPickPosition.transform.rotation,
                Scale = newPickPosition.transform.localScale,
                UserRole = EUserRole.Manager,
                StepIndex = selectedStep.StepIndex,
                Id = Guid.NewGuid()
            };
            
            EventManager.PickPositionEvents.OnPickPositionCreated.Get(EChannels.PickPosition).Invoke(this, pickPositionData);
        }

        public void SavePickPositions()
        {
            var pickPositionDataList = (from Transform pickPositionTransform in pickPositionParent
                select new PickPositionData
                {
                    Id = Guid.NewGuid(),
                    StepIndex = 1, // Sample value, replace with actual step index
                    Position = pickPositionTransform.position,
                    UserRole = RoleManager.IsManager ? EUserRole.Manager : EUserRole.Operator
                }).ToList();

            SavePickPositionsAsync(pickPositionDataList);
        }

        private async void SavePickPositionsAsync(List<PickPositionData> pickPositionDataList)
        {
            await DatabaseService.Instance.SavePickPositionsAsync(pickPositionDataList);

            Debug.Log("Pick positions saved successfully.");
        }

        private Guid GetSavedAnchorUuid()
        {
            return Guid.Empty;
        }
    }
}
