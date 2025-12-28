using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    private List<GameObject> placedGameObjects = new();
    private Vector3 newObjectRotation = Vector3.zero;
    [SerializeField] private Transform objectParent;


    internal int PlaceObject(GameObject prefab, Vector3 position, int iD)
    {
        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = position;
        newObject.transform.rotation = Quaternion.Euler(newObjectRotation);
        newObject.name = iD.ToString();
        newObject.transform.SetParent(objectParent);
        placedGameObjects.Add(newObject);
        return placedGameObjects.Count - 1;
    }

    internal void RotateObject(int dir)
    {
        if (dir == 0)
        {
            newObjectRotation -= new Vector3(0, 90, 0);
        }
        else
        {
            newObjectRotation += new Vector3(0, 90, 0);
        }
    }

    internal void RemoveObjectAt(int gameObjectIndex)
    {
        Debug.Log("Remove Object Prefab");
        if (placedGameObjects.Count <= gameObjectIndex || placedGameObjects[gameObjectIndex] == null) { return; }
        Destroy(placedGameObjects[gameObjectIndex]);
        placedGameObjects[gameObjectIndex] = null;

    }
}
