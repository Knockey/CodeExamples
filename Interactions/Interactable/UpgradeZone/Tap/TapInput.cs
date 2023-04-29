using LoadingScreenView;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Interactions
{
    public class TapInput : MonoBehaviour
    {
        private const int UILayerMaskID = 5;
        private const float ScreenDivider = 2f;
        private const float ScaleModifier = 2.35f;
        private const string TutorialPref = "TUTORIAL_COMPLETE";
        private const string CompletePref = "COMPLETED";


        [SerializeField, Min(0f)] private int _tapsToStartSpeedIncreace = 3;
        [SerializeField, Min(0f)] private float _disableAfterSeconds = 1f;
        [SerializeField] private ViewPanel _tapReminderViewPanel;
        [Header("Particle")]
        [SerializeField] private Canvas _vfxCanvas;
        [SerializeField] private ParticleSystem _tapParticle;
        [Header("UI raycast check")]
        [SerializeField] private GraphicRaycaster _raycaster;
        [SerializeField] private EventSystem _eventSystem;

        private Coroutine _tapObserver;
        private PointerEventData _pointerEventData;

        public int TapCount { get; private set; } = 0;
        public bool IsTaping { get; private set; } = false;

        public event Action TapCountChanged;

        private void Awake()
        {
            _pointerEventData = new PointerEventData(_eventSystem);
            _tapReminderViewPanel.DisableView();
        }

        public void StartTapObserver()
        {
            StopTapObserver();
            _tapObserver = StartCoroutine(ObserveTaps());

            if (PlayerPrefs.GetString(TutorialPref, "") == CompletePref)
            {
                _tapReminderViewPanel.EnableView();
            }
        }

        public void StopTapObserver()
        {
            if (_tapObserver != null)
            {
                StopCoroutine(_tapObserver);
            }
            _tapReminderViewPanel.DisableView();
        }

        private IEnumerator ObserveTaps()
        {
            var time = 0f;
            TapCount = 0;
            TapCountChanged?.Invoke();

            while (enabled)
            {
                if (Input.GetMouseButtonDown(0) && IsCastedOnUI() == false)
                {
                    time = 0f;
                    TapCount += 1;
                    TapCountChanged?.Invoke();

                    TryInstantiateTapParticle();

                    if (TapCount >= _tapsToStartSpeedIncreace)
                    {
                        IsTaping = true;
                        _tapReminderViewPanel.DisableView();
                    }
                }

                if (time > _disableAfterSeconds && IsTaping)
                {
                    IsTaping = false;
                    TapCount = 0;
                    TapCountChanged?.Invoke();

                    if (PlayerPrefs.GetString(TutorialPref, "") == CompletePref)
                    {
                        _tapReminderViewPanel.EnableView();
                    }
                }

                yield return null;

                time += Time.deltaTime;
            }
        }

        private void TryInstantiateTapParticle()
        {
            var particle = Instantiate(_tapParticle, _vfxCanvas.transform);
            var main = particle.main;
            main.stopAction = ParticleSystemStopAction.Destroy;

            var tapPosition = new Vector3((Input.mousePosition.x - Screen.width / ScreenDivider) / ScaleModifier,
                (Input.mousePosition.y - Screen.height / ScreenDivider) / ScaleModifier,
                0f);
            particle.transform.localPosition = tapPosition;
        }

        private bool IsCastedOnUI()
        {
            _pointerEventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            _raycaster.Raycast(_pointerEventData, results);

            return results.Count > 0 && results[0].gameObject.layer == UILayerMaskID;
        }
    }
}
