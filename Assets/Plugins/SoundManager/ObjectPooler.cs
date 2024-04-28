using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler<T>
{
    public delegate GameObject SpawnerDelegate(T instance);
    public Dictionary<string, GameObject> poolDictionary;
    private IPooledObject pooledObject;
    public SpawnerDelegate OnSpawned;

    public ObjectPooler()
    {
        poolDictionary = new Dictionary<string, GameObject>();
    }

    public void SpawnFromPool(T instance, string key)
    {
        if (!poolDictionary.ContainsKey(key))
        {
            AddElement(key, OnSpawned(instance));
        }
        else
        {
            SpawnFromPool(key);
        }
    }

    public void SpawnFromPool(string key)
    {
        GameObject objectToSpawn = poolDictionary[key];
        objectToSpawn.SetActive(true);
        Spawn(objectToSpawn);
    }
    
    private void Spawn(GameObject objectToSpawn)
    {
        pooledObject = objectToSpawn.GetComponent<IPooledObject>();
        if (pooledObject != null)
        {
            pooledObject.OnObjectSpawn();
        }
        else
        {
            Debug.LogWarning("pooled object is null!");
        }
    }

    public void AddElement(string key, GameObject GO, bool isSpawned = true)
    {
        poolDictionary.Add(key, GO);
        if (isSpawned)
        {
            Spawn(GO);
        }
        else
        {
            GO.SetActive(false);
        }
        
    }
}
