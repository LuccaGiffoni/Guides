using System.Collections.Generic;
using Anchor;
using Data.Entities;
using Data.Enums;
using PickPositions.Roles;
using UnityEngine;

namespace Data.ScriptableObjects
{        
    [CreateAssetMenu(fileName = "RuntimeDataForOperator", menuName = "ScriptableObjects/RuntimeDataForOperator", order = 2)]
    public class RuntimeDataForOperator : ScriptableObject
    {
        public Operation Operation;
        public StepList Steps = new();
        public List<OperatorPickPosition> PickPositions = new();
        public List<GameObject> StepButtons = new();
        public OVRSpatialAnchor OVRSpatialAnchor;
        public SpatialAnchor SpatialAnchor;
        public int Index = 0;

        public Step ActiveStep => Steps.Steps[Index];

        public void Clear()
        {
            Operation = null;
            Steps.Steps?.Clear();
            PickPositions?.Clear();
            StepButtons?.Clear();
            OVRSpatialAnchor = null;
            SpatialAnchor = null;
            Index = 1;
        }
        
        public void UpdateCubes()
        {
            foreach (var pick in PickPositions)
            {
                pick.SetInteractionState(EInteractionState.Normal);
            }
        }
        
        public void SetCubes()
        {
            foreach (var pick in PickPositions)
            {
                pick.SetInteractionState(EInteractionState.Normal);

                if (pick.stepIndex - 1 == Index)
                {
                    pick.SetInteractionState(EInteractionState.Target);
                }
            }
        }
    }
}