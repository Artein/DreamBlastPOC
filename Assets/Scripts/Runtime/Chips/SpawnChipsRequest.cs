using System;

namespace Game.Chips
{
    public class SpawnChipsRequest
    {
        public int ChipsCount { get; }
        public bool IsCompleted { get; private set; }
        public event Action<SpawnChipsRequest> Completed;

        public SpawnChipsRequest(int chipsCount)
        {
            ChipsCount = chipsCount;
        }

        public void MarkCompleted()
        {
            IsCompleted = true;
            Completed?.Invoke(this);
        }
    }
}