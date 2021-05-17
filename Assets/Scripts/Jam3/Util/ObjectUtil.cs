using UnityEngine;

namespace Jam3.Util
{
    public class ObjectUtil
    {
        public static void SetObjectLayer(GameObject gameObject, int layer, bool allChildren)
        {
            gameObject.layer = layer;

            if (allChildren)
                SetChildLayer(gameObject, layer);

        }

        private static void SetChildLayer(GameObject gameObject, int layer)
        {
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.layer = layer;
                if (child.childCount > 0)
                    SetChildLayer(child.gameObject, layer);
            }
        }
    }
}
