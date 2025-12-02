using System;
using UnityEngine;

public class RemovingState : IBuildingState
{
    private int gameObjectIndex = -1;
    Grid grid;
    PreviewSystem previewSystem;
    GridData floorData;
    GridData propData;
    ObjectPlacer objectPlacer;


    public RemovingState(Grid grid,
                         PreviewSystem previewSystem,
                         GridData floorData,
                         GridData propData,
                         ObjectPlacer objectPlacer)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.floorData = floorData;
        this.propData = propData;
        this.objectPlacer = objectPlacer;

        previewSystem.StartShowingRemovePreview();
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        Debug.Log("Remove State OnAction" +
            "PropData: " + propData.CanPlaceObjectAt(gridPosition, Vector2Int.one) +
            "FloorData: " + floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one));


        GridData selectedData = null;
        if (propData.CanPlaceObjectAt(gridPosition, Vector2Int.one) == false)
        {
            //Debug.Log("there is prop exists here");
            selectedData = propData;

            Debug.Log("Removing Object");
            gameObjectIndex = selectedData.GetRepresentationIndex(gridPosition);
            if (gameObjectIndex == -1) { return; }
            //Debug.Log("Object to be removed: " + selectedData.GetRepresentationIndex(gridPosition));

            selectedData.RemoveObjectAt(gridPosition);
            objectPlacer.RemoveObjectAt(gameObjectIndex);
        }
        else if (floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one) == false)
        {
            //Debug.Log("there is floor exists here");
            selectedData = floorData;

            Debug.Log("Removing Object");
            gameObjectIndex = selectedData.GetRepresentationIndex(gridPosition);
            if (gameObjectIndex == -1) { return; }
            //Debug.Log("Object to be removed: " + selectedData.GetRepresentationIndex(gridPosition));

            selectedData.RemoveObjectAt(gridPosition);
            objectPlacer.RemoveObjectAt(gameObjectIndex);
            //Debug.Log("data: " + selectedData.GetRepresentationIndex(gridPosition));
        }

     
        Vector3 cellPosition = grid.CellToWorld(gridPosition);
        previewSystem.UpdatePosition(cellPosition, CheckIfSelectionIsValid(gridPosition));

    }


    private bool CheckIfSelectionIsValid(Vector3Int gridPosition)
    {
        return !(propData.CanPlaceObjectAt(gridPosition, Vector2Int.one)
            && floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one));
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool validity = CheckIfSelectionIsValid(gridPosition);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), validity);
    }

    public void RotateStructure(int direction)
    {
        Debug.Log("Cant rotate on remove");
    }
}


/*
   

/---------------------------------------------------------------------------------------



 */