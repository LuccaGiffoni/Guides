using Data.Responses;
using UnityEngine.Events;

namespace EventSystem.Events
{
    public class AnchorEvents
    {
        public class AnchorEvent<T> : UnityEvent<T> { }

        public readonly GenericEvent<AnchorEvent<Response<OVRSpatialAnchor>>> OnAnchorLoaded = new();
        public readonly GenericEvent<AnchorEvent<Response<OVRSpatialAnchor>>> OnAnchorCreated = new();
    }
}