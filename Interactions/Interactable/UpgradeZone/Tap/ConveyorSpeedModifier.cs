using UnityEngine;

namespace Interactions
{
    [RequireComponent(typeof(TapInput))]
    public class ConveyorSpeedModifier : MonoBehaviour
    {
        private const float DefaultSpawnSpeedModifier = 1f;
        private const float DefaultMoveSpeedModifier = 1f;

        [SerializeField, Min(DefaultSpawnSpeedModifier)] private float _spawnSpeedModifier = 1.7f;
        [SerializeField, Min(DefaultMoveSpeedModifier)] private float _moveSpeedModifier = 2f;

        private TapInput _tapInput;
        private bool _isSpawnSpeedActive = true;
        private bool _isMoveSpeedActive = true;

        public float CurrentSpawnSpeedModifier => _tapInput.IsTaping && _isSpawnSpeedActive ? _spawnSpeedModifier : DefaultSpawnSpeedModifier;
        public float CurrentMoveSpeedModifier => _tapInput.IsTaping && _isMoveSpeedActive ? _moveSpeedModifier : DefaultMoveSpeedModifier;

        private void Awake()
        {
            _tapInput = GetComponent<TapInput>();
        }

        public void StartTapObserver()
        {
            _tapInput.StartTapObserver();
        }

        public void StopTapObserver()
        {
            _tapInput.StopTapObserver();
        }
    }
}
