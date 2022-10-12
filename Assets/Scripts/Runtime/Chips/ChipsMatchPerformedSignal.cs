namespace Game.Chips
{
    public class ChipsMatchPerformedSignal
    {
        public int MatchSize { get; }
        
        public ChipsMatchPerformedSignal(int matchSize)
        {
            MatchSize = matchSize;
        }
    }
}