using System.Collections.Generic;
using UnityEngine;

public class oldObjectPlacer : MonoBehaviour
{
    private List<GameObject> placedGameObjects = new();
    [SerializeField] private Transform objectParent;


    internal int PlaceObject(GameObject prefab, Vector3 position, int iD)
    {
        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = position;
        newObject.name = iD.ToString();
        newObject.transform.SetParent(objectParent);
        placedGameObjects.Add(newObject);
        return placedGameObjects.Count - 1;
    }

    internal void RemoveObjectAt(int gameObjectIndex)
    {
        Debug.Log("Remove Object Prefab");
        if (placedGameObjects.Count <= gameObjectIndex || placedGameObjects[gameObjectIndex] == null) { return; }
        Destroy(placedGameObjects[gameObjectIndex]);
        placedGameObjects[gameObjectIndex] = null;

    }
}

/*

 */