using System.Collections.Generic;
using Data.Entities;
using Data.Enums;
using PickPositions.Roles;
using UnityEngine;

namespace Data.Runtime
{
    public static class ManagerRuntimeData
    {
        public static Operation selectedOperation { get; set; }
        public static Step selectedStep { get; set; }
        public static StepList steps { get; set; } = new();
        public static OVRSpatialAnchor activeAnchor { get; set; }
        public static List<GameObject> stepButtons { get; set; } = new();
        public static List<ManagerPickPosition> pickPositionsOnScene { get; set; } = new();
        public static EManagerState currentCreativeMode { get; set; } = EManagerState.None;
        public static int index { get; set; } = 0;

        public static void ClearData()
        {
            selectedOperation = null;
            selectedStep = null;
            steps?.Steps.Clear();
            activeAnchor = null;
            stepButtons?.Clear();
            pickPositionsOnScene?.Clear();
            currentCreativeMode = EManagerState.None;
        }

        public static ManagerPickPosition ReturnActivePickPosition()
        {
            var foundPickPosition = pickPositionsOnScene.Find(x =>
                x.stepIndex == index);

            return foundPickPosition == null ? null : foundPickPosition;
        }

        public static bool RemoveActivePickPosition()
        {
            var result = pickPositionsOnScene.Remove(ReturnActivePickPosition());
            return result;
        }
    }
}