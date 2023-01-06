namespace Game.Loading.Tasks
{
    public struct WeightedLoadingTask
    {
        public int Weight;
        public ILoadingTask Task;

        public WeightedLoadingTask(ILoadingTask task)
        {
            Weight = 1;
            Task = task;
        }
    }
}