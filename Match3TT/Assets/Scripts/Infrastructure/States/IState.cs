namespace Infrastructure.States
{
    /// <summary>
    /// State abstraction
    /// </summary>
    public interface IState
    {
        void Enter();
        void Exit();
    }
}