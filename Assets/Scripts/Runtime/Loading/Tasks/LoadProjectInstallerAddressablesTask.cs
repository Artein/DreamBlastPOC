using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Chips;
using Game.Level;
using JetBrains.Annotations;

namespace Game.Loading.Tasks
{
    [UsedImplicitly]
    public class LoadProjectInstallerAddressablesTask : BaseLoadingTask
    {
        private readonly AddressableInject<ColoredChipsActivationConfig> _coloredChipsActivationConfigAddressable;
        private readonly AddressableInject<ChipViewsConfig> _chipViewsConfigAddressable;
        private readonly AddressableInject<LevelsConfig> _levelsConfigAddressable;

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
            SetProgress01(0f);
            cancellationToken.ThrowIfCancellationRequested();
            await _chipViewsConfigAddressable;
            
            SetProgress01(0.33f);
            cancellationToken.ThrowIfCancellationRequested();
            await _coloredChipsActivationConfigAddressable;
            
            SetProgress01(0.66f);
            cancellationToken.ThrowIfCancellationRequested();
            await _levelsConfigAddressable;
            
            SetProgress01(1f);
            return true;
        }
    }
}