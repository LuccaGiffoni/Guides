using System.Collections.Generic;
using System.Linq;
using Anchor;
using Data.Entities;
using Data.Enums;
using PickPositions.Roles;
using UnityEngine;

namespace Data.ScriptableObjects
{        
    [CreateAssetMenu(fileName = "RuntimeDataForOperator", menuName = "ScriptableObjects/RuntimeDataForOperator", order = 2)]
    public class RuntimeDataForOperator
    {
        public Operation Operation;
        public StepList Steps = new();
        public List<OperatorPickPosition> PickPositions = new();
        public List<GameObject> StepButtons = new();
        public OVRSpatialAnchor OVRSpatialAnchor;
        public SpatialAnchor SpatialAnchor;
        public EManagerState CreativeMode = EManagerState.None;
        public int Index = 1;
    
        public OperatorPickPosition ActivePickPosition => PickPositions.FirstOrDefault(x => x.stepId == Steps.Steps[Index - 1].StepID);
        public Step ActiveStep => Steps.Steps[Index];

        public void Clear()
        {
            Operation = null;
            Steps.Steps?.Clear();
            PickPositions?.Clear();
            StepButtons?.Clear();
            OVRSpatialAnchor = null;
            SpatialAnchor = null;
            CreativeMode = EManagerState.None;
            Index = 1;
        }
    }
}