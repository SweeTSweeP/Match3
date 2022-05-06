namespace Balls
{
    public interface IBallSwapper
    {
        Ball[,] Balls { get; set; }
        void SubscribeBallClick(BallMove ballMove);
    }
}