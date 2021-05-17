using UnityEngine;

namespace Jam3.Util
{
    public class WireframeRender : MonoBehaviour
    {
        // Attach this script to a camera, this will make it render in wireframe
        void OnPreRender()
        {
            GL.wireframe = true;
        }

        void OnPostRender()
        {
            GL.wireframe = false;
        }
    }
}
