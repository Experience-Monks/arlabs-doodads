using UnityEngine;

namespace Jam3
{
    public class CollisionDetect : MonoBehaviour
    {
        public ARObject Controller = null;

        // Runtime
        private bool collidingObject = false;
        private bool collidingSurface = false;

        private MeshCollider selfTriggerCollider = null;
        private MeshCollider selfMeshCollider = null;

        /// <summary>
        /// Awakes this instance.
        /// </summary>
        void Awake()
        {
            var colliders = GetComponents<MeshCollider>();
            if (colliders.Length > 2)
                Debug.LogWarning("CollisionDetect:: You must have only 2 mesh colliders on this mesh and one must be convex");

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].convex)
                {
                    selfTriggerCollider = colliders[i];
                    selfTriggerCollider.isTrigger = true;
                }
                else
                {
                    selfMeshCollider = colliders[i];
                }
            }

            if (Controller == null)
                Controller = gameObject.GetComponentInParent<ARObject>();

            collidingSurface = false;
            collidingObject = false;
        }

        void FixedUpdate()
        {
            collidingSurface = false;
            collidingObject = false;
        }

        void Update()
        {
            // if (Controller.IsCustomizable)
            // {
            //     if (collidingObject)
            //         Controller.Customization.ChangeColorAmount(1.0f);
            //     else
            //         Controller.Customization.ChangeColorAmount(0.0f);
            // }

            if (Controller.IsPhysic && Controller.Physics.AudioController != null)
            {
                Controller.Physics.AudioController.CanPlayAudio = collidingSurface;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (Controller != null && Controller.Selected)
            {
                if (IsColliderObject(other))
                    collidingObject = true;
            }
        }

        private void OnCollisionEnter(Collision collisionInfo)
        {
            if (IsColliderSurface(collisionInfo.collider))
            {
                if (Controller.IsPhysic && Controller.Physics.AudioController != null)
                    Controller.Physics.AudioController.PlayHit();
            }
        }

        private void OnCollisionStay(Collision collisionInfo)
        {
            if (IsColliderSurface(collisionInfo.collider))
                collidingSurface = true;
        }

        private bool IsColliderObject(Collider other)
        {
            bool isCollider = (other.gameObject.layer == (int)Layers.Draggable || other.gameObject.layer == (int)Layers.DraggableAction) &&
             other != selfTriggerCollider &&
             other != selfMeshCollider &&
             (other.GetType() == typeof(MeshCollider));

            return isCollider;
        }

        private bool IsColliderSurface(Collider other)
        {
            bool isCollider = (other.gameObject.layer == (int)Layers.Surface ||
            other.gameObject.layer == (int)Layers.Draggable ||
            other.gameObject.layer == (int)Layers.BackgroundMesh) &&
             other != selfTriggerCollider &&
             other != selfMeshCollider &&
             (other.GetType() == typeof(MeshCollider));

            return isCollider;
        }
    }
}
