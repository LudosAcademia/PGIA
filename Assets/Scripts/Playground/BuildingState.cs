using UnityEngine;

public class BuildingState : IBuildingState
{
    Grid grid;
    PreviewSystem previewSystem;
    PlaygroundGrid gridData;
    ObjectPlacer objectPlacer;

    public BuildingState(Grid grid, PreviewSystem previewSystem, PlaygroundGrid gridData, ObjectPlacer objectPlacer)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.gridData = gridData;
        this.objectPlacer = objectPlacer;

        this.previewSystem.StartShowingCursor(Vector2Int.one);
        this.previewSystem.SetCursorColor(Color.green);

    }

    void IBuildingState.EndState()
    {
        throw new System.NotImplementedException();
    }

    void IBuildingState.OnBrushAction(Vector3Int gridPosition)
    {
        throw new System.NotImplementedException();
    }

    void IBuildingState.OnDotAction(Vector3Int gridPosition, int objectId, GameEnums.TileLevel tile)
    {
        if (CheckValidity(gridData, gridPosition.x, gridPosition.z, tile))
        {
            gridData.GridTilesBase[gridPosition.x, gridPosition.z].containId = objectId;
            objectPlacer.PlaceObject(gridPosition, objectId);
        }
        else
        {
            Debug.Log("There is another object there");
            return;
        }

    }

    void IBuildingState.OnFillAction(Vector3Int gridPosition)
    {
        throw new System.NotImplementedException();
    }

    void IBuildingState.UpdateState(Vector3Int gridPosition)
    {
        previewSystem.UpdateCursor(gridPosition);
    }

    private bool CheckValidity(PlaygroundGrid gridData, int x, int z, GameEnums.TileLevel tile)
    {
        if (tile == GameEnums.TileLevel.Base)
        {
            return gridData.GridTilesBase[x, z].contain == "empty";
        }
        else
        {
            return gridData.GridTilesLevel[x, z].contain == "empty";
        }
    }

}

