using UnityEngine;

internal interface ISelectionState
{
    public abstract void EndState();
    public abstract void OnAction(Vector3Int gridPosition);
    public abstract void UpdateState(Vector3Int gridPosition);

}

