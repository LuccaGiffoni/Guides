using System.Collections.Generic;
using System.Linq;
using Anchor;
using Data.Entities;
using Data.Enums;
using PickPositions.Roles;
using UnityEngine;

namespace Data.ScriptableObjects
{
    [CreateAssetMenu(fileName = "RuntimeDataForManager", menuName = "ScriptableObjects/RuntimeDataForManager", order = 1)]
    public class RuntimeDataForManager : ScriptableObject
    {
        public Operation Operation;
        public StepList Steps = new();
        public List<ManagerPickPosition> PickPositions = new();
        public OVRSpatialAnchor OVRSpatialAnchor;
        public SpatialAnchor SpatialAnchor;
        public EManagerState CreativeMode = EManagerState.None;
        public int Index = 1;
        
        public ManagerPickPosition ActivePickPosition => PickPositions.FirstOrDefault(x => x.stepId == Steps.Steps[Index].StepID);
        public Step ActiveStep => Steps.Steps[Index];

        public void Clear()
        {
            Operation = null;
            Steps.Steps?.Clear();
            PickPositions?.Clear();
            OVRSpatialAnchor = null;
            SpatialAnchor = null;
            CreativeMode = EManagerState.None;
            Index = 1;
        }
    }
}