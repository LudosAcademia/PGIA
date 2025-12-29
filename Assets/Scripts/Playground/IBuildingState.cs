using GameEnums;
using UnityEngine;

public interface IBuildingState
{
    public abstract void EndState();

    public abstract void OnDotAction(Vector3Int gridPosition, int objectId, GameEnums.TileLevel tile);

    public abstract void OnFillAction(Vector3Int gridPosition);

    public abstract void OnBrushAction(Vector3Int gridPosition);

    public abstract void UpdateState(Vector3Int gridPosition);
}