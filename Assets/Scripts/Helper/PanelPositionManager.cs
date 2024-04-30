using System;
using KBCore.Refs;
using UnityEngine;

namespace Helper
{
    public class PanelPositionManager : ValidatedMonoBehaviour
    {
        [SerializeField] private Camera centerEyeAnchor;

        public void OnEnable() => SetPanelPosition();

        private void SetPanelPosition()
        {
            transform.position = new Vector3(centerEyeAnchor.transform.position.x,
                centerEyeAnchor.transform.position.y, centerEyeAnchor.transform.position.z + 0.65f);
        }
    }
}
