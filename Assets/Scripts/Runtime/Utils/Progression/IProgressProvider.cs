namespace Game.Utils.Progression
{
    public interface IProgressProvider
    {
        float Progress01 { get; }
        event ValueChangeHandler<float> Changed;
    }
}