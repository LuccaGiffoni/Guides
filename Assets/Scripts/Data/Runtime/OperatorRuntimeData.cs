using System.Collections.Generic;
using Data.Entities;
using PickPositions.Roles;
using UnityEngine;

namespace Data.Runtime
{
    public static class OperatorRuntimeData
    {
        public static Operation selectedOperation { get; set; }
        public static Step selectedStep { get; set; }
        public static StepList steps { get; private set; } = new();
        public static OVRSpatialAnchor activeAnchor { get; set; }
        public static List<GameObject> stepButtons { get; set; } = new();
        public static List<OperatorPickPosition> pickPositionsOnScene { get; set; } = new();
        
        public static void SaveOperation(Operation returnedOperation)
        {
            selectedOperation = returnedOperation;
        }
        
        public static void SaveSteps(List<Step> returnedSteps)
        {
            steps.Steps = returnedSteps;
            selectedStep = returnedSteps[0];
        }

        public static void SetActiveAnchor(OVRSpatialAnchor ovrSpatialAnchor)
        {
            activeAnchor = ovrSpatialAnchor;
        }
    }
}