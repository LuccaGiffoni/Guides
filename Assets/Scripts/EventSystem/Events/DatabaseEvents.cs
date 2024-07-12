using Data.Entities;
using Data.Responses;
using UnityEngine.Events;

namespace EventSystem.Events
{
    public class DatabaseEvents
    {
        public class DatabaseEvent<T> : UnityEvent<T> { }

        public readonly GenericEvent<DatabaseEvent<Response<StepList>>> OnStepsLoaded = new();
    }
}