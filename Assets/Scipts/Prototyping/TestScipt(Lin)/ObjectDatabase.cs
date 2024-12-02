using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class ObjectsDatabase : ScriptableObject
{
    public List<ObjectData> objectsData;

    public void Put(ObjectData data)
    {
        objectsData.Add(data);
    }
    
    public ObjectData Get(int index)
    {
        return objectsData[index];
    }

    public int Count()
    {
        return objectsData.Count;
    }

    public void Clear()
    {
        Debug.LogWarning("Clearing Database");
        objectsData = new List<ObjectData>();
    }
}

[Serializable]
public class ObjectData
{

    public ObjectData(ProducerDescriptor producerDescriptor, int id, GameObject prefab)
    {
        Entity = producerDescriptor; 
        ID = id;
        Size = Vector2Int.one;
        Prefab = prefab;
    }
    
    [field: SerializeField]
    public ISelectableEntity Entity{ get; private set; }
    [field: SerializeField]
    public int ID { get; private set; }
    [field: SerializeField]
    public Vector2 Size { get; private set; } = Vector2Int.one;
    [field: SerializeField]
    public GameObject Prefab { get; private set; }
}