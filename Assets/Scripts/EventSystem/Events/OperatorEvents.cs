using Data.Responses;
using UnityEngine;
using UnityEngine.Events;

namespace EventSystem.Events
{
    public class OperatorEvents : MonoBehaviour
    {
        public class OperatorEvent<T> : UnityEvent<T> { }

        public readonly GenericEvent<OperatorEvent<Response<int>>> OnStepChanged = new();
    }
}
