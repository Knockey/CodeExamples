using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Interactions
{
    public class GrowBuildingBlink : MonoBehaviour
    {
        private const string MaterialColor = "_Color";

        [SerializeField] private List<MeshRenderer> _renderers;
        [SerializeField, Min(0)] private int _materialIndex = 0;
        [SerializeField, Min(0f)] private float _blinkTime = 0.75f;
        [Header("Transparency parametrs")]
        [SerializeField, Min(0f)] private float _minTransparency = 0.15f;
        [SerializeField, Min(0f)] private float _maxTransparency = 0.65f;

        private List<Material> _transparentMaterials;
        private Coroutine _blink;

        private void OnValidate()
        {
            if (_renderers != null && _renderers.Count > 0)
            {
                _renderers.ForEach(renderer =>
                {
                    if (renderer != null && renderer.sharedMaterials.Length <= _materialIndex)
                        Debug.LogError($"{nameof(_materialIndex)} is more, than materials count!" +
                            $"Materials count - {renderer.sharedMaterials.Length}!");
                });
            }
        }

        private void Awake()
        {
            FillMaterialsList();
        }

        public void EnableObject()
        {
            gameObject.SetActive(true);
        }

        public void DisableObject()
        {
            gameObject.SetActive(false);
            TryDisableBlink();
        }

        public void TryEnableBlink(bool isBlinking)
        {
            if (isBlinking == false)
                return;

            if (gameObject.activeSelf == false)
                return;

            _blink = StartCoroutine(Blink());
        }

        public void TryDisableBlink()
        {
            if (_blink != null)
                StopCoroutine(_blink);

            SetTransparency(_minTransparency);
        }

        private void FillMaterialsList()
        {
            _transparentMaterials = new List<Material>();

            _renderers.ForEach(renderer =>
            {
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                var materials = renderer.materials;
                _transparentMaterials.Add(materials[_materialIndex]);
            });

            SetTransparency(_minTransparency);
        }

        private IEnumerator Blink()
        {
            var currentMax = _maxTransparency;
            var currentMin = _minTransparency;
            var currentTime = 0f;

            while (enabled)
            {
                yield return null;

                currentTime += Time.deltaTime;

                var currentTransparency = Mathf.Lerp(currentMin, currentMax, currentTime / _blinkTime);
                SetTransparency(currentTransparency);

                if (currentTime >= _blinkTime)
                {
                    currentTime = 0f;
                    (currentMin, currentMax) = (currentMax, currentMin);
                }
            }
        }

        private void SetTransparency(float currentTransparency)
        {
            if (_transparentMaterials == null)
            {
                FillMaterialsList();
            }

            _transparentMaterials.ForEach(material =>
            {
                var color = material.color;
                color.a = currentTransparency;
                material.SetColor(MaterialColor, color);
            });
        }

#if UNITY_EDITOR
        public void FillRenderedsList()
        {
            _renderers = GetComponentsInChildren<MeshRenderer>().ToList();
            Save();
        }

        private void Save() => UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
