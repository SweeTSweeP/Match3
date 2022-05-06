using System.Collections;
using UnityEngine;

namespace Infrastructure.Bootstrapper
{
    public interface ICoroutineRunner
    {
        public Coroutine StartCoroutine(IEnumerator coroutine);
    }
}