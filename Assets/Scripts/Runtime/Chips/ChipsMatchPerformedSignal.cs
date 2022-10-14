namespace Game.Chips
{
    public class ChipsMatchPerformedSignal : ISignal
    {
        public int MatchSize { get; }
        
        public ChipsMatchPerformedSignal(int matchSize)
        {
            MatchSize = matchSize;
        }
    }
}