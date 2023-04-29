using UnityEngine;

namespace Interactions.Resources
{
    public class UpgradeResourceView : MonoBehaviour
    {
        [SerializeField] private UpgradeResource _upgradeResource;
        [SerializeField] private FlyingCurrencyView.FlyingCurrencyView _flyingCurrencyView;
        [Header("Added currency")]
        [SerializeField] private UpgradeCurrencyAddedView _upgradeCurrencyAddedView;
        [SerializeField] private Transform _currencySpawnPoint;
        [SerializeField] private Transform _currencyEndPoint;

        private void OnEnable()
        {
            _upgradeResource.ResourceAmountInited += OnResourceAmountInited;
            _upgradeResource.ResourceAmountUpdated += OnResourceAmountUpdated;
        }

        private void OnDisable()
        {
            _upgradeResource.ResourceAmountInited -= OnResourceAmountInited;
            _upgradeResource.ResourceAmountUpdated -= OnResourceAmountUpdated;
        }

        private void OnResourceAmountInited(int amount)
        {
            _flyingCurrencyView.UpdateCurrencyAmountWithAnimation(amount, 0);
        }

        private void OnResourceAmountUpdated(int amount, int addedAmount)
        {
            _flyingCurrencyView.UpdateCurrencyAmountWithAnimation(amount, addedAmount);

            if (addedAmount <= 0)
                return;

            var view = Instantiate(_upgradeCurrencyAddedView, _currencySpawnPoint);
            view.Init(addedAmount, _currencyEndPoint.localPosition);
        }
    }
}
