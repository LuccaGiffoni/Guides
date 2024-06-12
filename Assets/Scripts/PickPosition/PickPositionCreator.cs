using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Primitives;

public class PickPositionCreator : MonoBehaviour
{
    [Header("Cube")]
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private TextMeshProUGUI debugger;

    [Header("Controller Settings")]
    [SerializeField] private float initialScale = 0.1f;
    [SerializeField] private float sensitivity = 1f;
    [SerializeField] private float deadzone = 0.05f;
    [SerializeField] private float rotationSpeed = 25f;
    [SerializeField] private float translationSpeed = 0.2f;
    [SerializeField] private List<TextMeshProUGUI> parameters = new();

    [Header("Reference Zone")]
    [SerializeField] private Transform referenceZone;

    private GameObject activeCube;
    private PickPosition activePickPosition;
    private SetupOperation setupOperation;

    private InputDevice rightController;
    private InputDevice leftController;

    private void Start()
    {
        GetControllers();
        setupOperation = GameObject.FindGameObjectWithTag("API").GetComponent<SetupOperation>();
    }

    private void GetControllers()
    {
        var leftHandedControllers = new List<InputDevice>();
        var desiredLeftCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(desiredLeftCharacteristics, leftHandedControllers);

        foreach (var device in leftHandedControllers)
        {
            leftController = device;
        }

        var rightHandedControllers = new List<InputDevice>();
        var desiredRightCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(desiredRightCharacteristics, rightHandedControllers);

        foreach (var device in rightHandedControllers)
        {
            rightController = device;
        }
    }

    void Update()
    {
        if (rightController.isValid && leftController.isValid)
        {
            if (rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool aButtonValue))
            {
                if (aButtonValue)
                {
                    CreateCube();
                    activePickPosition.SetNewParametersForPickPosition();
                }
            }

            if (leftController.TryGetFeatureValue(CommonUsages.triggerButton, out bool rightGripValue))
            {
                if(rightGripValue && activeCube != null)
                {
                    ScaleCube();
                    activePickPosition.SetNewParametersForPickPosition();
                }
            }

            if (leftController.TryGetFeatureValue(CommonUsages.primaryButton, out bool xButtonValue))
            {
                if (xButtonValue)
                {
                    DestroyActiveCube();
                }
            }

            if (rightController.TryGetFeatureValue(CommonUsages.triggerButton, out bool leftGripValue))
            {
                if(leftGripValue)
                {
                    RotateCube();
                    activePickPosition.SetNewParametersForPickPosition();
                }
            }
        }
    }

    private void CreateCube()
    {
        var cubeList = GameObject.FindGameObjectsWithTag("Cube").ToList();

        if (cubeList.Count == 0)
        {
            if (rightController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position))
            {
                activeCube = Instantiate(cubePrefab, position, Quaternion.identity);
                activePickPosition = activeCube.GetComponent<PickPosition>();
                activePickPosition.runtimeIndex = setupOperation.runtimeIndex;
                debugger.text += $"\n\nPickPosition criado em {activePickPosition.position}";
                activePickPosition.SetPickPositionOnCreation();
            }
        }
    }

    private void ScaleCube()
    {
        if (leftController.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 controllerVelocity))
        {
            if (Mathf.Abs(controllerVelocity.x) > deadzone || Mathf.Abs(controllerVelocity.y) > deadzone || Mathf.Abs(controllerVelocity.z) > deadzone)
            {
                float scaleChangeX = controllerVelocity.x * sensitivity * initialScale * translationSpeed;
                float scaleChangeY = controllerVelocity.y * sensitivity * initialScale * translationSpeed;
                float scaleChangeZ = controllerVelocity.z * sensitivity * initialScale * translationSpeed;

                activeCube.transform.localScale += new Vector3(scaleChangeX, scaleChangeY, scaleChangeZ);
            }
        }
    }

    private void RotateCube()
    {
        if (leftController.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out Vector3 angularVelocity))
        {
            activeCube.transform.Rotate(rotationSpeed * Time.deltaTime * angularVelocity, Space.World);
        }
    }

    public void DestroyActiveCube()
    {
        if (activeCube != null)
        {
            Destroy(activeCube);
            activeCube = null;
            activePickPosition = null;
        }
    }

    public void UpdateSensitivity(float newValue) { sensitivity = newValue; UpdateParametersValue(1, newValue);  }
    public void UpdateDeadzone(float newValue) { deadzone = newValue; UpdateParametersValue(0, newValue); }
    public void UpdateRotationSpeed(float newValue) { rotationSpeed = newValue; UpdateParametersValue(2, newValue); }
    public void UpdateTranslationSpeed(float newValue) { translationSpeed = newValue; UpdateParametersValue(3, newValue); }

    private void UpdateParametersValue(int index, float value)
    {
        parameters[index].text = value.ToString("F2");
    }
}