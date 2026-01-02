using UnityEngine;

public class ObjectManipulator : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    private GameObject currentGameObject;
    public void PlaceObject(Vector3Int gridPosition)
    {

        int currObjectId = gridManager.CurrentObjecyId;
        int selectedObjectIndex = gridManager.ObjectsDatabase.objectData.FindIndex(data => data.ID == currObjectId);
        //Debug.Log("Object Id: " + currentObjecyId);
        //Debug.Log("Object Placed: " + objectsDatabase.objectData[selectedObjectIndex].Name);

        if (selectedObjectIndex > -1)
        {
            GameObject newobj = Instantiate(gridManager.ObjectsDatabase.objectData[selectedObjectIndex].Prefab);
            newobj.transform.position = gridPosition;
            newobj.transform.SetParent(gridManager.GridContainer);

            string layer = gridManager.CurrentGridLayer;

            gridManager.PlacedGameObjects[layer][gridPosition.x, gridPosition.z] = newobj;

            int index = gridManager.GridData.GetTileIndex(gridPosition.x, gridPosition.z, layer);
        }

    }

    public void RemoveObject(Vector3Int gridPosition, string layer)
    {
        gridManager.GridData.SetTileContainId(gridPosition.x, gridPosition.z, -1, layer);
        Destroy(gridManager.PlacedGameObjects[layer][gridPosition.x, gridPosition.z]);
        gridManager.PlacedGameObjects[layer][gridPosition.x, gridPosition.z] = null;

    }

    public void StoreObject(GameObject selectedObject)
    {
        currentGameObject = Instantiate(selectedObject);
    }

    public void RotateObject()
    {


    }

}
