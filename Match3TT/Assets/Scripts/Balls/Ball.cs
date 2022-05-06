using UnityEngine;

namespace Balls
{
    public class Ball : MonoBehaviour
    {
        [SerializeField] private BallColor ballColor;

        public Vector2 PlaceInFieldArray { get; set; }

        public BallColor BallColor => ballColor;
    }
}