using System;
using System.Collections.Generic;
using Balls;
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

        private IBallSwapper ballSwapper;
        
        private List<GameObject> ballsObjects;
        private Ball[,] balls;
        private Vector3 startPosition;
        private Transform playFieldTransform;

        public BallSet(IBallSwapper ballSwapper) => 
            this.ballSwapper = ballSwapper;

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
            var parent = InstantiateBallParent();
            var firstBallPlacePosition = startPosition;

            for (var i = 0; i < FieldSize; i++)
            {
                for (var j = 0; j < FieldSize; j++)
                {
                    balls[i, j] = InstantiateBall(firstBallPlacePosition, parent, i, j);

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
    }
}
