using UnityEngine;

namespace Jam3
{
    public class CameraCollision : MonoBehaviour
    {
        public bool IsOverMesh { get; private set; }
        public Layers MeshLayer = Layers.BackgroundMesh;

        private void Start()
        {
            Reset();
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer == (int)MeshLayer)
                IsOverMesh = true;
            else
                IsOverMesh = false;
        }

        private void OnTriggerExit(Collider other)
        {
            IsOverMesh = false;
        }

        public void Reset()
        {
            IsOverMesh = false;
        }
    }
}
