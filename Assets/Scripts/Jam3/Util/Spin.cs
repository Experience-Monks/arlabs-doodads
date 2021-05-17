using UnityEngine;

namespace Jam3.Util
{
    /// <summary>
    /// Spin.
    /// </summary>
    /// <seealso cref="MonoBehaviour" />
    public class Spin : MonoBehaviour
    {
        [SerializeField] Vector3 speed = default;

        /// <summary>
        /// Update.
        /// </summary>
        private void Update()
        {
            transform.Rotate(speed * Time.deltaTime);
        }
    }
}
