using UnityEngine;
using UnityEngine.Rendering;

namespace Jam3
{
    [RequireComponent(typeof(ARObject))]
    public class LineDrawer : MonoBehaviour
    {
        public Material LineMaterial = null;
        public float LineWidth = 1f;

        private ARObject cachedArObjectComponent;
        private LineRenderer line;
        private RaycastHit[] rayHits = new RaycastHit[5];
        private int hitLayerMask = -1;
        private Transform TranslateTransformObject;


        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            // Get required components
            cachedArObjectComponent = GetComponent<ARObject>();
            TranslateTransformObject = cachedArObjectComponent.TranslateObject.transform;

            // Create raycast auxiliary structs
            rayHits = new RaycastHit[5];
            hitLayerMask = 1 << (int)Layers.Draggable | 1 << (int)Layers.BackgroundMesh | 1 << (int)Layers.Surface;

            // Create line renderer
            CreateLineRenderer();
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            SetLinePosition();
        }

        /// <summary>
        /// Creates the line renderers.
        /// </summary>
        private void CreateLineRenderer()
        {
            if (LineMaterial != null)
            {
                // Create each wire
                var go = new GameObject();
                go.name = "Line";
                go.transform.SetParent(gameObject.transform, false);

                line = go.AddComponent<LineRenderer>();
                line.material = LineMaterial;
                line.widthMultiplier = LineWidth * ObjectManager.Instance.PositionLineWidthMultiplier;
                line.positionCount = 2;
                line.numCapVertices = 8;
                line.numCornerVertices = 8;
                line.loop = false;
                line.receiveShadows = false;
                line.shadowCastingMode = ShadowCastingMode.Off;
                line.useWorldSpace = true;
            }
        }

        /// <summary>
        /// Sets the position of the line.
        /// </summary>
        private void SetLinePosition()
        {
            if (line != null && TranslateTransformObject != null)
            {
                Vector3 startPosition = TranslateTransformObject.position;
                Vector3 endPosition = TranslateTransformObject.position;

                if (cachedArObjectComponent.Selected)
                {
                    var hits = Physics.RaycastNonAlloc(startPosition, -Vector3.up, rayHits, 100, hitLayerMask, QueryTriggerInteraction.Ignore);
                    if (hits > 0)
                    {
                        var highestPosition = Vector3.negativeInfinity;
                        for (int i = 0; i < hits; i++)
                        {
                            if (rayHits[i].point.y > highestPosition.y)
                                highestPosition = rayHits[i].point;
                        }
                        endPosition = highestPosition;
                    }
                }

                line.enabled = startPosition != endPosition;
                line.SetPosition(0, startPosition);
                line.SetPosition(1, endPosition);
            }
        }
    }
}
