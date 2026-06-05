using System;
using System.Collections;
using UnityEngine;

public class CreationDebugManager : MonoBehaviour
{
    [SerializeField] private bool createDummyPlayground;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private PlaygroundManager playgroundManager;
    [SerializeField] private CreatePlayground createPlayground;
    [SerializeField] private EventManager eventManager;
    private void Start()
    {
        if (createDummyPlayground)
        {
            StartCoroutine(DelayOnDebug());
        }
    }


    IEnumerator DelayOnDebug()
    {
        yield return new WaitForSeconds(1f);
        DebugCreatePlayground();
        DebugCreateEvent();
    }


    private void DebugCreatePlayground()
    {
        createPlayground.SetPlaygroundParameters("DummyPlayground", "This is a test playground", 0);
        createPlayground.CreateNewPlayground();
        //Debug.Log("Dummy Playground Created ");
    }


    private void DebugCreateEvent()
    {
        EventData eventData = new EventData();
        eventData.id = new Guid();
        eventData.name = "Dummy Event";
        eventData.debug = true;
        int playgroundIndex = GameManager.Instance.GameData.currentUser.curr_ply_index;
        GameManager.Instance.GameData.currentUser.playgrounds[playgroundIndex].event_data = new();
        GameManager.Instance.GameData.currentUser.playgrounds[playgroundIndex].event_data.Add(eventData);
        eventManager.EventCreated();
    }

}
