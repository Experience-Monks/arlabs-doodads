using UnityEngine;
using UnityEditor;

namespace Jam3.Util
{
    /// <summary>
    /// Singleton.
    /// </summary>
    /// <seealso cref="MonoBehaviour" />
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static T Instance
        {
            get {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name;
                        _instance = obj.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Awake.
        /// </summary>
        public virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else if (_instance != this as T)
            {
#if UNITY_EDITOR
                if (EditorApplication.isPlayingOrWillChangePlaymode)
                {
#endif
                    Debug.Log("Destroying " + this.gameObject.name + " as " + _instance.gameObject.name + " is already the instance.");
                    Destroy(gameObject);
#if UNITY_EDITOR
                }
#endif
            }
        }

        /// <summary>
        /// Gets the is instantiated.
        /// </summary>
        /// <value>
        /// The is instantiated.
        /// </value>
        public static bool IsInstantiated
        {
            get {
                return _instance != null;
            }
        }
    }
}
