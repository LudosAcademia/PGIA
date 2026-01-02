using GameEnums;
using UnityEngine;

public interface IBuildingState
{
    public abstract void OnDotAction(Vector3Int gridPosition, int objectId, string layer);

    public abstract void UpdateState(Vector3Int gridPosition);
}