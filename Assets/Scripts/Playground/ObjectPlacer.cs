using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{

    [SerializeField] private ObjectsDatabase objectsDatabase;
    [SerializeField] private Transform gridContainer;
    public void PlaceObject(Vector3 gridPosition, int objectId)
    {
        int selectedObjectIndex = objectsDatabase.objectData.FindIndex(data => data.ID == objectId);
        if (selectedObjectIndex > -1)
        {
            GameObject newobj = Instantiate(objectsDatabase.objectData[selectedObjectIndex].Prefab);
            newobj.transform.position = gridPosition;
            newobj.transform.SetParent(gridContainer);
        }


    }

}
