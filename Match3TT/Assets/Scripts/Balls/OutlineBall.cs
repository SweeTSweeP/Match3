using UnityEngine;

namespace Balls
{
    /// <summary>
    /// Outline ball if clicked
    /// </summary>
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