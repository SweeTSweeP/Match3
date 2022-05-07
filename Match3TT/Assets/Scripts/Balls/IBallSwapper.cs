using System;
using System.Collections.Generic;

namespace Balls
{
    public interface IBallSwapper
    {
        Ball[,] Balls { get; set; }
        event Action<List<Ball>> RowsDetected;
        void SubscribeBallClick(BallMove ballMove);
        
    }
}