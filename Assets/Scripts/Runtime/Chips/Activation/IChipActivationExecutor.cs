namespace Game.Chips.Activation
{
    public interface IChipActivationExecutor
    {
        bool TryActivate(ChipModel pivotChipModel);
    }
}