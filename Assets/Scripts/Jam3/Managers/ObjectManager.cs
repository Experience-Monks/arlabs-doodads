using System;
using System.Collections;
using UnityEngine;
using Jam3.Util;

namespace Jam3
{
    [Serializable]
    public class InteractiveElementTextureGroup
    {
        public Sprite Icon = null;

        public Texture2D Diffuse = null;
        public Texture2D Specular = null;
        public Texture2D Glosiness = null;
        public Texture2D Normal = null;
    }

    [Serializable]
    public class InteractiveElement
    {
        public int Id = 0;
        public GameObject PrefabObject = null;
        public Sprite Icon = null;
        public InteractiveElementTextureGroup[] ObjectTextures = null;
    }

    [Serializable]
    public class BallElement
    {
        public int Id = 0;
        public GameObject PrefabObject = null;
        public Sprite Icon = null;
    }

    public class ObjectManager : Singleton<ObjectManager>
    {
        public PhysicObject BallObject { get; set; }

        [Header("Elements")]
        public InteractiveElement[] InteractiveElements = null;
        public BallElement[] BallElements = null;

        public float PositionLineWidthMultiplier = 1.0f;
        public float LineWidthMultiplier = 1.0f;

        public float BallSnapSpeed = 4f;

        private GameObject ObjectContainer = null;
        private Vector3 ballSpawnPosition = Vector3.zero;

        void Start()
        {
            ObjectContainer = new GameObject();
            ObjectContainer.name = "ObjectContaier";
        }

        public void SpawnObject(int index)
        {
            if (index > -1 && index < InteractiveElements.Length)
            {
                // Instantiate new object
                var newObject = Instantiate(InteractiveElements[index].PrefabObject, PlacementManager.Instance.HitPosition, Quaternion.identity);
                newObject.transform.SetParent(ObjectContainer.transform, false);

                // Initialize object
                var arObject = newObject.GetComponent<ARObject>();
                arObject.ID = index;
                if (arObject.IsCustomizable)
                    arObject.Customization.ChangeTexture(0);

                // Let the placement manager position the new object
                PlacementManager.Instance.SetSelectedObject(arObject);
            }
        }

        public void SpawnBall(int index)
        {
            if (index > -1 && index < BallElements.Length)
            {
                if (BallObject != null)
                    ResetBall();
                else
                {
                    ballSpawnPosition = PlacementManager.Instance.HitPosition;

                    GameObject newObject = Instantiate(BallElements[index].PrefabObject, ballSpawnPosition, Quaternion.identity);
                    newObject.transform.SetParent(ObjectContainer.transform, false);

                    BallObject = newObject.GetComponent<PhysicObject>();
                    BallObject.ARBase.ID = -1;
                }

                PlacementManager.Instance.SetSelectedObject(BallObject.ARBase);
                StartCoroutine("MoveBallToHighestSnap");
            }
        }

        private IEnumerator MoveBallToHighestSnap()
        {
            yield return new WaitForSeconds(0.1f);
            if (BallObject != null)
            {
                Vector3 fallbackPosition = BallObject.ARBase.GetWorldPosition();
                BallObject.ARBase.SetWorldPosition(GetBallStartPoint(fallbackPosition), BallSnapSpeed);
            }
        }

        public void MoveBallToSnap(Vector3 snapPosition)
        {
            if (BallObject != null && BallObject.ARBase.Selected)
                BallObject.ARBase.SetWorldPosition(snapPosition, BallSnapSpeed);
        }

        public void UpdateBallInitialPosition(Vector3 position)
        {
            if (BallObject != null)
                ballSpawnPosition = position;
        }

        public void ResetBall()
        {
            if (BallObject != null)
            {
                BallObject.ARBase.Physics.Replay();
                BallObject.ARBase.SetWorldPosition(ballSpawnPosition, -1f);
            }
        }

        public void Reset()
        {
            BallObject = null;
        }

        public void Restart()
        {
            foreach (Transform child in ObjectContainer.transform)
            {
                Destroy(child.gameObject);
            }
        }

        public Vector3 GetBallStartPoint(Vector3 fallbackPosition)
        {
            Vector3 position = fallbackPosition;
            float height = -10000f;

            foreach (Transform child in ObjectContainer.transform)
            {
                ARObject arobject = child.gameObject.GetComponent<ARObject>();
                if (arobject != null && arobject.SnapObject != null)
                {
                    if (arobject.SnapObject.position.y > height)
                    {
                        position = arobject.SnapObject.position;
                        height = arobject.SnapObject.position.y;
                    }
                }
            }

            return position;
        }
    }
}
