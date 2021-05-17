using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jam3
{
    public class Snapper : MonoBehaviour
    {
        private bool canSnap;

        private bool hasTriggerCollision;

        private Vector3 snapToObjectOffset;

        private void Update()
        {
            if (Input.GetMouseButtonUp(0) && hasTriggerCollision)
            {
                canSnap = true;
            }

            if (Input.GetMouseButtonDown(0))
            {
                canSnap = false;
                hasTriggerCollision = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Snap")
            {
                if (TransformManager.Instance.SelectedObject == null)
                    return;

                if (TransformManager.Instance.SelectedObject.gameObject != other.transform.parent.parent.parent.parent.parent.gameObject)
                {
                    snapToObjectOffset = TransformManager.Instance.SelectedObject.GetWorldPosition() - transform.position;
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.tag == "Snap")
            {
                if (TransformManager.Instance.SelectedObject == null)
                    return;

                hasTriggerCollision = true;

                if (TransformManager.Instance.SelectedObject.gameObject != other.transform.parent.parent.parent.parent.parent.gameObject && canSnap)
                {
                    TransformManager.Instance.SelectedObject.SetWorldPosition(other.transform.position + snapToObjectOffset, 0);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Snap")
            {
                if (TransformManager.Instance.SelectedObject == null)
                    return;

                if (TransformManager.Instance.SelectedObject.gameObject != other.transform.parent.parent.parent.parent.parent.gameObject)
                {
                    canSnap = false;
                    hasTriggerCollision = false;
                }
            }
        }
    }
}


