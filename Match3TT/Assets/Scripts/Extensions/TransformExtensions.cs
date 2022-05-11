using System.Collections.Generic;
using UnityEngine;

namespace Extensions
{
    /// <summary>
    /// Unity.Transform extensions
    /// </summary>
    public static class TransformExtensions
    {
        /// <summary>
        /// Collect transform children 
        /// </summary>
        /// <param name="parent"></param>
        /// <returns>List of children transforms</returns>
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