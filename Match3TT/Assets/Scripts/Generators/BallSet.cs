using System;
using System.Collections.Generic;
using System.Linq;
using Balls;
using Extensions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Random = UnityEngine.Random;

namespace Generators
{
    public class BallSet : MonoBehaviour
    {
        private const int FieldSize = 6;

        [SerializeField] private Transform ballsParent;
        [SerializeField] private Transform ballPlacesTransform;

        private List<GameObject> _ballsObjects;
        private Ball[,] _balls;

        private void Awake()
        {
            InitBalls();
            InitBallObjects();

            FillBallsArray();
        }

        private void InitBallObjects()
        {
            _ballsObjects = new List<GameObject>();

            foreach (var ballColor in Enum.GetValues(typeof(BallColor)))
                _ballsObjects.Add(Addressables.LoadAssetAsync<GameObject>(ballColor + "Ball").WaitForCompletion());
        }

        private void InitBalls() => 
            _balls = new Ball[FieldSize, FieldSize];

        private void FillBallsArray()
        {
            var i = 0;
            var j = 0;

            var sortedBallPositions = SortBallPositions();

            foreach (var ballPlace in sortedBallPositions)
            {
                _balls[i,j] = Instantiate(
                    GetBall(), 
                    ballPlace.transform.position, 
                    Quaternion.identity, ballsParent).GetComponent<Ball>();
                
                j++;

                if (j < FieldSize) continue;
                
                j = 0;
                i++;
            }
        }

        private List<Transform> SortBallPositions() =>
            ballPlacesTransform.transform
                .AllChild()
                .OrderBy(s => s.position.x)
                .ThenBy(s => s.position.y)
                .ToList();

        private GameObject GetBall() => 
            _ballsObjects[Random.Range(0, _ballsObjects.Count)];
    }
}
