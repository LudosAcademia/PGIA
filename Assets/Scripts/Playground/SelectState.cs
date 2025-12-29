using UnityEngine;

public class SelectState : ISelectionState
{
    Grid grid;
    PreviewSystem previewSystem;
    PlaygroundGrid gridData;
    public int selectedObjectIndex = -1;

    public SelectState(Grid grid, PreviewSystem previewSystem, PlaygroundGrid gridData)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.gridData = gridData;

        this.previewSystem.StartShowingCursor(Vector2Int.one);
        this.previewSystem.SetCursorColor(Color.white);
    }


    public void EndState()
    {
        //previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        //previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
        Debug.Log("Selected Pos in Grid: " + gridPosition);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        //previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), true);
        previewSystem.UpdateCursor(gridPosition);
    }
 }


/*
     private int selectedObjectIndex = -1;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectsDatabase database;
    GridData basePropData;
    GridData levelPropData;
    ObjectPlacer objectPlacer;

    public PlacementState(int iD,
                          Grid grid,
                          PreviewSystem previewSystem,
                          ObjectsDatabase database,
                          GridData basePropData,
                          GridData levelPropData,
                          ObjectPlacer objectPlacer)
    {
        ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.basePropData = basePropData;
        this.levelPropData = levelPropData;
        this.objectPlacer = objectPlacer;


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

        GridData selectedData = database.objectData[selectedObjectIndex].PropLevel == PropLevel.Base ? basePropData : levelPropData;
        selectedData.AddObject(gridPosition,
            database.objectData[selectedObjectIndex].Size,
            database.objectData[selectedObjectIndex].ID,
            index);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = database.objectData[selectedObjectIndex].PropLevel == PropLevel.Base ? basePropData : levelPropData;
        return selectedData.CanPlaceObjectAt(gridPosition, database.objectData[selectedObjectIndex].Size);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
    }
}

 
 */