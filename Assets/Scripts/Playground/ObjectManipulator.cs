using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

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

    public void RotateObject(Vector3Int gridPosition, GameObject selectedObject)
    {
        if (selectedObject != null)
        {
            selectedObject.transform.GetChild(0).transform.Rotate(0, 90, 0);

            if (gridManager.GridData.gridLayers[gridManager.CurrentGridLayer][gridPosition.x, gridPosition.z].rotY == 360)
            {
                gridManager.GridData.gridLayers[gridManager.CurrentGridLayer][gridPosition.x, gridPosition.z].rotY = 0;
            }
            else
            {
                gridManager.GridData.gridLayers[gridManager.CurrentGridLayer][gridPosition.x, gridPosition.z].rotY += 90;
            }
        }

    }


    public void MoveObject(Vector3Int oldGridPosition, Vector3Int newGridPosition, int currObjectId,string layer)
    {
        gridManager.GridData.SetTileContainId(oldGridPosition.x, oldGridPosition.z, -1, layer);
        gridManager.GridData.SetTileContainId(newGridPosition.x, newGridPosition.z, currObjectId, layer);
        Destroy(gridManager.PlacedGameObjects[layer][oldGridPosition.x, oldGridPosition.z]);
        gridManager.PlacedGameObjects[layer][oldGridPosition.x, oldGridPosition.z] = null;
        currentGameObject.transform.position = newGridPosition;
        gridManager.PlacedGameObjects[layer][newGridPosition.x, newGridPosition.z] = currentGameObject;

    }

}
