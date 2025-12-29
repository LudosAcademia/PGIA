using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ObjectsDatabase : ScriptableObject
{
    public List<ObjectData> objectData;

}

[Serializable]
public class ObjectData
{

    [field: SerializeField] public string Name { get; private set; } = "";
    [field: SerializeField] public int ID { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }
    [field: SerializeField] public InteractType Interaction { get; private set; } = InteractType.None;
    [field: SerializeField] public PropLevel PropLevel { get; private set; } = PropLevel.Base;


}

public enum InteractType
{
    Actor,
    Input,
    Output,
    Event,
    None
}

public enum PropLevel
{
   Base,
   Level
}