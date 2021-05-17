using UnityEngine;
using Jam3;
using DG.Tweening;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AnimateObject : MonoBehaviour
{
    public GameObject GameObject = null;

    [Header("Animate")]
    [Range(1, 100)]
    public int Springs = 3;

    [Range(1, 20)]
    public int DampingPower = 2;

    [Header("Spring")]
    [Range(1f, 100f)]
    public float Stiffness = 20f;
    [Range(1f, 20f)]
    public float Mass = 2f;
    [Range(0.0f, 10.0f)]
    public float Damping = 0.3f;

    private Vector3 position = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private Vector3 velocity = Vector3.zero;

    private float k = -1.0f;
    private float d = -1.0f;

    private Vector3 iniPosition = Vector3.zero;
    private Vector3 newPosition = Vector3.zero;
    private Vector3 finalPosition = Vector3.zero;

    private float value = 1.0f;
    private bool invert = false;
    private bool canAnimate = false;

    private RaycastHit[] rayHits;
    private int hitLayerMask = -1;

    void Start()
    {
        hitLayerMask = 1 << (int)Layers.Surface;
        rayHits = new RaycastHit[5];
    }

    void Update()
    {
        if (GameObject != null)
        {
            var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hits = Physics.RaycastNonAlloc(cameraRay, rayHits, 100.0f, hitLayerMask);
            if (hits > 0)
            {
                var nearestPoint = default(Vector3);
                var nearstPointSqrDst = float.MaxValue;

                for (int i = 0; i < hits; i++)
                {
                    var sqrDst = Vector3.SqrMagnitude(rayHits[i].point - cameraRay.origin);
                    if (sqrDst < nearstPointSqrDst)
                    {
                        nearestPoint = rayHits[i].point;
                        nearstPointSqrDst = sqrDst;
                    }
                }
                newPosition = nearestPoint;
            }

            k = -Stiffness;
            d = -Damping;

            SetPosition(0, 2, true, true);
            SetPosition(1, 1, false, true);
            SetPosition(2, 0, true, true);

            GameObject.transform.position = position;
            GameObject.transform.eulerAngles = rotation;
        }
    }

    private void SetPosition(int pos, int rot, bool setPosition = true, bool setRotation = false)
    {
        float force = k * (position[pos] - newPosition[pos]);
        float damping = d * velocity[pos];

        float acceleration = (force + damping) / Mass;

        velocity[pos] += acceleration * Time.deltaTime;

        if (setPosition)
            position[pos] += velocity[pos] * Time.deltaTime;

        if (setRotation)
        {
            float rotAngle = velocity[pos] * 0.4f;
            rotation[rot] = rotAngle;
        }
    }

    public void Animate()
    {
        if (canAnimate && GameObject != null)
        {
            canAnimate = false;

            iniPosition = GameObject.transform.position;
            newPosition = GameObject.transform.position;
            newPosition.x += invert ? 10f : -10f;
            value = 0.0f;

            finalPosition = Vector3.zero;
            finalPosition.x = iniPosition.x;
            finalPosition.y = iniPosition.y;
            finalPosition.z = iniPosition.z;

            float springs = (float)Springs;
            float damping = (float)DampingPower;

            DOTween.To(() => value, x => value = x, 1.0f, 2.0f).SetEase(Ease.Linear).OnUpdate(() => {

                float time = Mathf.Sin(Mathf.PI * springs * value) * Mathf.Pow(1.0f - value, damping);
                finalPosition.x = iniPosition.x - (newPosition.x - iniPosition.x) * time;

                GameObject.transform.position = finalPosition; //Vector3Extensions.Spring(iniPosition, newPosition, value);
            }).OnComplete(() => { canAnimate = true; });
        }

        invert = !invert;
    }
}

// editor tools
#if UNITY_EDITOR
[CustomEditor(typeof(AnimateObject))]
public class BabyAnimationControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (!Application.isPlaying) return;

        AnimateObject t = target as AnimateObject;
        if (GUILayout.Button("Animate"))
        {
            t.Animate();
        }
    }
}
#endif
