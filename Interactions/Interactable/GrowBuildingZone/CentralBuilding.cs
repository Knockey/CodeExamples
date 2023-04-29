using Extensions;
using UnityEngine;

namespace Interactions
{
    public class CentralBuilding : MonoBehaviour
    {
        private const float DefaultHeight = 0.1f;

        [SerializeField] private Transform _mesh;
        [SerializeField] private Renderer _meshRenderer;
        [SerializeField] private GrowBuildingsZone _buildingsZone;

        private Vector3 _minPosition;
        private Vector3 _maxPosition;

        private void Awake()
        {
            var height = _mesh.transform.position.y - _meshRenderer.bounds.size.y - DefaultHeight;
            _minPosition = _mesh.transform.position.With(y: height);
            _maxPosition = _mesh.transform.position;
        }

        private void OnEnable()
        {
            _buildingsZone.ValueInited += OnValueInited;
            _buildingsZone.ValueChanged += OnValueChanged;
        }

        private void OnDisable()
        {
            _buildingsZone.ValueInited -= OnValueInited;
            _buildingsZone.ValueChanged -= OnValueChanged;
        }

        private void OnValueInited(float value, float maxValue) => SetMeshHeight(value, maxValue);

        private void OnValueChanged(float value, float maxValue) => SetMeshHeight(value, maxValue);

        private void SetMeshHeight(float _, float __)
        {
            _mesh.transform.position = Vector3.Lerp(_minPosition, _maxPosition, _buildingsZone.ResourceLeftToSpentPercent);
        }
    }
}
