using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Balls
{
    /// <summary>
    /// Describes ball swap logic and also verify if its possible
    /// </summary>
    public class BallSwapper : IBallSwapper
    {
        private const int FieldSize = 6;
        
        private Ball selectedBall;

        public event Action<List<Ball>> RowsDetected;

        public Ball[,] Balls { get; set; }

        /// <summary>
        /// Observe each ball if was clicked
        /// </summary>
        /// <param name="ballMove"></param>
        public void SubscribeBallClick(BallMove ballMove) => 
            ballMove.BallSelected += DetectSelectedBall;

        /// <summary>
        /// Try to detect if two balls were selected
        /// </summary>
        /// <param name="ballMove"></param>
        private void DetectSelectedBall(BallMove ballMove)
        {
            if (IsSelectedBallClickedSecondTime(ballMove)) return;

            if (selectedBall == null)
                selectedBall = ballMove.gameObject.GetComponent<Ball>();
            else
                TryToSwapBalls(ballMove.gameObject.GetComponent<Ball>());
        }

        /// <summary>
        /// Method to check if two balls is near and if its possible destroy balls after swap
        /// </summary>
        /// <param name="secondBall"></param>
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

        /// <summary>
        /// Find balls which will be destroyed after swap
        /// </summary>
        /// <param name="secondBall"></param>
        /// <returns>list of balls to destroy</returns>
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

        /// <summary>
        /// Change balls positions on scene and on array
        /// </summary>
        /// <param name="secondBall"></param>
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

        /// <summary>
        /// Find rows of balls with similar color 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>list of balls in rows with the same color</returns>
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

        /// <summary>
        /// Return rows with same color balls after the swap
        /// </summary>
        /// <param name="firstCollectedBalls"></param>
        /// <returns>List of balls of linked rows</returns>
        private IEnumerable<List<Ball>> GetLinkedRows(List<Ball> firstCollectedBalls) =>
            firstCollectedBalls
                .Select(ball => UpDownBallsCollection((int)ball.PlaceInFieldArray.x, (int)ball.PlaceInFieldArray.y))
                .Where(secondCollectedBalls => secondCollectedBalls.Count >= 3);

        /// <summary>
        /// Horizontal search of same color balls
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>List of balls in row</returns>
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

        /// <summary>
        /// Vertical search of same color balls in row
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>List of balls in row</returns>
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

        /// <summary>
        /// Revert ball selection
        /// </summary>
        /// <param name="secondBall"></param>
        private void StopSwap(Ball secondBall)
        {
            selectedBall.gameObject.GetComponent<Outline>().enabled = false;
            secondBall.gameObject.GetComponent<Outline>().enabled = false;

            selectedBall = null;
        }

        /// <summary>
        /// Detect if selected ball was clicked again 
        /// </summary>
        /// <param name="ballMove"></param>
        /// <returns>True or false if selected ball was clicked second time</returns>
        private bool IsSelectedBallClickedSecondTime(BallMove ballMove)
        {
            if (selectedBall != ballMove.gameObject.GetComponent<Ball>()) return false;
            selectedBall = null;
            return true;
        }
        
        /// <summary>
        /// Check if second selected ball near to first one
        /// </summary>
        /// <param name="secondBall"></param>
        /// <returns>Status if two balls is near to each other</returns>
        private bool IsBallsNear(Ball secondBall) =>
            IsNearBottomCell(secondBall) ||
            IsNearTopCell(secondBall) ||
            IsNearLeftCell(secondBall) ||
            IsNearRightCell(secondBall);

        /// <summary>
        /// Check right cell
        /// </summary>
        /// <param name="secondBall"></param>
        /// <returns>Status if second ball near on right</returns>
        private bool IsNearRightCell(Ball secondBall)
        {
            var rightCellPosition = selectedBall.PlaceInFieldArray + new Vector2(0, 1);
            if (!(rightCellPosition.y < FieldSize)) return false;
            return rightCellPosition == secondBall.PlaceInFieldArray;
        }

        /// <summary>
        /// Check left cell
        /// </summary>
        /// <param name="secondBall"></param>
        /// <returns>Status if second ball near on left</returns>
        private bool IsNearLeftCell(Ball secondBall)
        {
            var leftCellPosition = selectedBall.PlaceInFieldArray - new Vector2(0, 1);
            if (!(leftCellPosition.y >= 0)) return false;
            return leftCellPosition == secondBall.PlaceInFieldArray;
        }

        /// <summary>
        /// Check bottom cell
        /// </summary>
        /// <param name="secondBall"></param>
        /// <returns>Status if second ball near on bottom</returns>
        private bool IsNearBottomCell(Ball secondBall)
        {
            var bottomCellPosition = selectedBall.PlaceInFieldArray + new Vector2(1, 0);
            if (!(bottomCellPosition.x < FieldSize)) return false;
            return bottomCellPosition == secondBall.PlaceInFieldArray;
        }

        /// <summary>
        /// Check up cell
        /// </summary>
        /// <param name="secondBall"></param>
        /// <returns>Status if second ball near on up</returns>
        private bool IsNearTopCell(Ball secondBall)
        {
            var topCellPosition = selectedBall.PlaceInFieldArray - new Vector2(1, 0);
            if (!(topCellPosition.x >= 0)) return false;
            return topCellPosition == secondBall.PlaceInFieldArray;
        }
    }
}