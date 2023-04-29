using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    [RequireComponent(typeof(TapInput))]
    public class ConveyorVFX : MonoBehaviour
    {
        private const string MainColor = "_Color";
        private const string DimColor = "_ColorDim";

        [SerializeField] private List<Renderer> _renderers;
        [SerializeField] private Color _maxTapsColor = Color.red;
        [SerializeField] private Color _defaultColor = Color.white;
        [SerializeField] private Color _defaultDimColor = Color.white;
        [SerializeField, Min(0)] private int _tapsCountToMaxHeat = 10;
        [SerializeField, Min(0f)] private float _returnToDefaultTime = 1f;
        [Header("Scale change")]
        [SerializeField] private List<Transform> _meshes;
        [SerializeField, Min(1f)] private float _maxScaleModifier = 1.35f;

        private TapInput _tapInput;
        private Coroutine _returnToDefault;
        private Vector3 _defaultScale;
        private Vector3 _maxScale;

        private void Awake()
        {
            _tapInput = GetComponent<TapInput>();
            _defaultScale = _meshes[0].localScale;
            _maxScale = _defaultScale * _maxScaleModifier;
        }

        private void OnEnable()
        {
            _tapInput.TapCountChanged += OnTapCountChanged;
        }

        private void OnDisable()
        {
            _tapInput.TapCountChanged -= OnTapCountChanged;
        }

        private void OnTapCountChanged()
        {
            if (_tapInput.TapCount == 0 && _returnToDefault == null)
            {
                _returnToDefault = StartCoroutine(ReturnToDefaultColors());
                return;
            }

            if (_returnToDefault != null)
            {
                StopCoroutine(_returnToDefault);
                _returnToDefault = null;
            }

            var mainColor = _renderers[0].materials[0].GetColor(MainColor);
            var dimColor = _renderers[0].materials[0].GetColor(DimColor);
            float percent = _tapInput.TapCount / (float)_tapsCountToMaxHeat;

            _meshes.ForEach(mesh =>
            {
                mesh.localScale = Vector3.Lerp(mesh.localScale, _maxScale, percent);
            });
        }

        private IEnumerator ReturnToDefaultColors()
        {
            var time = _returnToDefaultTime;
            var mainColor = _renderers[0].materials[0].GetColor(MainColor);
            var dimColor = _renderers[0].materials[0].GetColor(DimColor);

            while (time >= 0f)
            {
                yield return null;
                time -= Time.deltaTime;

                _meshes.ForEach(mesh =>
                {
                    float percent = time / _returnToDefaultTime;
                    mesh.localScale = Vector3.Lerp(_defaultScale, mesh.localScale, percent);
                });
            }
        }
    }
}
