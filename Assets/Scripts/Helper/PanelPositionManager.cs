using System;
using KBCore.Refs;
using Oculus.Interaction.Input;
using UnityEngine;
using UnityEngine.Serialization;

namespace Helper
{
    public class PanelPositionManager : ValidatedMonoBehaviour
    {
        [SerializeField] private Camera centerEyeAnchor;

        private bool finalPosition;

        private void Start() => finalPosition = false;

        public void LateUpdate()
        {
            if (centerEyeAnchor.transform.position.y >= 0.5 && !finalPosition)
            {
                SetPanelPosition();
            }
        }

        private void SetPanelPosition()
        {
            transform.position = new Vector3(centerEyeAnchor.transform.localPosition.x,
                centerEyeAnchor.transform.localPosition.y - 0.15f, centerEyeAnchor.transform.localPosition.z + 0.4f);
            finalPosition = true;
        }
    }
}
