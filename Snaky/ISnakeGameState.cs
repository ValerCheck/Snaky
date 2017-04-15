namespace Snaky
{
    public interface ISnakeGameVisualState<T> where T : class
    {
        T State { get; }
        void UpdateState();
    }
}
