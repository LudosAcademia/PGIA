using GameEnums;
using UnityEngine;

public class RemoveState : IBuildingState
{
    PreviewSystem previewSystem;
    PlaygroundGrid gridData;
    ObjectManipulator objectManipulator;

    public RemoveState(PreviewSystem previewSystem, PlaygroundGrid gridData, ObjectManipulator objectPlacer)
    {
        this.previewSystem = previewSystem;
        this.gridData = gridData;
        this.objectManipulator = objectPlacer;

        this.previewSystem.StartShowingCursor(Vector2Int.one);
        this.previewSystem.SetCursorColor(Color.red);

    }

    void IBuildingState.OnDotAction(Vector3Int gridPosition, int objectId, string layer)
    {
        if (!CheckValidity(gridData, gridPosition.x, gridPosition.z, layer))
        {
            objectManipulator.RemoveObject(gridPosition, layer);
        }
        else
        {
            //Debug.Log("There is nothing to remove");
        }
    }


    void IBuildingState.UpdateState(Vector3Int gridPosition)
    {
        previewSystem.UpdateCursor(gridPosition);
    }


    private bool CheckValidity(PlaygroundGrid gridData, int x, int z, string layer)
    {
        return gridData.grid[layer].data[x, z].containId == -1;
    }

}
