using UnityEngine;
using Upgrades;

namespace Interactions.Resources
{
    public class ResourcePerTick : MonoBehaviour, IUpgradable
    {
        [SerializeField] private UpgradeData _resourcePerTickData;

        public UpgradeData UpgradeData => _resourcePerTickData;
    }
}
