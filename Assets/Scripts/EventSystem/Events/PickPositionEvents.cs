using Data.Responses;
using UnityEngine;
using UnityEngine.Events;

namespace EventSystem.Events
{
    public class PickPositionEvents
    {
        public class PickPositionEvent<T> : UnityEvent<T> { }

        public readonly GenericEvent<PickPositionEvent<Response<Transform>>> OnPickPositionDeleted = new();
        public readonly GenericEvent<PickPositionEvent<Response<Transform>>> OnPickPositionUpdated = new();
        public readonly GenericEvent<PickPositionEvent<Response<Transform>>> OnPickPositionCreated= new();
    }
}