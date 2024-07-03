using Data.Settings;
using Helper;
using KBCore.Refs;
using Language;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

namespace PickPositions
{
    public class PickPositionCreator : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Self] private PickPositionController pickPositionController;
        [SerializeField] private PopupManager popupManager;
        
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

        public ManagerPickPosition pickPositionOnEditMode;

        private bool aButtonPreviouslyPressed = false;

        private void Start()
        {
            pickPositionOnEditMode = null;

            sensitivityText.text = sensitivity.ToString("2F");
            deadZoneText.text = deadZone.ToString("2F");
            translationSpeedText.text = translationSpeed.ToString("2F");
            rotationSpeedText.text = rotationSpeed.ToString("2F");
        }
        
        private void Update()
        {
            if (!Validate()) return;

            // Debounce system
            if (pickPositionController.rightController.TryGetFeatureValue(CommonUsages.primaryButton, out var aButtonValue))
            {
                if (aButtonValue && !aButtonPreviouslyPressed) CreatePickPosition();
                aButtonPreviouslyPressed = aButtonValue;
            }

            if (pickPositionOnEditMode == null) return;
            if (pickPositionController.leftController.TryGetFeatureValue(CommonUsages.triggerButton, out var rightGripValue) && rightGripValue) ScalePickPosition();
            if (pickPositionController.rightController.TryGetFeatureValue(CommonUsages.triggerButton, out var leftGripValue) && leftGripValue) RotatePickPosition();
        }

        private bool Validate()
        {
            var isValid = true;
            var errorMessage = "";

            switch ((pickPositionController.validControllers, ManagerRuntimeData.activeAnchor != null))
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

            if (!isValid) popupManager.SendMessageToUser(errorMessage, PopupType.Error);

            return isValid;
        }
        
        private void CreatePickPosition()
        {
            if (ManagerRuntimeData.ReturnActivePickPosition() || pickPositionOnEditMode != null)
            {
                popupManager.SendMessageToUser(PickPositionLogMessages.pickPositionAlreadyCreatedOrLoaded, PopupType.Warning);
                return;
            }

            var position = pickPositionController.GetRightControllerPosition();

            var isPickPositionValid = Instantiate(pickPositionPrefab, position, Quaternion.identity, ManagerRuntimeData.activeAnchor.transform)
                .TryGetComponent(out ManagerPickPosition createdPickPosition);
            if (!isPickPositionValid)
            {
                popupManager.SendMessageToUser("ManagerPickPosition component is missing!", PopupType.Error);
                return;
            }
            
            createdPickPosition.SetDefaultPickPosition(ManagerRuntimeData.selectedStep.StepIndex);
            ManagerRuntimeData.pickPositionsOnScene.Add(createdPickPosition);

            pickPositionOnEditMode = createdPickPosition;
        }

        private void RotatePickPosition()
        {
            if (!pickPositionController.leftController.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out var angularVelocity)) return;
            pickPositionOnEditMode.gameObject.transform.Rotate(rotationSpeed * Time.deltaTime * angularVelocity, Space.World);
        }

        private void ScalePickPosition()
        {
            if (!pickPositionController.leftController.TryGetFeatureValue(CommonUsages.deviceVelocity, out var controllerVelocity)) return;
            if (!(Mathf.Abs(controllerVelocity.x) > deadZone) && !(Mathf.Abs(controllerVelocity.y) > deadZone) && !(Mathf.Abs(controllerVelocity.z) > deadZone)) return;
        
            var scaleChangeX = controllerVelocity.x * sensitivity * initialScale * translationSpeed;
            var scaleChangeY = controllerVelocity.y * sensitivity * initialScale * translationSpeed;
            var scaleChangeZ = controllerVelocity.z * sensitivity * initialScale * translationSpeed;

            pickPositionOnEditMode.gameObject.transform.localScale += new Vector3(scaleChangeX, scaleChangeY, scaleChangeZ);
        }

        #region Sliders
        
        public void UpdateDeadZone(float dead)
        { 
            deadZone = dead;
            deadZoneText.text = dead.ToString("F2");
        }

        public void UpdateTranslation(float translation)
        {
            translationSpeed = translation;
            translationSpeedText.text = translation.ToString("F2");
        }

        public void UpdateRotation(float rotation)
        {
            rotationSpeed = rotation;
            rotationSpeedText.text = rotation.ToString("F2");
        }

        public void UpdateSense(float sense)
        {
            sensitivity = sense;
            sensitivityText.text = sense.ToString("F2");
        }
        
        #endregion
    }
}