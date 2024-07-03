using System.Collections.Generic;
using Data.Entities;
using PickPositions;
using SceneBehaviours.OperationManager;
using UnityEngine;

namespace Data.Settings
{
    public static class ManagerRuntimeData
    {
        public static Operation selectedOperation { get; set; }
        public static Step selectedStep { get; set; }
        public static StepList steps { get; private set; } = new();
        public static OVRSpatialAnchor activeAnchor { get; set; }
        public static List<GameObject> stepButtons { get; set; } = new();
        public static List<ManagerPickPosition> pickPositionsOnScene { get; set; } = new();
        public static GameObject activePickPosition { get; set; } = new();
        public static OperationManagerState currentCreativeMode { get; set; } = OperationManagerState.None;

        public static void ClearData()
        {
            selectedOperation = null;
            selectedStep = null;
            steps.Steps.Clear();
            activeAnchor = null;
            stepButtons.Clear();
            pickPositionsOnScene.Clear();
            activePickPosition = null;
            currentCreativeMode = OperationManagerState.None;
        }
        
        public static void SaveOperation(Operation returnedOperation)
        {
            selectedOperation = returnedOperation;
            currentCreativeMode = OperationManagerState.None;
        }
        
        public static void SaveSteps(List<Step> returnedSteps)
        {
            steps.Steps = returnedSteps;
            selectedStep = returnedSteps[0];
        }

        public static ManagerPickPosition ReturnActivePickPosition()
        {
            var foundPickPosition = pickPositionsOnScene.Find(x =>
                x.stepIndex == selectedStep.StepIndex);

            return foundPickPosition;
        }

        public static bool RemoveActivePickPosition()
        {
            var result = pickPositionsOnScene.Remove(ReturnActivePickPosition());
            return result;
        }
    }
}