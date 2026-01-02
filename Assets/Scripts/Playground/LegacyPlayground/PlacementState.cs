using UnityEngine;

public class PlacementState : oldIBuildingState
{
    private int selectedObjectIndex = -1;
    int ID;
    Grid grid;
    oldPreviewSystem previewSystem;
    ObjectsDatabase database;
    GridData basePropData;
    GridData levelPropData;
    oldObjectPlacer objectPlacer;

    public PlacementState(int iD,
                          Grid grid,
                          oldPreviewSystem previewSystem,
                          ObjectsDatabase database,
                          GridData basePropData,
                          GridData levelPropData,
                          oldObjectPlacer objectPlacer)
    {
        ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.basePropData = basePropData;
        this.levelPropData = levelPropData;
        this.objectPlacer = objectPlacer;
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (placementValidity == false) { return; }


        int index = objectPlacer.PlaceObject(database.objectData[selectedObjectIndex].Prefab, grid.CellToWorld(gridPosition), selectedObjectIndex);

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        //database.objectData[selectedObjectIndex].TileLevel == GameEnums.TileLevel.Base ? basePropData :
        GridData selectedData = levelPropData;
        return selectedData.CanPlaceObjectAt(gridPosition, Vector2Int.one);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
    }
}

/*
         selectedObjectIndex = database.objectData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex > -1)
        {
            //cellIndicator.SetActive(true);
            previewSystem.StartShowingPlacementPreview(database.objectData[selectedObjectIndex].Prefab,
                database.objectData[selectedObjectIndex].Size);
        }
        else
        {
            throw new System.Exception($"No Object with ID {iD}");
        }



        GridData selectedData = database.objectData[selectedObjectIndex].PropLevel == PropLevel.Base ? basePropData : levelPropData;
        selectedData.AddObject(gridPosition,
            database.objectData[selectedObjectIndex].Size,
            database.objectData[selectedObjectIndex].ID,
            index);


    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (placementValidity == false) { return; }


        int index = objectPlacer.PlaceObject(database.propData[selectedObjectIndex].Prefab, grid.CellToWorld(gridPosition));

        GridData selectedData = database.propData[selectedObjectIndex].ID == 0 ? floorData : propData;
        selectedData.AddObject(gridPosition,
            database.propData[selectedObjectIndex].Size,
            database.propData[selectedObjectIndex].ID,
            index);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = database.propData[selectedObjectIndex].ID == 0 ? floorData : propData;
        return selectedData.CanPlaceObjectAt(gridPosition, database.propData[selectedObjectIndex].Size);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
    }

 
 
 
 */