using System;
using UnityEngine;

public class PlayState : ISelectionState
{
    PreviewSystem previewSystem;
    GridManager gridManager;
    public static event Action<AvaItemPreBuild> OnTileItemSelect;

    private GameObject currentSelectedObject;
    private Vector3Int currentSelectedVector;
    private int currentSelectedObjectId;

    public int selectedObjectIndex = -1;

    public PlayState(PreviewSystem previewSystem, GridManager gridManager)
    {
        this.previewSystem = previewSystem;
        this.gridManager = gridManager;

        this.previewSystem.StartShowingCursor(Vector2Int.one);
        this.previewSystem.SetCursorColor(Color.white);
    }

    public void EndState()
    {
        //previewSystem.StopShowingPreview();
        previewSystem.StopHighlightSelectedTile();

    }

    public void OnAction(Vector3Int gridPosition, string layer, GameObject selectedObject)
    {
        //previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
        //Debug.Log("Selected Pos in Grid: " + gridPosition);
        previewSystem.StartHighlightSelectedTile(gridPosition);

        if (gridManager.PlaygroundGrid.grid[layer].data[gridPosition.x, gridPosition.z].containId != -1)
        {
            //Debug.Log("Grid Base: " + gridData.gridLayers[layer][gridPosition.x, gridPosition.z].containId);
            currentSelectedObject = selectedObject;
            currentSelectedVector = gridPosition;
            currentSelectedObjectId = gridManager.PlaygroundGrid.grid[layer].data[gridPosition.x, gridPosition.z].containId;


            int currObjectId = gridManager.CurrentObjecyId;
            int selectedObjectIndex = gridManager.ObjectsDatabase.objectData.FindIndex(data => data.ID == currObjectId);
            //Debug.Log(gridManager.ObjectsDatabase.objectData[selectedObjectIndex].Name);
            //Debug.Log(gridManager.ObjectsDatabase.objectData[selectedObjectIndex].Interaction);

            //Debug.Log("gridPosition.x, gridPosition.z " + gridPosition.x + " " + gridPosition.z);


        }
        else
        {
            Debug.Log("Grid Base: empty");
        }

    }

    public void UpdateState(Vector3Int gridPosition)
    {
        //previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), true);
        previewSystem.UpdateCursor(gridPosition);
    }

    public void OnRotate(Vector3Int gridPosition)
    {

    }

    public void OnMoveStart()
    {
    }

    public void OnMoveEnd(Vector3Int gridPosition, string layer)
    {
        //objectManipulator.MoveObject(currentSelectedVector, gridPosition, layer);
        //previewSystem.StartHighlightSelectedTile(gridPosition);

    }

}

/*
 
 
            if (gridManager.ObjectsDatabase.objectData[selectedObjectIndex].Interaction != GameEnums.InteractType.None)
            {
                Guid instanceId = gridManager.PlaygroundGrid.grid[layer].data[gridPosition.x, gridPosition.z].instanceId;
                Debug.Log("");
                AvaItemPreBuild itemRef = gridManager.PlaygroundGrid.logicReadyItems[instanceId];
                OnTileItemSelect?.Invoke(itemRef);
                Debug.Log("Selected Item: " + itemRef.itemName);
            }
 
 */