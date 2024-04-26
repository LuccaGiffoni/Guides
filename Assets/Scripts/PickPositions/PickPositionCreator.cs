using System.Collections.Generic;
using System.Linq;
using Database.Methods;
using Database.Settings;
using Helper;
using KBCore.Refs;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

namespace PickPositions
{
    public class PickPositionCreator : ValidatedMonoBehaviour
    {
        [Header("References")] [SerializeField, Scene] private PopupManager _popupManager;
        [SerializeField] private PickPositionLoader pickPositionLoader;
        
        [Header("PickPosition Settings")]
        [SerializeField] private GameObject cubePrefab;
        [SerializeField] private float initialScale = 0.2f;

        [Header("Controller Settings")]
        [SerializeField] private float sensitivity = 1f;
        [SerializeField] private float deadzone = 0.01f;
        [SerializeField] private float rotationSpeed = 25f;
        [SerializeField] private float translationSpeed = 0.2f;

        public GameObject activeCube;
        private PickPosition _activePickPosition;

        private InputDevice _rightController;
        private InputDevice _leftController;

        private const InputDeviceCharacteristics DesiredLeftCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        private const InputDeviceCharacteristics DesiredRightCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;

        private void Start()
        {
            GetControllers();
        }

        private void GetControllers()
        {
            var leftHandedControllers = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(DesiredLeftCharacteristics, leftHandedControllers);

            foreach (var device in leftHandedControllers)
            {
                _leftController = device;
            }

            var rightHandedControllers = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(DesiredRightCharacteristics, rightHandedControllers);

            foreach (var device in rightHandedControllers)
            {
                _rightController = device;
            }
        }

        void Update()
        {
            if (!_rightController.isValid || !_leftController.isValid) return;
            
            if (_rightController.TryGetFeatureValue(CommonUsages.primaryButton, out var aButtonValue))
            {
                if (aButtonValue)
                {
                    CreateCube();
                }
            }

            if (_leftController.TryGetFeatureValue(CommonUsages.triggerButton, out var rightGripValue) && activeCube != null)
            {
                if(rightGripValue)
                {
                    ScaleCube();
                }
            }

            if (_leftController.TryGetFeatureValue(CommonUsages.primaryButton, out var xButtonValue) && activeCube != null)
            {
                if (xButtonValue)
                {
                    DestroyActiveCube();
                }
            }

            if (_rightController.TryGetFeatureValue(CommonUsages.triggerButton, out var leftGripValue) && activeCube != null)
            {
                if(leftGripValue)
                {
                    RotateCube();
                }
            }
        }

        private void CreateCube()
        {
            if (RuntimeData.activeAnchor == null)
            {
                _popupManager.SendMessageToUser("Nenhum ponto de ancoragem ativo. Carregue a âncora ao clicar em 'B'", PopupType.Warning);
                return;
            }
            
            if(activeCube != null) return;
            if (!_rightController.TryGetFeatureValue(CommonUsages.devicePosition, out var position)) return;

            if (pickPositionLoader.localLoadedPickPosition is not null)
            {
                Destroy(pickPositionLoader.localLoadedPickPosition.gameObject);
                pickPositionLoader.localLoadedPickPosition = null;
            }

            activeCube = Instantiate(cubePrefab, position, Quaternion.identity, RuntimeData.activeAnchor.transform);
            activeCube.transform.localScale = new Vector3(initialScale, initialScale, initialScale);
            _activePickPosition = activeCube.GetComponent<PickPosition>();
            RuntimeData.activePickPosition = activeCube;
            _activePickPosition.runtimeIndex = RuntimeData.selectedStep.StepIndex;
        }

        private void ScaleCube()
        {
            if (!_leftController.TryGetFeatureValue(CommonUsages.deviceVelocity, out var controllerVelocity)) return;
            if (!(Mathf.Abs(controllerVelocity.x) > deadzone) && !(Mathf.Abs(controllerVelocity.y) > deadzone) && !(Mathf.Abs(controllerVelocity.z) > deadzone)) return;
        
            var scaleChangeX = controllerVelocity.x * sensitivity * initialScale * translationSpeed;
            var scaleChangeY = controllerVelocity.y * sensitivity * initialScale * translationSpeed;
            var scaleChangeZ = controllerVelocity.z * sensitivity * initialScale * translationSpeed;

            activeCube.transform.localScale += new Vector3(scaleChangeX, scaleChangeY, scaleChangeZ);
            _activePickPosition.runtimeIndex = RuntimeData.selectedStep.StepIndex;
        }

        private void RotateCube()
        {
            if (!_leftController.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out var angularVelocity)) return;
            
            activeCube.transform.Rotate(rotationSpeed * Time.deltaTime * angularVelocity, Space.World);
            _activePickPosition.runtimeIndex = RuntimeData.selectedStep.StepIndex;
        }

        private void DestroyActiveCube()
        {
            if (activeCube == null) return;
        
            Destroy(activeCube);
            activeCube = null;
            _activePickPosition = null;
            RuntimeData.activePickPosition = null;
        }

        public async void SavePickPositionToDatabase()
        {
            //var relativePosition = activeCube.transform.position - RuntimeData.activeAnchor.transform.position;
            // var result = await Post.SavePickPositionToDatabase(RuntimeData.selectedStep.StepID, relativePosition, activeCube.transform);

            var result = await Post.SavePickPositionToDatabase(RuntimeData.selectedStep.StepID, activeCube.transform);
            
            Destroy(activeCube.gameObject);
            activeCube = null;

            if(result) _popupManager.SendMessageToUser("PickPosition registrada no banco de dados com sucesso!", PopupType.Info);
            else _popupManager.SendMessageToUser("Houve um problema ao salvar a PickPosition. Tente novamente!", PopupType.Error);
        }
    }
}