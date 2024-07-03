using System;
using Data.Settings;
using KBCore.Refs;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

namespace Anchor
{
    [Serializable]
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
        
        public void Save(string persistentDataPath)
        {
            var filePath = persistentDataPath + DirectoryPaths.Anchor;
            var json = JsonConvert.SerializeObject(this);
            System.IO.File.WriteAllText(filePath, json);
        }

        public static SpatialAnchor Read(string persistentDataPath)
        {
            var filePath = persistentDataPath + DirectoryPaths.Anchor;
            var file = System.IO.File.ReadAllText(filePath);
            
            return JsonConvert.DeserializeObject<SpatialAnchor>(file);
        }
    }
}