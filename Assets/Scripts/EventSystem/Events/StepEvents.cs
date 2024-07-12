using Data.Enums;
using Data.Responses;
using UnityEngine.Events;

namespace EventSystem.Events
{
    public class StepEvents
    {
        public class StepEvent<T> : UnityEvent<T> { }

        public readonly GenericEvent<StepEvent<Response<int>>> OnStepChanged = new();
        public readonly GenericEvent<StepEvent<Response<EManagerState>>> OnCreativeModeChanged = new();
    }
}