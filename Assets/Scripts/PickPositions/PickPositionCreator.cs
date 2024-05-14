using System.Collections.Generic;
using Database.Methods;
using Database.Settings;
using Helper;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.XR;

namespace PickPositions
{
    public class PickPositionCreator : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Scene] private PopupManager popupManager;
        [SerializeField, Scene] private PickPositionLoader pickPositionLoader;
        
        [Header("PickPosition Settings")]
        [SerializeField] private GameObject cubePrefab;
        [SerializeField] private float initialScale = 0.2f;

        [Header("Controller Settings")]
        [SerializeField] private float sensitivity = 1f;
        [SerializeField] private float deadzone = 0.01f;
        [SerializeField] private float rotationSpeed = 40f;
        [SerializeField] private float translationSpeed = 0.2f;

        public GameObject activeCube;
        private PickPosition activePickPosition;

        private InputDevice rightController;
        private InputDevice leftController;

        private const InputDeviceCharacteristics DesiredLeftCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        private const InputDeviceCharacteristics DesiredRightCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;

        private void Start() => GetControllers();

        private void GetControllers()
        {
            var leftHandedControllers = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(DesiredLeftCharacteristics, leftHandedControllers);

            foreach (var device in leftHandedControllers)
                leftController = device;

            var rightHandedControllers = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(DesiredRightCharacteristics, rightHandedControllers);

            foreach (var device in rightHandedControllers)
                rightController = device;
        }

        private void Update()
        {
            if (!rightController.isValid || !leftController.isValid) return;
            if (rightController.TryGetFeatureValue(CommonUsages.primaryButton, out var aButtonValue) && aButtonValue) CreateCube();
            
            if(activeCube == null) return;
            if (leftController.TryGetFeatureValue(CommonUsages.triggerButton, out var rightGripValue) && rightGripValue) ScaleCube();
            if (leftController.TryGetFeatureValue(CommonUsages.primaryButton, out var xButtonValue)  && xButtonValue) DestroyActiveCube();
            if (rightController.TryGetFeatureValue(CommonUsages.triggerButton, out var leftGripValue) && leftGripValue) RotateCube();
        }

        private void CreateCube()
        {
            if (RuntimeData.activeAnchor == null)
            {
                popupManager.SendMessageToUser("Nenhum ponto de ancoragem ativo. Carregue a âncora ao clicar em 'B'", PopupType.Warning);
                return;
            }
            
            if(activeCube != null) return;
            if (!rightController.TryGetFeatureValue(CommonUsages.devicePosition, out var position)) return;

            if (pickPositionLoader.localLoadedPickPosition is not null)
            {
                Destroy(pickPositionLoader.localLoadedPickPosition.gameObject);
                pickPositionLoader.localLoadedPickPosition = null;
            }

            activeCube = Instantiate(cubePrefab, position, Quaternion.identity, RuntimeData.activeAnchor.transform);
            activeCube.transform.localScale = new Vector3(initialScale, initialScale, initialScale);
            activePickPosition = activeCube.GetComponent<PickPosition>();
            RuntimeData.activePickPosition = activeCube;
            activePickPosition.runtimeIndex = RuntimeData.selectedStep.StepIndex;
        }

        private void ScaleCube()
        {
            if (!leftController.TryGetFeatureValue(CommonUsages.deviceVelocity, out var controllerVelocity)) return;
            if (!(Mathf.Abs(controllerVelocity.x) > deadzone) && !(Mathf.Abs(controllerVelocity.y) > deadzone) && !(Mathf.Abs(controllerVelocity.z) > deadzone)) return;
        
            var scaleChangeX = controllerVelocity.x * sensitivity * initialScale * translationSpeed;
            var scaleChangeY = controllerVelocity.y * sensitivity * initialScale * translationSpeed;
            var scaleChangeZ = controllerVelocity.z * sensitivity * initialScale * translationSpeed;

            activeCube.transform.localScale += new Vector3(scaleChangeX, scaleChangeY, scaleChangeZ);
            activePickPosition.runtimeIndex = RuntimeData.selectedStep.StepIndex;
        }

        private void RotateCube()
        {
            if (!leftController.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out var angularVelocity)) return;
            
            activeCube.transform.Rotate(rotationSpeed * Time.deltaTime * angularVelocity, Space.World);
            activePickPosition.runtimeIndex = RuntimeData.selectedStep.StepIndex;
        }

        private void DestroyActiveCube()
        {
            if (activeCube == null) return;
        
            Destroy(activeCube);
            activeCube = null;
            activePickPosition = null;
            RuntimeData.activePickPosition = null;
        }

        public async void SavePickPositionToDatabase()
        {
            var result = await Post.SavePickPositionToDatabase(RuntimeData.selectedStep.StepID, activeCube.transform);
            activeCube = null;

            if(result) popupManager.SendMessageToUser("PickPosition registrada no banco de dados com sucesso!", PopupType.Info);
            else popupManager.SendMessageToUser("Houve um problema ao salvar a PickPosition. Tente novamente!", PopupType.Error);
        }
    }
}