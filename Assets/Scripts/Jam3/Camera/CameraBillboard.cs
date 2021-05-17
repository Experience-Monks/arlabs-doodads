using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jam3.Effects
{
    public class CameraBillboard : MonoBehaviour
    {
        [Header("Config")]
        public bool IsCilindrical = true;

        [Header("Camera")]
        public Camera MainCamera = null;

        void Awake()
        {
            if (MainCamera == null)
                MainCamera = Camera.main;
        }

        void LateUpdate()
        {
            if(MainCamera != null)
            {
                var target = MainCamera.transform.position;

                if(IsCilindrical)
                    target.y = transform.position.y;

                transform.LookAt(target);
            }
        }
    }
}
