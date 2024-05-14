using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace PickPositions
{
    public class RuntimeTransformHelper : MonoBehaviour
    {
        [Header("Logger")]
        [SerializeField] private TextMeshProUGUI rotation;
        [SerializeField] private TextMeshProUGUI localPosition;
        [FormerlySerializedAs("rotation")] [SerializeField] private TextMeshProUGUI localRotation;
        [SerializeField] private TextMeshProUGUI scale;

        private void FixedUpdate()
        {
            rotation.text = $"Rotation | X: {transform.rotation.x} | Y: {transform.rotation.y} |" +
                            $"Z: {transform.rotation.z} | W: {transform.localRotation.w}";
            localPosition.text = $"Local Position | X: {transform.localPosition.x} | Y: {transform.localPosition.y} | Z: {transform.localPosition.z}";
            scale.text = $"Scale | X: {transform.localScale.x} | Y: {transform.localScale.y} | Z: {transform.localScale.z}";
            localRotation.text = $"Rotation | X: {transform.localRotation.x} | Y: {transform.localRotation.y} |" +
                            $"Z: {transform.localRotation.z} | W: {transform.localRotation.w}";
        }
    }
}