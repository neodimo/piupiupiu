using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPoolItem
{
    public GameObject objectToPool;
    public int amountToPool;
    public bool shouldExpand = true;
    [HideInInspector] public string prefabName;
    public GameObject parentOfObjectToPool;

}

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler SharedInstance;

    public List<GameObject> pooledObjects;
    public List<ObjectPoolItem> itemsToPool;
    

    private void Awake()
    {
        SharedInstance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        pooledObjects = new List<GameObject>();
        foreach (ObjectPoolItem item in itemsToPool)
        {
            item.prefabName = item.objectToPool.name;
            item.parentOfObjectToPool = new GameObject(item.prefabName);
            for (int i = 0; i < item.amountToPool; i++)
            {
                GameObject obj = (GameObject)Instantiate(item.objectToPool);
                obj.name = item.prefabName;
                obj.SetActive(false);
                obj.transform.SetParent(item.parentOfObjectToPool.transform);
                pooledObjects.Add(obj);
            }
        }
    }


    public GameObject GetPooledObject(GameObject prefab, Vector3 position, Quaternion quaternion)
    {
        
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].name == prefab.name)
            {
                pooledObjects[i].transform.position = position;
                pooledObjects[i].transform.rotation = quaternion;
                pooledObjects[i].SetActive(true);
                return pooledObjects[i];
            }
        }
        foreach (ObjectPoolItem item in itemsToPool)
        {
            if (item.prefabName == prefab.name)
            {
                if (item.shouldExpand)
                {
                    GameObject obj = (GameObject)Instantiate(item.objectToPool);
                    obj.name = item.prefabName;
                    obj.SetActive(false);
                    obj.transform.SetParent(item.parentOfObjectToPool.transform);
                    pooledObjects.Add(obj);
                    obj.transform.position = position;
                    obj.transform.rotation = quaternion;
                    obj.SetActive(true);
                    return obj;
                }
            }
        }
        return null;
    }
}
