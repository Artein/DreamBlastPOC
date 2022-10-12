using System;

namespace Game.Level.Stages
{
    public interface ILevelStageGoal
    {
        bool IsFinished { get; }
        event Action<ILevelStageGoal> Finished;
    }
}