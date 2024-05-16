using Database.Settings;
using Helper;
using KBCore.Refs;
using Language;
using UnityEngine;
using UnityEngine.XR;

namespace PickPositions
{
    public class PickPositionCreator : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Self] private PickPositionController pickPositionController;
        [SerializeField, Scene] private PopupManager popupManager;
        
        [Header("OperatorPickPosition Settings")]
        [SerializeField] private GameObject pickPositionPrefab;

        [Header("Controller Settings")]
        [SerializeField] private float sensitivity = 1f;
        [SerializeField] private float deadZone = 0.01f;
        [SerializeField] private float rotationSpeed = 40f;
        [SerializeField] private float translationSpeed = 0.2f;
        
        private void Update()
        {
            if (!Validate()) return;
            
            if (pickPositionController.rightController.TryGetFeatureValue(CommonUsages.primaryButton, out var aButtonValue) && aButtonValue) CreatePickPosition();
        }

        private bool Validate()
        {
            var isValid = true;
            var errorMessage = "";

            switch ((pickPositionController.validControllers, ManagerRuntimeData.activeAnchor != null))
            {
                case (false, false):
                    errorMessage = PickPositionLogMessages.failedToGetControllersAndAnchor;
                    isValid = false;
                    break;
                case (false, true):
                    errorMessage = PickPositionLogMessages.failedToGetControllerPosition;
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
            var currentPickPosition = ManagerRuntimeData.ReturnActivePickPosition();
            if (currentPickPosition)
            {
                popupManager.SendMessageToUser(PickPositionLogMessages.pickPositionAlreadyCreatedOrLoaded, PopupType.Warning);
                return;
            }
            
            var position = pickPositionController.GetRightControllerPosition();
            if (position == Vector3.zero)
            {
                popupManager.SendMessageToUser(PickPositionLogMessages.failedToGetControllerPosition, PopupType.Error);
            }

            var isPickPositionValid = Instantiate(pickPositionPrefab, position, Quaternion.identity, ManagerRuntimeData.activeAnchor.transform).TryGetComponent(out OperatorPickPosition createdPickPosition);
            if (!isPickPositionValid)
            {
                popupManager.SendMessageToUser(PickPositionLogMessages.pickPositionAlreadyCreatedOrLoaded, PopupType.Warning);
                return;
            }
            
            createdPickPosition.SetDefaultPickPosition(ManagerRuntimeData.selectedStep.StepIndex);
            ManagerRuntimeData.pickPositionsOnScene.Add(createdPickPosition);
        }
    }
}