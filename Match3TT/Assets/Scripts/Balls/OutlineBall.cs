using System;
using UnityEngine;

namespace Balls
{
    [RequireComponent(typeof(Outline))]
    public class OutlineBall : MonoBehaviour
    {
        private Outline outline;

        private void Awake() => 
            outline = GetComponent<Outline>();

        private void OnMouseUp() => 
            outline.enabled = !outline.enabled;
    }
}