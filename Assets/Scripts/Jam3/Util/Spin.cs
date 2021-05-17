using UnityEngine;

namespace Jam3.Util
{
    public class Spin : MonoBehaviour
    {
        [SerializeField] Vector3 speed = default;

        void Update()
        {
            transform.Rotate(speed * Time.deltaTime);
        }
    }
}
