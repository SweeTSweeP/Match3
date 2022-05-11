using UnityEngine;

namespace Balls
{
    /// <summary>
    /// Ball entity describes color of ball and its position on array field 
    /// </summary>
    public class Ball : MonoBehaviour
    {
        [SerializeField] private BallColor ballColor;

        public Vector2 PlaceInFieldArray { get; set; }

        public BallColor BallColor => ballColor;
    }
}