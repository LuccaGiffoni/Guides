using Scripts_v2.Data.Models;
using UnityEngine.Events;
using UnityEngine;

namespace Scripts_v2.EventSystem
{
    public class PickPositionEvents
    {
        public class PickPositionEvent : UnityEvent<Component, PickPositionData> { }

        public GenericEvent<PickPositionEvent> OnPickPositionCreated = new();
        public GenericEvent<PickPositionEvent> OnPickPositionUpdated = new();
        public GenericEvent<PickPositionEvent> OnPickPositionDeleted = new();
    }
}