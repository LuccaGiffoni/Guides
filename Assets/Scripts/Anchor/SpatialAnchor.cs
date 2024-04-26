using KBCore.Refs;
using TMPro;
using UnityEngine;

namespace Anchor
{
    public class SpatialAnchor : ValidatedMonoBehaviour
    {
        [SerializeField] public OVRSpatialAnchor spatialAnchor;
        [SerializeField] public TextMeshProUGUI uuid;
        [SerializeField] public TextMeshProUGUI status;

        public void SetSpatialAnchorData(string newStatus, string newUuid)
        {
            status.text = newStatus;
            uuid.text = newUuid;
        }

        public void SetStatus(string newStatus) => status.text = newStatus;
        public void SetUuid(string newUuid) => uuid.text = newUuid;
    }
}