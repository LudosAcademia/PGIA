using UnityEngine;

public interface oldIBuildingState
{
    public abstract void EndState();
    public abstract void OnAction(Vector3Int gridPosition);
    public abstract void UpdateState(Vector3Int gridPosition);
}