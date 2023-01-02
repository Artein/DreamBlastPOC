namespace Game.Utils
{
    public interface IProgressProvider
    {
        float Progress01 { get; }
        event ChangedHandler Changed;
        public delegate void ChangedHandler(float value, float prevValue);
    }
}