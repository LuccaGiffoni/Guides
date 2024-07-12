using KBCore.Refs;
using UnityEngine;

namespace UI
{
    public class PanelPositionManager : ValidatedMonoBehaviour
    {
        [SerializeField] private Camera centerEyeAnchor;

        private bool finalPosition;

        private void Start() => finalPosition = false;

        public void LateUpdate()
        {
            if (centerEyeAnchor.transform.position.z - transform.position.z <= 0.7f && 
                centerEyeAnchor.transform.position.y - transform.position.y <= 0.05f && !finalPosition)
            {
                SetPanelPosition();
            }
        }

        private void SetPanelPosition()
        {
            transform.position = new Vector3(centerEyeAnchor.transform.localPosition.x,
                centerEyeAnchor.transform.localPosition.y, centerEyeAnchor.transform.localPosition.z + 0.65f);
            finalPosition = true;
        }
    }
}
