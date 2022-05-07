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

        public void SetBalls()
        {
            playFieldTransform = PlayFieldTransform();   
            InitBalls();
            InitStartPosition();
            InitBallObjects();
            SetupBalls();
        }

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

        private Transform InstantiateBallParent()
        {
            var parent = new GameObject(Constants.BallsParent).transform;
            parent.SetParent(playFieldTransform);
            parent.localPosition = Vector3.zero;
            parent.localScale = Vector3.one;

            return parent;
        }

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

        private void InitBallObjects()
        {
            ballsObjects = new List<GameObject>();

            foreach (var ballColor in Enum.GetValues(typeof(BallColor)))
                ballsObjects.Add(Addressables.LoadAssetAsync<GameObject>(ballColor + Constants.Ball).WaitForCompletion());
        }

        private Vector3 UpdatePosition(Vector3 position, float x = 0, float y = 0) => 
            new Vector3(x, y, position.z);

        private Transform PlayFieldTransform() => 
            GameObject.FindWithTag(Constants.PlayField).transform;

        private void InitStartPosition() => 
            startPosition = new Vector3(1, 0, 0);

        private void InitBalls() => 
            balls = new Ball[FieldSize, FieldSize];

        private GameObject GetBall() => 
            ballsObjects[Random.Range(0, ballsObjects.Count)];

        private void UpdateField(List<Ball> ballsToDestroy) => 
            coroutineRunner.StartCoroutine(WaitForFieldUpdate(ballsToDestroy));

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
