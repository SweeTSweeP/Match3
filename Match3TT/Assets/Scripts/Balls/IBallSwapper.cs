using System;
using System.Collections.Generic;

namespace Balls
{
    /// <summary>
    /// Ball swapper abstraction
    /// </summary>
    public interface IBallSwapper
    {
        Ball[,] Balls { get; set; }
        event Action<List<Ball>> RowsDetected;
        void SubscribeBallClick(BallMove ballMove);
        
    }
}