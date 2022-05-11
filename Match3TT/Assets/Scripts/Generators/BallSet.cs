using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Balls;
using Infrastructure.Bootstrapper;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Generators
{
    /// <summary>
    /// Ball positioner 
    /// </summary>
    public class BallSet : IBallSet
    {
        private const int FieldSize = 6;
        private const int DistanceBetweenCells = 2;

        private readonly IBallSwapper ballSwapper;
        private readonly ICoroutineRunner coroutineRunner;

        private List<GameObject> ballsObjects;
        private Ball[,] balls;
        private Vector3 startPosition;
        private Transform playFieldTransform;
        private Transform ballsParent;

        private int score;

        public BallSet(IBallSwapper ballSwapper, ICoroutineRunner coroutineRunner)
        {
            this.ballSwapper = ballSwapper;
            this.coroutineRunner = coroutineRunner;
        }

        /// <summary>
        /// General method of balls setting
        /// </summary>
        public void SetBalls()
        {
            playFieldTransform = PlayFieldTransform();   
            InitBalls();
            InitStartPosition();
            InitBallObjects();
            SetupBalls();
        }

        /// <summary>
        /// Put random color ball in cell if its empty
        /// </summary>
        private void SetupBalls()
        {
            ballsParent = InstantiateBallParent();
            var firstBallPlacePosition = startPosition;

            for (var i = 0; i < FieldSize; i++)
            {
                for (var j = 0; j < FieldSize; j++)
                {
                    balls[i, j] ??= InstantiateBall(firstBallPlacePosition, ballsParent, i, j);

                    firstBallPlacePosition = UpdatePosition(
                        firstBallPlacePosition,
                        firstBallPlacePosition.x + DistanceBetweenCells,
                        firstBallPlacePosition.y);
                }

                firstBallPlacePosition = UpdatePosition(
                    firstBallPlacePosition,
                    1,
                    firstBallPlacePosition.y - DistanceBetweenCells);
            }

            ballSwapper.Balls = balls;
            ballSwapper.RowsDetected += UpdateField;
        }

        /// <summary>
        /// Create balls parent on scene
        /// </summary>
        /// <returns>New parent transform</returns>
        private Transform InstantiateBallParent()
        {
            var parent = new GameObject(Constants.BallsParent).transform;
            parent.SetParent(playFieldTransform);
            parent.localPosition = Vector3.zero;
            parent.localScale = Vector3.one;

            return parent;
        }

        /// <summary>
        /// Get new ball and set parent and position
        /// </summary>
        /// <param name="firstBallPlacePosition"></param>
        /// <param name="parent"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>Created ball</returns>
        private Ball InstantiateBall(Vector3 firstBallPlacePosition, Transform parent, int x, int y)
        {
            var newBall = Object.Instantiate(GetBall()).transform;
            newBall.SetParent(parent);
            newBall.localPosition = firstBallPlacePosition;
            newBall.localScale = Vector3.one;

            var ballMove = newBall.GetComponent<BallMove>();
            var ball = newBall.GetComponent<Ball>();
            ball.PlaceInFieldArray = new Vector2(x, y);
            
            ballSwapper.SubscribeBallClick(ballMove);

            return ball;
        }

        /// <summary>
        /// Load balls assets
        /// </summary>
        private void InitBallObjects()
        {
            ballsObjects = new List<GameObject>();

            foreach (var ballColor in Enum.GetValues(typeof(BallColor)))
                ballsObjects.Add(Addressables.LoadAssetAsync<GameObject>(ballColor + Constants.Ball).WaitForCompletion());
        }

        /// <summary>
        /// Update position for new ball 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>New position</returns>
        private Vector3 UpdatePosition(Vector3 position, float x = 0, float y = 0) => 
            new Vector3(x, y, position.z);

        /// <summary>
        /// Transform of PlayField object on scene
        /// </summary>
        /// <returns></returns>
        private Transform PlayFieldTransform() => 
            GameObject.FindWithTag(Constants.PlayField).transform;

        /// <summary>
        /// Initiate start position for first ball
        /// </summary>
        private void InitStartPosition() => 
            startPosition = new Vector3(1, 0, 0);

        /// <summary>
        /// Initiate array of created balls
        /// </summary>
        private void InitBalls() => 
            balls = new Ball[FieldSize, FieldSize];

        /// <summary>
        /// Get random color ball
        /// </summary>
        /// <returns></returns>
        private GameObject GetBall() => 
            ballsObjects[Random.Range(0, ballsObjects.Count)];

        /// <summary>
        /// Update field after balls swap
        /// </summary>
        /// <param name="ballsToDestroy"></param>
        private void UpdateField(List<Ball> ballsToDestroy) => 
            coroutineRunner.StartCoroutine(WaitForFieldUpdate(ballsToDestroy));

        /// <summary>
        /// Process of field updating
        /// </summary>
        /// <param name="ballsToDestroy"></param>
        /// <returns>Started coroutine</returns>
        private IEnumerator WaitForFieldUpdate(List<Ball> ballsToDestroy)
        {
            yield return new WaitForSeconds(0.3f);

            var emptyPlaces = ballsToDestroy
                .ToDictionary(k => k.PlaceInFieldArray, v => v.transform.localPosition);
            
            score += ballsToDestroy.Count;
            Debug.Log($"Score:{score}");
            
            DestroyBalls(ballsToDestroy);

            yield return new WaitForSeconds(0.3f);
            
            DropBalls();

            yield return new WaitForSeconds(0.3f);
            
            SetupBalls();

            score = 0;
        }

        /// <summary>
        /// Low positions for balls which should be lower after balls swap
        /// </summary>
        private void DropBalls()
        {
            for (var j = 0; j < FieldSize; j++)
            {
                var count = 0;
                
                for (var i = FieldSize-1; i >= 0; i--)
                {
                    if (balls[i, j] != null)
                    {
                        balls[i, j].transform.localPosition -= new Vector3(0, count * DistanceBetweenCells, 0);
                        balls[i, j].PlaceInFieldArray -= new Vector2(count, 0);

                        (balls[i, j], balls[i + count, j]) = (balls[i + count, j], balls[i, j]);

                        i -= count;
                        count = 0;
                    }
                    else
                    {
                        count++;
                    }
                }
            }
        }

        /// <summary>
        /// Destroy balls after swap
        /// </summary>
        /// <param name="ballsToDestroy"></param>
        private void DestroyBalls(List<Ball> ballsToDestroy)
        {
            foreach (var ball in ballsToDestroy.Where(ball =>
                         balls[(int) ball.PlaceInFieldArray.x, (int) ball.PlaceInFieldArray.y] != null))
            {
                Object.Destroy(balls[(int) ball.PlaceInFieldArray.x, (int) ball.PlaceInFieldArray.y].gameObject);
                balls[(int) ball.PlaceInFieldArray.x, (int) ball.PlaceInFieldArray.y] = null;
            }
        }
    }
}
