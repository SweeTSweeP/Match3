using UnityEngine;

namespace Balls
{
    public class BallSwapper : IBallSwapper
    {
        private const int FieldSize = 6;
        
        private Ball selectedBall;
        private Direction direction;

        public Ball[,] Balls { get; set; }

        public void SubscribeBallClick(BallMove ballMove) => 
            ballMove.BallSelected += DetectSelectedBall;

        private void DetectSelectedBall(BallMove ballMove)
        {
            if (IsSelectedBallClickedSecondTime(ballMove)) return;

            if (selectedBall == null)
                selectedBall = ballMove.gameObject.GetComponent<Ball>();
            else
                TryToSwapBalls(ballMove.gameObject.GetComponent<Ball>());
        }

        private void TryToSwapBalls(Ball secondBall)
        {
            if (IsBallsNear(secondBall))
            {
                SwapBalls(secondBall);
            }
            else
            {
                StopSwap(secondBall);
            }
        }

        private void SwapBalls(Ball secondBall)
        {
            (Balls[(int) selectedBall.PlaceInFieldArray.x, (int) selectedBall.PlaceInFieldArray.y],
                    Balls[(int) secondBall.PlaceInFieldArray.x, (int) secondBall.PlaceInFieldArray.y]) = 
                (Balls[(int) secondBall.PlaceInFieldArray.x, (int) secondBall.PlaceInFieldArray.y],
                    Balls[(int) selectedBall.PlaceInFieldArray.x, (int) selectedBall.PlaceInFieldArray.y]);

            (selectedBall.gameObject.transform.localPosition, secondBall.gameObject.transform.localPosition) = 
                (secondBall.gameObject.transform.localPosition, selectedBall.gameObject.transform.localPosition);

            (selectedBall.PlaceInFieldArray, secondBall.PlaceInFieldArray) = 
                (secondBall.PlaceInFieldArray, selectedBall.PlaceInFieldArray);

            StopSwap(secondBall);
        }

        private void StopSwap(Ball secondBall)
        {
            selectedBall.gameObject.GetComponent<Outline>().enabled = false;
            secondBall.gameObject.GetComponent<Outline>().enabled = false;

            selectedBall = null;
            direction = Direction.None;
        }

        private bool IsSelectedBallClickedSecondTime(BallMove ballMove)
        {
            if (selectedBall != ballMove.gameObject.GetComponent<Ball>()) return false;
            selectedBall = null;
            return true;
        }

        private bool IsBallsNear(Ball secondBall) =>
            IsNearBottomCell(secondBall) ||
            IsNearTopCell(secondBall) ||
            IsNearLeftCell(secondBall) ||
            IsNearRightCell(secondBall);

        private bool IsNearRightCell(Ball secondBall)
        {
            var rightCellPosition = selectedBall.PlaceInFieldArray + new Vector2(0, 1);
            if (!(rightCellPosition.y < FieldSize)) return false;
            if (rightCellPosition != secondBall.PlaceInFieldArray) return false;
            direction = Direction.Right;
            return true;

        }

        private bool IsNearLeftCell(Ball secondBall)
        {
            var leftCellPosition = selectedBall.PlaceInFieldArray - new Vector2(0, 1);
            if (!(leftCellPosition.y >= 0)) return false;
            if (leftCellPosition != secondBall.PlaceInFieldArray) return false;
            direction = Direction.Left;
            return true;
        }

        private bool IsNearBottomCell(Ball secondBall)
        {
            var bottomCellPosition = selectedBall.PlaceInFieldArray + new Vector2(1, 0);
            if (!(bottomCellPosition.x < FieldSize)) return false;
            if (bottomCellPosition != secondBall.PlaceInFieldArray) return false;
            direction = Direction.Bottom;
            return true;
        }

        private bool IsNearTopCell(Ball secondBall)
        {
            var topCellPosition = selectedBall.PlaceInFieldArray - new Vector2(1, 0);
            if (!(topCellPosition.x >= 0)) return false;
            if (topCellPosition != secondBall.PlaceInFieldArray) return false;
            direction = Direction.Up;
            return true;
        }
    }
}