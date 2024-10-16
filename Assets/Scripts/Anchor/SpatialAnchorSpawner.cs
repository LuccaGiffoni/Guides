using KBCore.Refs;
using Meta.XR.BuildingBlocks;
using UnityEngine;

namespace Anchor
{
    public class SpatialAnchorSpawner : ValidatedMonoBehaviour
    {
        public GameObject AnchorPrefab
        {
            get => _anchorOnHand;
            set
            {
                _anchorOnHand = value;
                if (_anchorPrefabTransform) Destroy(_anchorPrefabTransform.gameObject);
                _anchorPrefabTransform = Instantiate(AnchorPrefab).transform;
                FollowHand = _followHand;
            }
        }

        public bool FollowHand
        {
            get => _followHand;
            set
            {
                _followHand = value;
                if (_followHand)
                {
                    _initialPosition = _anchorPrefabTransform.position;
                    _initialRotation = _anchorPrefabTransform.rotation;
                    _anchorPrefabTransform.parent = _cameraRig.rightControllerAnchor;
                    _anchorPrefabTransform.localPosition = Vector3.zero;
                    _anchorPrefabTransform.localRotation = Quaternion.identity;
                }
                else
                {
                    _anchorPrefabTransform.parent = null;
                    _anchorPrefabTransform.SetPositionAndRotation(_initialPosition, _initialRotation);
                }
            }
        }

        [Tooltip("A placeholder object to place in the anchor's position.")]
        [SerializeField] private GameObject _anchorOnHand;
        [SerializeField] private GameObject _anchorPrefab;

        [Tooltip("Anchor prefab GameObject will follow the user's right hand.")]
        [SerializeField] private bool _followHand = true;

        [SerializeField] private SpatialAnchorCoreBuildingBlock _spatialAnchorCore;
        [SerializeField, Scene] private OVRCameraRig _cameraRig;
        
        private Transform _anchorPrefabTransform;
        private Vector3 _initialPosition;
        private Quaternion _initialRotation;

        private void Awake()
        {
            _cameraRig = FindAnyObjectByType<OVRCameraRig>();
            AnchorPrefab = _anchorOnHand;
            FollowHand = _followHand;
        }

        public void SetAnchorVisibility(bool isAnchorVisible) => _anchorPrefab.SetActive(isAnchorVisible);
        
        public void SpawnSpatialAnchor()
        {
            if(!FollowHand)
                _spatialAnchorCore.InstantiateSpatialAnchor(_anchorPrefab, AnchorPrefab.transform.position, AnchorPrefab.transform.rotation);
            else
                _spatialAnchorCore.InstantiateSpatialAnchor(_anchorPrefab, _anchorPrefabTransform.position, _anchorPrefabTransform.rotation);
        }
    }
}
