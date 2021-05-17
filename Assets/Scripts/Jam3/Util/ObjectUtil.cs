using UnityEngine;

namespace Jam3.Util
{
    /// <summary>
    /// Object util.
    /// </summary>
    public class ObjectUtil
    {
        /// <summary>
        /// Sets object layer.
        /// </summary>
        /// <param name="gameObject">The game object.</param>
        /// <param name="layer">The layer.</param>
        /// <param name="allChildren">The all children.</param>
        public static void SetObjectLayer(GameObject gameObject, int layer, bool allChildren)
        {
            gameObject.layer = layer;

            if (allChildren)
                SetChildLayer(gameObject, layer);

        }

        /// <summary>
        /// Sets child layer.
        /// </summary>
        /// <param name="gameObject">The game object.</param>
        /// <param name="layer">The layer.</param>
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
