using Database.Settings;
using KBCore.Refs;
using UnityEngine;

namespace PickPositions
{
    public class PickPosition : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Self] private Transform localTransform;
        
        [Header("Runtime Information")]
        public Vector3 position;
        public Vector3 scale;
        public Quaternion rotation;
        
        public int runtimeIndex;
    }
}