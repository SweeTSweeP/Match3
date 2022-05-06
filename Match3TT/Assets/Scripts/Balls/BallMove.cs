using System;
using UnityEngine;

namespace Balls
{
    public class BallMove : MonoBehaviour
    {
        public event Action<BallMove> BallSelected;

        private void OnMouseUp() => 
            BallSelected?.Invoke(this);
    }
}