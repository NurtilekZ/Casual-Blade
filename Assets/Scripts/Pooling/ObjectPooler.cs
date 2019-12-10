using System.Collections.Generic;
using UnityEngine;

namespace Pooling
{
    public class ObjectPooler : MonoBehaviour
    {
        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int size;
        }

        #region Singleton
        public static ObjectPooler Instance;

        private void CreateSingleton()
        {
            Instance = this;
        }
        #endregion   

        public List<Pool> pools;
        public Dictionary<string, Queue<GameObject>> poolDictionary;
    
        private void Awake()
        {
            CreateSingleton();
            
            poolDictionary = new Dictionary<string, Queue<GameObject>>();

            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab, transform);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }
            
                poolDictionary.Add(pool.tag, objectPool);
            }
        }

        public GameObject SpawnFromPool(string objTag, Vector3 position, Quaternion rotation, Transform parent)
        {
            if (!poolDictionary.ContainsKey(objTag))
            {
                Debug.LogWarning($"Pool haven't {objTag} tag");
                return null;
            }
            GameObject objectToSpawn = poolDictionary[objTag].Dequeue();
        
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.parent = parent;
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

            IPooledObject pooledObject = objectToSpawn.GetComponent<IPooledObject>();


            if (pooledObject != null)
            {
                pooledObject.OnObjectSpawn();
            }

            poolDictionary[objTag].Enqueue(objectToSpawn);
            return objectToSpawn;
        }
    
        // Update is called once per frame
        public void DisableAllPooledObjects()
        {
            foreach (KeyValuePair<string,Queue<GameObject>> keyValuePair in poolDictionary)
            {
                foreach (GameObject gameObject in keyValuePair.Value)
                {
                    gameObject.transform.parent = transform.parent;
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
