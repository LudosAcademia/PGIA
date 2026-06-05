using System;
using TMPro;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;

    [SerializeField] private TMP_InputField eventName;
    [SerializeField] private Transform actorRef;
    [SerializeField] private Transform outputRef;
    [SerializeField] private Transform inputRefs;

    public Transform currentItemRef;
    public static event Action OnEventCreated;


    private void OnEnable()
    {
        PlaygroundManager.OnEndEventItemPanel += ResetEventUI;
        PlaygroundManager.OnEventCreateEnd += ResetEventUI;
    }

    private void OnDisable()
    {
        PlaygroundManager.OnEndEventItemPanel -= ResetEventUI;
        PlaygroundManager.OnEventCreateEnd -= ResetEventUI;
    }

    private void ResetEventUI()
    {
        if (actorRef.childCount != 0)
        {
            Destroy(actorRef.GetChild(0).gameObject);
        }

        if (outputRef.childCount != 0)
        {
            Destroy(outputRef.GetChild(0).gameObject);
        }

        if (inputRefs.childCount != 0)
        {
            for (int i = 0; i < inputRefs.childCount; i++)
            {
                Destroy(inputRefs.GetChild(i).gameObject);
            }
        }
    }

    public void CreateNewEvent()
    {
        if (IsRefsEmpty())
        {
            Debug.LogError("Please Assign the Refs");
            return;
        }

        if (string.IsNullOrWhiteSpace(eventName.text))
        {
            Debug.LogError("Event name cant be empty");
            return;
        }

        EventData eventData = new EventData();

        eventData.actorObjectRef = actorRef.GetChild(0).GetComponent<WorldItemRef>().itemRef;
        actorRef.GetChild(0).GetComponent<WorldItemRef>().itemRef.avalible = false;
        eventData.outputObjectRef = outputRef.GetChild(0).GetComponent<WorldItemRef>().itemRef;
        outputRef.GetChild(0).GetComponent<WorldItemRef>().itemRef.avalible = false;
        eventData.name = eventName.text;

        int len = inputRefs.childCount;
        for (int i = 0; i < len; i++)
        {
            eventData.inputObjectRefs.Add(inputRefs.GetChild(i).GetComponent<WorldItemRef>().itemRef);
            inputRefs.GetChild(i).GetComponent<WorldItemRef>().itemRef.avalible = false;
        }

        eventData.id = new Guid();

        int playgroundIndex = GameManager.Instance.GameData.currentUser.curr_ply_index;
        if (GameManager.Instance.GameData.currentUser.playgrounds[playgroundIndex].event_data == null)
        {
            GameManager.Instance.GameData.currentUser.playgrounds[playgroundIndex].event_data = new();
        }

        GameManager.Instance.GameData.currentUser.playgrounds[playgroundIndex].event_data.Add(eventData);
        EventCreated();
    }

    public void EventCreated()
    {
        OnEventCreated?.Invoke();
        ResetEventUI();
    }


    private bool IsRefsEmpty()
    {
        return actorRef.childCount == 0 || outputRef.childCount == 0 || inputRefs.childCount == 0;
    }

}

/*
 
    private void AssignEventInputs(bool set)
    {
        if (set)
        {

        }
        else
        {

        }
    }

    private void StoreItemRef()
    {

    }


 
 
 */