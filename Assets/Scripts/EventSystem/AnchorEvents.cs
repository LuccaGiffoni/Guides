using Data.Entities;
using Responses;
using UnityEngine.Events;

namespace EventSystem
{
    public class AnchorEvents
    {
        public class AnchorEvent<T> : UnityEvent<T> { }

        public readonly GenericEvent<AnchorEvent<Response<OVRSpatialAnchor>>> OnAnchorLoaded = new();
        public readonly GenericEvent<AnchorEvent<Response<OVRSpatialAnchor>>> OnAnchorCreated = new();
    }
    
    public class DatabaseEvents
    {
        public class DatabaseEvent<T> : UnityEvent<T> { }

        public readonly GenericEvent<DatabaseEvent<Response<StepList>>> OnStepsLoaded = new();
    }
    
    public class StepEvents
    {
        public class StepEvent<T> : UnityEvent<T> { }

        public readonly GenericEvent<StepEvent<Response<int>>> OnStepChanged = new();
    }
}