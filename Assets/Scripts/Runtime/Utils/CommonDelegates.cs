namespace Game.Utils
{
    public delegate void ValueChangeHandler<in T>(T value, T prevValue);
}