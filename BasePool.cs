using System;
using System.Linq;

namespace UnityEngine.Pool
{
    /// <summary>
    /// Base class to simplify object pooling in Unity 2021.
    /// </summary>
    /// <typeparam name="T">MonoBehaviour object you'd like to perform pooling on</typeparam>
    public abstract class BasePool<T> : MonoBehaviour where T : MonoBehaviour
    {
        private T _prefab;
        private Transform _parent;
        private ObjectPool<T> _pool;

        public ObjectPool<T> Pool
        {
            get
            {
                if (_pool == null)
                {
                    throw new InvalidOperationException($"ObjectPool<{nameof(T)}>(): Init must be called before using {nameof(T)} pool");
                }

                return _pool;
            }
            private set => _pool = value;
        }

        /// <summary>
        /// Initializes the pool and creates the objects
        /// </summary>
        /// <param name="prefab">prefab type</param>
        /// <param name="parent">parent object transform</param>
        /// <param name="defaultCapacity">default capacity of the pool</param>
        /// <param name="maxSize">maximum capacity of the pool</param>
        /// <param name="collectionCheck">collection check the pool</param>
        public void Init(T prefab, Transform parent = null, bool collectionCheck = false, int defaultCapacity = 10, int maxSize = 20)
        {
            _prefab = prefab;

            if (parent == null)
            {
                _parent = new GameObject($"{nameof(T)} Pool").transform;
            }

            _parent = parent;

            Pool = new ObjectPool<T>(CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy, collectionCheck, defaultCapacity, maxSize);

            CreatePool(defaultCapacity);
        }

        /// <summary>
        /// Creates the objects and releases them to the pool
        /// </summary>
        protected virtual void CreatePool(int size)
        {
            for (int i = 0; i < size; i++)
            {
                Release(CreateFunc());
            }
        }

        /// <summary>
        /// Releases object to the pool
        /// </summary>
        /// <param name="instance"></param>
        protected virtual void Release(T instance) => _pool.Release(instance);

        /// <summary>
        /// Releases all active child object of the parent object of the pool
        /// </summary>
        /// <param name="parent"></param>
        public virtual void ReleaseAllActive(Transform parent)
        {
            foreach (var child in parent.GetComponentsInChildren<T>().Where(t => t.gameObject.activeSelf))
            {
                Release(child);
            }
        }

        /// <summary>
        /// Create a new instance of object and sets the parent
        /// </summary>
        /// <returns>new instance of object</returns>
        protected virtual T CreateFunc()
        {
            T instance = Instantiate(_prefab, Vector3.zero, Quaternion.identity);
            instance.transform.SetParent(_parent);

            return instance;
        }

        /// <summary>
        /// Event called when the instance is being taken from the pool
        /// </summary>
        /// <param name="instance"></param>
        protected virtual void ActionOnGet(T instance) => instance.gameObject.SetActive(true);

        /// <summary>
        /// Event called when the instance is being returned to the pool
        /// </summary>
        /// <param name="instance"></param>
        protected virtual void ActionOnRelease(T instance) => instance.gameObject.SetActive(false);

        /// <summary>
        /// Event called when the element can not be returned to the pool
        /// </summary>
        /// <param name="instance"></param>
        protected virtual void ActionOnDestroy(T instance) => Destroy(instance.gameObject);
    }
}