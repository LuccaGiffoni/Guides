using UnityEngine;

namespace UI
{
    public class LazyFollow : MonoBehaviour
    {
        [Header("Target"), SerializeField] private Transform target;
    
        [Header("Settings")]
        [SerializeField] private float followSpeed = 1.1f; 
        [SerializeField] private Vector3 offset = new(0f, 0f, 1f);

        private void LateUpdate()
        {
            var targetPosition = target.position + target.TransformDirection(offset);
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
        }
    }
}