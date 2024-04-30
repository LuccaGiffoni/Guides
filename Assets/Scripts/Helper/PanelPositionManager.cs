using System;
using KBCore.Refs;
using UnityEngine;

namespace Helper
{
    public class PanelPositionManager : ValidatedMonoBehaviour
    {
        [SerializeField, Scene] private OVRCameraRig mainCamera;

        public void Start() => SetPanelPosition();

        private void SetPanelPosition()
        {
            transform.position = new Vector3(mainCamera.transform.position.x,
                mainCamera.transform.position.y, mainCamera.transform.position.z + 0.65f);
        }
    }
}
