using Upgrades;

namespace Interactions.Resources
{
    public interface IResource : IUpgradable
    {
        public void IncreaseSpentResourceAmount(float amount = 0f);
        public void IncreaceUpgradeResourceAmount(int resourceAmount);
        public bool IsAbleToDecreaseResourceAmount(int amount = 0);
        public void DecreaseResourceAmount(int amount = 0);
    }
}
