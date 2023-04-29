using UnityEngine;

namespace Interactions.UpgradesIsland
{
    public class ConveyorAnimation : MonoBehaviour
    {
        [SerializeField, Min(0)] private int _materialIndex = 0;
        [SerializeField] private float _conveyorSpeed = 2f;
        [SerializeField] private MeshRenderer _mesh;

        private void OnValidate()
        {
            if (_mesh != null && _mesh.sharedMaterials.Length <= _materialIndex)
                Debug.LogError($"{nameof(_materialIndex)} is more, than materials count!" +
                    $"Materials count - {_mesh.sharedMaterials.Length}!");
        }

        private void OnEnable()
        {
            SetMaterialToDefaultPosition();
        }

        private void OnDisable()
        {
            SetMaterialToDefaultPosition();
        }

        private void Update()
        {
            MoveMaterial();
        }

        private void MoveMaterial()
        {
            var materials = _mesh.materials;

            Vector2 materialOffset = materials[_materialIndex].mainTextureOffset;
            materialOffset.y += _conveyorSpeed * Time.deltaTime;
            materials[_materialIndex].mainTextureOffset = materialOffset;

            _mesh.materials = materials;
        }

        private void SetMaterialToDefaultPosition()
        {
            var materials = _mesh.materials;

            Vector2 materialOffset = materials[_materialIndex].mainTextureOffset;
            materialOffset.y = 0f;
            materials[_materialIndex].mainTextureOffset = materialOffset;

            _mesh.materials = materials;
        }
    }
}
