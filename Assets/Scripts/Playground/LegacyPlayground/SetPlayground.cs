using System.Collections.Generic;
using UnityEngine;

public class SetPlayground : MonoBehaviour
{
    [SerializeField] private ObjectsDatabase database;
    [SerializeField] private Transform objectParent;

    void Start()
    {
        SetupPlayground();
    }

    private void SetupPlayground()
    {
        UserData userData = GameManager.Instance.GetGameData().currentUser;
        //Debug.Log("UserData: " + userData.currentPlaygroundIndex);
        //int len = userData.playgroundDatas[userData.currentPlaygroundIndex].placedProps.Count;
        //List<PropData> propData = userData.playgroundDatas[userData.currentPlaygroundIndex].placedProps;

      
    }

}

/*
 * 
 *   for (int i = 0; i < propData.Count; i++)
        {
            int selectedObjectIndex = database.objectData.FindIndex(data => data.ID == propData[i].iD);
            GameObject newObject =  Instantiate(database.objectData[selectedObjectIndex].Prefab, propData[i].position, Quaternion.identity);
            newObject.transform.SetParent(objectParent);
        }

 * 
         foreach (var item in gridLevel.PlacedObjects)
        {
            int selectedObjectIndex = database.objectData.FindIndex(data => data.ID == item.Value.ID);
            Instantiate(database.objectData[selectedObjectIndex].Prefab, item.Key, Quaternion.identity);

        }

 GridData gridBase = userData.playgroundDatas[userData.currentPlaygroundIndex].basePropData;
        GridData gridLevel = userData.playgroundDatas[userData.currentPlaygroundIndex].levelPropData;

        foreach (var item in gridBase.PlacedObjects)
        {
            int selectedObjectIndex = database.objectData.FindIndex(data => data.ID == item.Value.ID);
            Instantiate(database.objectData[selectedObjectIndex].Prefab, item.Key, Quaternion.identity);

        }

        foreach (var item in gridLevel.PlacedObjects)
        {
            int selectedObjectIndex = database.objectData.FindIndex(data => data.ID == item.Value.ID);
            Instantiate(database.objectData[selectedObjectIndex].Prefab, item.Key, Quaternion.identity);

        }

 
 */