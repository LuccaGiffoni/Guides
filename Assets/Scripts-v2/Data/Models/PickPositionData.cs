using System;
using Scripts_v2.Data.Models.Enums;
using UnityEngine;

namespace Scripts_v2.Data.Models
{
    [Serializable] public class PickPositionData
    {
        public Guid Id;
        public int StepIndex;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;
        public EUserRole UserRole;
    }
}