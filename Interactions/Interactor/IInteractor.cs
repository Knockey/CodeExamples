using Airplane;
using Interactions.Resources;
using Upgrades;

namespace Interactions
{
    public interface IInteractor
    {
        public float ResourcePerTick { get; }
        public bool IsResourceMaxed { get; }
        public IMovement Movement { get; }
        public ILander Lander { get; }
        public IResource UpgradeResource { get; }
        public InteractionResource InteractionResource { get; }
        public IUpgradable ResourcePerTickUpgradable { get; }
    }
}
