using System;
using UnityEngine;

namespace Balls
{
    /// <summary>
    /// Try to move ball trigger. Activates every time when player click on ball
    /// </summary>
    public class BallMove : MonoBehaviour
    {
        public event Action<BallMove> BallSelected;

        private void OnMouseUp() => 
            BallSelected?.Invoke(this);
    }
}