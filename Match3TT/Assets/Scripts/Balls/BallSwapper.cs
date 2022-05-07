using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Balls
{
    public class BallSwapper : IBallSwapper
    {
        private const int FieldSize = 6;
        
        private Ball selectedBall;

        public event Action<List<Ball>> RowsDetected;

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
                var balls = ComputeBallsToDestroy(secondBall);

                if (balls.Count >= 3)
                {
                    StopSwap(secondBall);
                    RowsDetected?.Invoke(balls);
                }
                else
                {
                    SwapBalls(secondBall);
                    StopSwap(secondBall);
                }
            }
            else
            {
                StopSwap(secondBall);
            }
        }

        private List<Ball> ComputeBallsToDestroy(Ball secondBall)
        {
            var ballsToCollectFromSelectedBall = CollectBalls(
                (int) selectedBall.PlaceInFieldArray.x,
                (int) selectedBall.PlaceInFieldArray.y);

            ballsToCollectFromSelectedBall = ballsToCollectFromSelectedBall
                .Where(s => s.BallColor == selectedBall.BallColor).ToList();

            if (ballsToCollectFromSelectedBall.Count < 3)
                ballsToCollectFromSelectedBall.Clear();

            var ballsToCollectFromSecondBall = CollectBalls(
                (int) secondBall.PlaceInFieldArray.x,
                (int) secondBall.PlaceInFieldArray.y);

            ballsToCollectFromSecondBall = ballsToCollectFromSecondBall
                .Where(s => s.BallColor == secondBall.BallColor).ToList();

            if (ballsToCollectFromSecondBall.Count >= 3)
                ballsToCollectFromSelectedBall.AddRange(ballsToCollectFromSecondBall);

            return ballsToCollectFromSelectedBall;
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
        }

        private List<Ball> CollectBalls(int x, int y)
        {
            var firstCollectedBalls = LeftRightBallsCollection(x, y);
            
            if (firstCollectedBalls.Count < 3) 
            {
                firstCollectedBalls.Clear();
                var secondCollectedBalls = UpDownBallsCollection(x, y);
                
                if (secondCollectedBalls.Count >= 3) firstCollectedBalls.AddRange(secondCollectedBalls);
            }
            else
            {
                var otherCollectedBalls = new List<Ball>();
                
                foreach (var secondCollectedBalls in GetLinkedRows(firstCollectedBalls))
                    otherCollectedBalls.AddRange(secondCollectedBalls);

                if (otherCollectedBalls.Count > 3) firstCollectedBalls.AddRange(otherCollectedBalls);
            }
            
            return firstCollectedBalls.Distinct().ToList();
        }

        private IEnumerable<List<Ball>> GetLinkedRows(List<Ball> firstCollectedBalls) =>
            firstCollectedBalls
                .Select(ball => UpDownBallsCollection((int)ball.PlaceInFieldArray.x, (int)ball.PlaceInFieldArray.y))
                .Where(secondCollectedBalls => secondCollectedBalls.Count >= 3);

        private List<Ball> LeftRightBallsCollection(int x, int y)
        {
            var collectedBalls = new List<Ball> {Balls[x, y]};

            for (var i = y; i < FieldSize-1; i++)
            {
                if (Balls[x, i].BallColor == Balls[x, i + 1].BallColor) collectedBalls.Add(Balls[x, i + 1]);
            }

            for (var i = y; i > 0; i--)
            {
                if (Balls[x, i].BallColor == Balls[x, i - 1].BallColor) collectedBalls.Add(Balls[x, i - 1]);
            }

            return collectedBalls;
        }

        private List<Ball> UpDownBallsCollection(int x, int y)
        {
            var collectedBalls = new List<Ball> {Balls[x, y]};

            for (var i = x; i < FieldSize - 1; i++)
            {
                if (Balls[i, y].BallColor == Balls[i + 1, y].BallColor) collectedBalls.Add(Balls[i + 1, y]);
            }
            
            for (var i = x; i > 0; i--)
            {
                if (Balls[i, y].BallColor == Balls[i - 1, y].BallColor) collectedBalls.Add(Balls[i - 1, y]);
            }

            return collectedBalls;
        }

        private void StopSwap(Ball secondBall)
        {
            selectedBall.gameObject.GetComponent<Outline>().enabled = false;
            secondBall.gameObject.GetComponent<Outline>().enabled = false;

            selectedBall = null;
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
            return rightCellPosition == secondBall.PlaceInFieldArray;
        }

        private bool IsNearLeftCell(Ball secondBall)
        {
            var leftCellPosition = selectedBall.PlaceInFieldArray - new Vector2(0, 1);
            if (!(leftCellPosition.y >= 0)) return false;
            return leftCellPosition == secondBall.PlaceInFieldArray;
        }

        private bool IsNearBottomCell(Ball secondBall)
        {
            var bottomCellPosition = selectedBall.PlaceInFieldArray + new Vector2(1, 0);
            if (!(bottomCellPosition.x < FieldSize)) return false;
            return bottomCellPosition == secondBall.PlaceInFieldArray;
        }

        private bool IsNearTopCell(Ball secondBall)
        {
            var topCellPosition = selectedBall.PlaceInFieldArray - new Vector2(1, 0);
            if (!(topCellPosition.x >= 0)) return false;
            return topCellPosition == secondBall.PlaceInFieldArray;
        }
    }
}