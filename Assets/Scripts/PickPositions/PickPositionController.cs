using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using Vector3 = UnityEngine.Vector3;

namespace PickPositions
{
    public class PickPositionController : MonoBehaviour
    {
        public bool validControllers { get; private set; }
        
        public InputDevice rightController { get; private set; }
        public InputDevice leftController { get; private set; }
        
        private const InputDeviceCharacteristics DesiredLeftCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        private const InputDeviceCharacteristics DesiredRightCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;

        [HideInInspector] public UnityEvent<GameObject> onPickPositionCreated;

        private void Start() => validControllers = GetControllers();

        public bool GetControllers()
        {
            var leftHandedControllers = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(DesiredLeftCharacteristics, leftHandedControllers);
            foreach (var device in leftHandedControllers) leftController = device;

            var rightHandedControllers = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(DesiredRightCharacteristics, rightHandedControllers);
            foreach (var device in rightHandedControllers) rightController = device;
            
            return rightController.isValid && leftController.isValid;
        }

        public Vector3 GetRightControllerPosition()
        {
            var result = rightController.TryGetFeatureValue(CommonUsages.devicePosition, out var position);
            return result ? position : Vector3.zero;
        }
    }
}