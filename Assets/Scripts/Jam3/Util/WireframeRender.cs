using UnityEngine;

namespace Jam3.Util
{
    /// <summary>
    /// Wireframe render.
    /// </summary>
    /// <seealso cref="MonoBehaviour" />
    public class WireframeRender : MonoBehaviour
    {
        // Attach this script to a camera, this will make it render in wireframe
        /// <summary>
        /// Ons pre render.
        /// </summary>
        private void OnPreRender()
        {
            GL.wireframe = true;
        }

        /// <summary>
        /// Ons post render.
        /// </summary>
        private void OnPostRender()
        {
            GL.wireframe = false;
        }
    }
}
