using System.Collections.Generic;
using UnityEngine;

namespace Extensions
{
    public static class TransformExtensions
    {
        public static List<Transform> AllChild(this Transform parent)
        {
            var children = new List<Transform>();

            for (var i = 0; i < parent.childCount; i++)
            {
                children.Add(parent.GetChild(i));
            }

            return children;
        }
    }
}