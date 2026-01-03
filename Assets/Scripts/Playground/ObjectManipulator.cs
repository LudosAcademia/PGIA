using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class ObjectManipulator : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    //private GameObject currentGameObject;
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

            //int index = gridManager.GridData.GetTileIndex(gridPosition.x, gridPosition.z, layer);
        }

    }

    public void RemoveObject(Vector3Int gridPosition, string layer)
    {
        gridManager.GridData.SetTileWithCord(gridPosition.x, gridPosition.z, -1, 0, layer);
        Destroy(gridManager.PlacedGameObjects[layer][gridPosition.x, gridPosition.z]);
        gridManager.PlacedGameObjects[layer][gridPosition.x, gridPosition.z] = null;

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

    public void SetObjectRotation(Vector3Int gridPosition, int rotY, string layer)
    {
        if (gridManager.PlacedGameObjects[layer][gridPosition.x, gridPosition.z] != null)
        {
            gridManager.PlacedGameObjects[layer][gridPosition.x, gridPosition.z].transform.GetChild(0).transform.Rotate(0, rotY, 0);
        }
    }



    public void MoveObject(Vector3Int oldGridPosition, Vector3Int newGridPosition, int currObjectId, string layer, GameObject currentGameObject)
    {
        if (oldGridPosition == newGridPosition)
            return;

        Vector3 worldPos = gridManager.Grid.CellToWorld(newGridPosition);
        Debug.Log("Last Tile Content: " + gridManager.GridData.gridLayers[layer][oldGridPosition.x, oldGridPosition.z].containId);
        
        Debug.Log("Old Grid Pos: " + oldGridPosition);
        Debug.Log("New Grid Pos: " + newGridPosition);

        gridManager.GridData.SetTileWithCord(oldGridPosition.x, oldGridPosition.z, -1, 0, layer);
        gridManager.GridData.SetTileWithCord(newGridPosition.x, newGridPosition.z, currObjectId,
            (int)currentGameObject.transform.eulerAngles.y, layer);

        GameObject newObj = Instantiate(currentGameObject);
        newObj.transform.position = worldPos;
        newObj.transform.SetParent(gridManager.GridContainer);

        gridManager.PlacedGameObjects[layer][newGridPosition.x, newGridPosition.z] = newObj;

        Destroy(gridManager.PlacedGameObjects[layer][oldGridPosition.x, oldGridPosition.z]);
        gridManager.PlacedGameObjects[layer][oldGridPosition.x, oldGridPosition.z] = null;

        Destroy(currentGameObject);
    }

}


/*
 
        GameObject newobj = Instantiate(currentGameObject);
        newobj.transform.position = newGridPosition;
        newobj.transform.SetParent(gridManager.GridContainer);
        gridManager.PlacedGameObjects[layer][newGridPosition.x, newGridPosition.z] = newobj;
        Destroy(gridManager.PlacedGameObjects[layer][oldGridPosition.x, oldGridPosition.z]);
        Destroy(currentGameObject);
        currentGameObject = null;
        gridManager.PlacedGameObjects[layer][oldGridPosition.x, oldGridPosition.z] = null;


        gridManager.PlacedGameObjects[layer][newGridPosition.x, newGridPosition.z] = Instantiate(currentGameObject);
        gridManager.PlacedGameObjects[layer][newGridPosition.x, newGridPosition.z].transform.position = newGridPosition;
        gridManager.PlacedGameObjects[layer][newGridPosition.x, newGridPosition.z].transform.GetChild(0).rotation = currentGameObject.transform.rotation;


        Destroy(gridManager.PlacedGameObjects[layer][oldGridPosition.x, oldGridPosition.z]);
        gridManager.PlacedGameObjects[layer][oldGridPosition.x, oldGridPosition.z] = null;
 
 */