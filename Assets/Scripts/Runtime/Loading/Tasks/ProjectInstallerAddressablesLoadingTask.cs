using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Chips;
using Game.Level;
using JetBrains.Annotations;

namespace Game.Loading.Tasks
{
    [UsedImplicitly]
    public class ProjectInstallerAddressablesLoadingTask : BaseLoadingTask
    {
        private readonly AddressableInject<ColoredChipsActivationConfig> _coloredChipsActivationConfigAddressable;
        private readonly AddressableInject<ChipViewsConfig> _chipViewsConfigAddressable;
        private readonly AddressableInject<LevelsConfig> _levelsConfigAddressable;

        public ProjectInstallerAddressablesLoadingTask(AddressableInject<ChipViewsConfig> chipViewsConfigAddressable,
            AddressableInject<ColoredChipsActivationConfig> coloredChipsActivationConfigAddressable,
            AddressableInject<LevelsConfig> levelsConfigAddressable)
        {
            _levelsConfigAddressable = levelsConfigAddressable;
            _coloredChipsActivationConfigAddressable = coloredChipsActivationConfigAddressable;
            _chipViewsConfigAddressable = chipViewsConfigAddressable;
        }

        protected override async UniTask<bool> ExecuteAsync_Implementation(CancellationToken cancellationToken)
        {
            SetProgress(0f);
            await _chipViewsConfigAddressable;
            SetProgress(0.33f);
            await _coloredChipsActivationConfigAddressable;
            SetProgress(0.66f);
            await _levelsConfigAddressable;
            SetProgress(1f);

            return true;
        }
    }
}