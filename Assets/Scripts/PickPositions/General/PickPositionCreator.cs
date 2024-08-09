using System.Linq;
using Data.Entities;
using Data.Enums;
using Data.Runtime;
using Data.ScriptableObjects;
using Data.Settings;
using KBCore.Refs;
using Messages;
using PickPositions.Roles;
using Services.Implementations;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

namespace PickPositions.General
{
    public class PickPositionCreator : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Self] private PickPositionController pickPositionController;
        [FormerlySerializedAs("popupManager")] [SerializeField] private PopupService popupService;
        
        [Header("OperatorPickPosition Settings")]
        [SerializeField] private GameObject pickPositionPrefab;

        [Header("Controller Settings")]
        [SerializeField] private float sensitivity = 1f;
        [SerializeField] private float deadZone = 0.01f;
        [SerializeField] private float rotationSpeed = 40f;
        [SerializeField] private float translationSpeed = 0.2f;
        [SerializeField] private float initialScale = 0.1f;

        [SerializeField] private TMP_Text sensitivityText;
        [SerializeField] private TMP_Text deadZoneText;
        [SerializeField] private TMP_Text rotationSpeedText;
        [SerializeField] private TMP_Text translationSpeedText;

       [Header("Runtime Data"), SerializeField] private RuntimeDataForManager runtimeDataForManager;

        private OVRSpatialAnchor ovrSpatialAnchor;
        
        private bool aButtonPreviouslyPressed = false;
        
        private void Update()
        {
            if (!Validate()) return;

            // Debounce system
            if (pickPositionController.rightController.TryGetFeatureValue(CommonUsages.primaryButton, out var aButtonValue))
            {
                if (aButtonValue && !aButtonPreviouslyPressed) CreatePickPosition();
                aButtonPreviouslyPressed = aButtonValue;
            }

            if (runtimeDataForManager.ActivePickPosition == null) return;
            if (pickPositionController.leftController.TryGetFeatureValue(CommonUsages.triggerButton, out var rightGripValue) && rightGripValue) ScalePickPosition();
            if (pickPositionController.rightController.TryGetFeatureValue(CommonUsages.triggerButton, out var leftGripValue) && leftGripValue) RotatePickPosition();
        }

        private bool Validate()
        {
            var isValid = true;
            var errorMessage = "";

            ovrSpatialAnchor = FindFirstObjectByType<OVRSpatialAnchor>();
            
            switch ((pickPositionController.validControllers, ovrSpatialAnchor is not null))
            {
                case (false, false):
                    errorMessage = PickPositionLogMessages.failedToGetControllersAndAnchor;
                    pickPositionController.GetControllers();
                    isValid = false;
                    break;
                case (false, true):
                    errorMessage = PickPositionLogMessages.failedToGetControllerPosition;
                    pickPositionController.GetControllers();
                    isValid = false;
                    break;
                case (true, false):
                    errorMessage = PickPositionLogMessages.noAnchorLoadedOnScene;
                    isValid = false;
                    break;
            }

            if (!isValid) popupService.SendMessageToUser(errorMessage, EPopupType.Error);

            return isValid;
        }
        
        private void CreatePickPosition()
        {
            if (runtimeDataForManager.ActivePickPosition)
            {
                popupService.SendMessageToUser(PickPositionLogMessages.pickPositionAlreadyCreatedOrLoaded, EPopupType.Warning);
                return;
            }

            var position = pickPositionController.GetRightControllerPosition();

            var isPickPositionValid = Instantiate(pickPositionPrefab, position, Quaternion.identity, ovrSpatialAnchor.transform)
                .TryGetComponent(out ManagerPickPosition createdPickPosition);
            if (!isPickPositionValid)
            {
                popupService.SendMessageToUser("ManagerPickPosition component is missing!", EPopupType.Error);
                return;
            }
            
            createdPickPosition.SetDefaultPickPosition(runtimeDataForManager.Index);
            createdPickPosition.stepId = runtimeDataForManager.Index;
            runtimeDataForManager.PickPositions.Add(createdPickPosition);
        }

        private void RotatePickPosition()
        {
            if (!pickPositionController.leftController.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out var angularVelocity)) return;
            runtimeDataForManager.ActivePickPosition.gameObject.transform.Rotate(rotationSpeed * Time.deltaTime * angularVelocity, Space.World);
        }

        private void ScalePickPosition()
        {
            if (!pickPositionController.leftController.TryGetFeatureValue(CommonUsages.deviceVelocity, out var controllerVelocity)) return;
            if (!(Mathf.Abs(controllerVelocity.x) > deadZone) && !(Mathf.Abs(controllerVelocity.y) > deadZone) && !(Mathf.Abs(controllerVelocity.z) > deadZone)) return;
        
            var scaleChangeX = controllerVelocity.x * sensitivity * initialScale * translationSpeed;
            var scaleChangeY = controllerVelocity.y * sensitivity * initialScale * translationSpeed;
            var scaleChangeZ = controllerVelocity.z * sensitivity * initialScale * translationSpeed;

            runtimeDataForManager.ActivePickPosition.gameObject.transform.localScale += new Vector3(scaleChangeX, scaleChangeY, scaleChangeZ);
        }
    }
}