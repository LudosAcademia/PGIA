using UnityEngine;

public class BuildingState : IBuildingState
{
    PreviewSystem previewSystem;
    PlaygroundGrid gridData;
    ObjectManipulator objectManipulator;

    public BuildingState(PreviewSystem previewSystem, PlaygroundGrid gridData, ObjectManipulator objectPlacer)
    {
        this.previewSystem = previewSystem;
        this.gridData = gridData;
        this.objectManipulator = objectPlacer;

        this.previewSystem.StartShowingCursor(Vector2Int.one);
        this.previewSystem.SetCursorColor(Color.green);

    }

    void IBuildingState.OnDotAction(Vector3Int gridPosition, int objectId, string layer)
    {
        //Debug.Log("Object Chaged: " + objectId + " Level: " + tile);

        Debug.Log(CheckValidity(gridData, gridPosition.x, gridPosition.z, layer));

        if (CheckValidity(gridData, gridPosition.x, gridPosition.z, layer))
        {
            gridData.gridLayers[layer][gridPosition.x, gridPosition.z].containId = objectId;
            objectManipulator.PlaceObject(gridPosition);
        }
        else
        {
            Debug.Log("There is another object there");
            return;
        }

    }

    void IBuildingState.UpdateState(Vector3Int gridPosition)
    {
        previewSystem.UpdateCursor(gridPosition);
    }

    private bool CheckValidity(PlaygroundGrid gridData, int x, int z, string layer)
    {
        return gridData.gridLayers[layer][x,z].containId == -1;
    }

}

