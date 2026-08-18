using System;
using UnityEngine;

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
            gridManager.PlaygroundGrid.grid[layer].visuals[gridPosition.x, gridPosition.z] = newobj;

            if (gridManager.ObjectsDatabase.objectData[selectedObjectIndex].Interaction != GameEnums.InteractType.None)
            {
                AvaItemPreBuild newItem = new AvaItemPreBuild();
                newItem.layer = layer;
                newItem.index = gridManager.PlaygroundGrid.grid[layer].data[gridPosition.x, gridPosition.z].index;
                newItem.containId = gridManager.ObjectsDatabase.objectData[selectedObjectIndex].ID;
                newItem.type = gridManager.ObjectsDatabase.objectData[selectedObjectIndex].Interaction;
                newItem.itemId = Guid.NewGuid();
                gridManager.PlaygroundGrid.grid[layer].data[gridPosition.x, gridPosition.z].instanceId = newItem.itemId;
                gridManager.PlaygroundGrid.grid[layer].data[gridPosition.x, gridPosition.z].logicTile = true;
                newItem.itemName = gridManager.ObjectsDatabase.objectData[selectedObjectIndex].Name;
                gridManager.PlaygroundGrid.logicReadyItems.Add(newItem.itemId, newItem);
            }

            //int index = gridManager.GridData.GetTileIndex(gridPosition.x, gridPosition.z, layer);
        }
    }

    public void PlaceObjectWithRotation(Vector3Int gridPosition, int rotY)
    {
        int currObjectId = gridManager.CurrentObjecyId;
        int selectedObjectIndex = gridManager.ObjectsDatabase.objectData.FindIndex(data => data.ID == currObjectId);
        //Debug.Log("Object Id: " + currentObjecyId);
        //Debug.Log("Object Placed: " + objectsDatabase.objectData[selectedObjectIndex].Name);

        if (selectedObjectIndex > -1)
        {
            GameObject newobj = Instantiate(gridManager.ObjectsDatabase.objectData[selectedObjectIndex].Prefab);
            newobj.transform.SetParent(gridManager.GridContainer);
            newobj.transform.position = gridPosition;
            //Debug.Log("On Pos: " + gridPosition + " The Rotation: " + rotY);
            newobj.transform.GetChild(0).transform.Rotate(0, rotY, 0);

            string layer = gridManager.CurrentGridLayer;
            gridManager.PlaygroundGrid.grid[layer].visuals[gridPosition.x, gridPosition.z] = newobj;

            //int index = gridManager.GridData.GetTileIndex(gridPosition.x, gridPosition.z, layer);
        }
    }


    public void RemoveObject(Vector3Int gridPosition, string layer)
    {
        GridTile emptyTile = new();
        gridManager.PlaygroundGrid.grid[layer].data[gridPosition.x, gridPosition.z] = emptyTile;
        Destroy(gridManager.PlaygroundGrid.grid[layer].visuals[gridPosition.x, gridPosition.z]);
        gridManager.PlaygroundGrid.grid[layer].visuals[gridPosition.x, gridPosition.z] = null;

    }

    public void RotateObject(Vector3Int gridPosition, GameObject selectedObject)
    {
        if (selectedObject != null)
        {
            //int posX = gridManager.GridData.grid[gridManager.CurrentGridLayer][gridPosition.x, gridPosition.z].x;
            //int posZ = gridManager.GridData.grid[gridManager.CurrentGridLayer][gridPosition.x, gridPosition.z].z;
            //Debug.Log("The Cords: " + posX + " / " + posZ);
            var tileRot = gridManager.PlaygroundGrid.grid[gridManager.CurrentGridLayer].data[gridPosition.x, gridPosition.z].rotY;
            //Add 90 deg and then take the mod of 360 in order to make sure the rotation is valid within the boudries of 360
            tileRot = (tileRot + 90) % 360;
            Debug.Log("Tile Rot: " + tileRot);
            selectedObject.transform.GetChild(0).transform.Rotate(0, 90, 0);
            gridManager.PlaygroundGrid.grid[gridManager.CurrentGridLayer].data[gridPosition.x, gridPosition.z].rotY = tileRot;
        }
    }

    public void MoveObject(Vector3Int oldGridPosition, Vector3Int newGridPosition, string layer)
    {
        if (oldGridPosition == newGridPosition)
            return;

        Vector3 worldPos = gridManager.Grid.CellToWorld(newGridPosition);
        //Debug.Log("Last Tile Content: " + gridManager.GridData.gridLayers[layer][oldGridPosition.x, oldGridPosition.z].containId);

        //Debug.Log("Old Grid Pos: " + oldGridPosition);
        //Debug.Log("New Grid Pos: " + newGridPosition);

        GameObject currentGameObject = gridManager.PlaygroundGrid.grid[layer].visuals[oldGridPosition.x, oldGridPosition.z].gameObject;

        int containId = gridManager.PlaygroundGrid.grid[layer].data[oldGridPosition.x, oldGridPosition.z].containId;
        int rotY = gridManager.PlaygroundGrid.grid[layer].data[oldGridPosition.x, oldGridPosition.z].rotY;

        gridManager.PlaygroundGrid.grid[layer].data[newGridPosition.x, newGridPosition.z].containId = containId;
        gridManager.PlaygroundGrid.grid[layer].data[newGridPosition.x, newGridPosition.z].rotY = rotY;


        GameObject newObj = Instantiate(currentGameObject);
        newObj.transform.position = worldPos;
        newObj.transform.SetParent(gridManager.GridContainer);

        gridManager.PlaygroundGrid.grid[layer].visuals[newGridPosition.x, newGridPosition.z] = newObj;
        Destroy(gridManager.PlaygroundGrid.grid[layer].visuals[oldGridPosition.x, oldGridPosition.z]);
        Destroy(currentGameObject);
        currentGameObject = null;
        gridManager.PlaygroundGrid.DeleteTile(layer, oldGridPosition.x, oldGridPosition.z);

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
             

if (gridManager.GridData.gridLayers[gridManager.CurrentGridLayer][gridPosition.x, gridPosition.z].rotY == 360)
            {
                gridManager.GridData.gridLayers[gridManager.CurrentGridLayer][gridPosition.x, gridPosition.z].rotY = 0;
            }
            else
            {
                gridManager.GridData.gridLayers[gridManager.CurrentGridLayer][gridPosition.x, gridPosition.z].rotY += 90;
            }

 Debug.Log("In RotateObject " + "Cords: " + gridPosition + "Tile Rot: " 
                + gridManager.GridData.gridLayers[gridManager.CurrentGridLayer][gridPosition.x, gridPosition.z].rotY);

 */