using System.Collections.Generic;
using Database.Entities;
using SceneBehaviours.OperationManager;
using UnityEngine;

namespace Database.Settings
{
    public static class RuntimeData
    {
        public static Operation selectedOperation { get; set; }
        public static Operation selectedOperationToOperate { get; set; }
        public static Step selectedStep { get; set; }
        public static Step selectedStepToOperate { get; set; }
        public static StepList steps { get; private set; } = new();
        public static OVRSpatialAnchor activeAnchor { get; set; }
        public static List<GameObject> stepButtons { get; set; } = new();
        public static GameObject activePickPosition { get; set; } = new();
        public static OperationManagerState currentCreativeMode { get; set; } = OperationManagerState.None;
        
        public static void Reset()
        {
            selectedOperation = null;
            selectedStep = null;
            steps = new StepList();
        }

        public static void SaveReceivedOperation(Operation returnedOperation)
        {
            selectedOperation = returnedOperation;
        }
        
        public static void SaveReceivedOperationToOperate(Operation returnedOperation)
        {
            selectedOperationToOperate = returnedOperation;
        }
        
        public static void SaveReceivedSteps(List<Step> returnedSteps)
        {
            steps.Steps = returnedSteps;
            selectedStep = returnedSteps[0];
        }
        
        public static void SaveReceivedStepsToOperate(List<Step> returnedSteps)
        {
            steps.Steps = returnedSteps;
            selectedStepToOperate = returnedSteps[0];
        }
    }
}