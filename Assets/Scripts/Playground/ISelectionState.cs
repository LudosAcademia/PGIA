using UnityEngine;

internal interface ISelectionState
{
    public abstract void EndState();
    public abstract void OnAction(Vector3Int gridPosition, string layer, GameObject selectedObject);
    public abstract void UpdateState(Vector3Int gridPosition);

}

