using System.Collections;
using UnityEngine;

namespace Infrastructure.Bootstrapper
{
    /// <summary>
    /// Return MonoBehaviour object logic for running coroutines
    /// </summary>
    public interface ICoroutineRunner
    {
        public Coroutine StartCoroutine(IEnumerator coroutine);
    }
}