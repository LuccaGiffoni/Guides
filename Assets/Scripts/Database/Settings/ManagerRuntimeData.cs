using System.Collections.Generic;
using Database.Entities;
using PickPositions;
using SceneBehaviours.OperationManager;
using UnityEngine;

namespace Database.Settings
{
    public static class ManagerRuntimeData
    {
        public static Operation selectedOperation { get; set; }
        public static Step selectedStep { get; set; }
        public static StepList steps { get; private set; } = new();
        public static OVRSpatialAnchor activeAnchor { get; set; }
        public static List<GameObject> stepButtons { get; set; } = new();
        public static List<OperatorPickPosition> pickPositionsOnScene { get; set; } = new();
        public static GameObject activePickPosition { get; set; } = new();
        public static OperationManagerState currentCreativeMode { get; set; } = OperationManagerState.None;

        public static void SaveOperation(Operation returnedOperation)
        {
            selectedOperation = returnedOperation;
        }
        
        public static void SaveSteps(List<Step> returnedSteps)
        {
            steps.Steps = returnedSteps;
            selectedStep = returnedSteps[0];
        }

        public static OperatorPickPosition ReturnActivePickPosition()
        {
            var foundPickPosition = pickPositionsOnScene.Find(x =>
                x.stepIndex == selectedStep.StepIndex - 1);

            return foundPickPosition;
        }

        public static bool RemoveActivePickPosition()
        {
            var result = pickPositionsOnScene.Remove(ReturnActivePickPosition());
            return result;
        }
    }
}