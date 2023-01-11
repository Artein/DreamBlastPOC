using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Chips;
using Game.Level;
using Game.Utils.Progression;
using JetBrains.Annotations;
using Progress = Game.Utils.Progression.Progress;

namespace Game.Loading.Tasks
{
    [UsedImplicitly]
    public class LoadProjectInstallerAddressablesTask : BaseLoadingTask
    {
        private readonly AddressableInject<ColoredChipsActivationConfig> _coloredChipsActivationConfigAddressable;
        private readonly AddressableInject<ChipViewsConfig> _chipViewsConfigAddressable;
        private readonly AddressableInject<LevelsConfig> _levelsConfigAddressable;
        private readonly Progress _progress = new();

        public override IProgressProvider Progress => _progress;

        public LoadProjectInstallerAddressablesTask(AddressableInject<ChipViewsConfig> chipViewsConfigAddressable,
            AddressableInject<ColoredChipsActivationConfig> coloredChipsActivationConfigAddressable,
            AddressableInject<LevelsConfig> levelsConfigAddressable)
        {
            _levelsConfigAddressable = levelsConfigAddressable;
            _coloredChipsActivationConfigAddressable = coloredChipsActivationConfigAddressable;
            _chipViewsConfigAddressable = chipViewsConfigAddressable;
        }

        public override string ToString()
        {
            return $"{nameof(LoadProjectInstallerAddressablesTask)}";
        }

        protected override async UniTask<bool> ExecuteAsync_Implementation(CancellationToken cancellationToken)
        {
            _progress.Progress01 = 0f;
            cancellationToken.ThrowIfCancellationRequested();
            await _chipViewsConfigAddressable;

            _progress.Progress01 = 0.33f;
            cancellationToken.ThrowIfCancellationRequested();
            await _coloredChipsActivationConfigAddressable;
            
            _progress.Progress01 = 0.66f;
            cancellationToken.ThrowIfCancellationRequested();
            await _levelsConfigAddressable;
            
            _progress.Progress01 = 1f;
            return true;
        }
    }
}