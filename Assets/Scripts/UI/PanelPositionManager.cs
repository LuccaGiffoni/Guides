using KBCore.Refs;
using UnityEngine;

namespace UI
{
    public class PanelPositionManager : ValidatedMonoBehaviour
    {
        private void Start()
        {
            var position = GetPosition();
            if (position.HasValue && gameObject.transform.position != position.Value.Item1 &&
                gameObject.transform.rotation != position.Value.Item2)
                SetPosition();
        }

        private (Vector3, Quaternion)? GetPosition()
        {
            return (new Vector3(PlayerPrefs.GetFloat("PX"), PlayerPrefs.GetFloat("PY"), PlayerPrefs.GetFloat("PZ")),
                new Quaternion(PlayerPrefs.GetFloat("RX"), PlayerPrefs.GetFloat("RY"), PlayerPrefs.GetFloat("RZ"),
                    PlayerPrefs.GetFloat("RW")));
        }

        public void SetPosition()
        {
            PlayerPrefs.SetFloat("PX", gameObject.transform.position.x);
            PlayerPrefs.SetFloat("PY", gameObject.transform.position.y);
            PlayerPrefs.SetFloat("PZ", gameObject.transform.position.z);
            PlayerPrefs.SetFloat("RX", gameObject.transform.rotation.x);
            PlayerPrefs.SetFloat("RY", gameObject.transform.rotation.y);
            PlayerPrefs.SetFloat("RZ", gameObject.transform.rotation.z);
            PlayerPrefs.SetFloat("RW", gameObject.transform.rotation.w);
        }
    }
}
