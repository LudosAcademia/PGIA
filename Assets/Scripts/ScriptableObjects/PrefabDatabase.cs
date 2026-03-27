using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabDatabase", menuName = "Scriptable Objects/PrefabDatabase")]
public class PrefabDatabase : ScriptableObject
{
    public List<PrefabObjects> prefabData;

}

[Serializable]
public class PrefabObjects
{
    public GameObject prefab;
    public Sprite symbol;
}