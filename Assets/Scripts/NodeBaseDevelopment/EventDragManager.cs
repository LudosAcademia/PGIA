using UnityEngine;

public class EventDragManager : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    private GameObject currentPlaceholderBox;
    private GameObject lastPlaceholderBox;
    private GameObject currentWorldItem;

    private void OnEnable()
    {
        AssignEventTriggers(true);
    }

    private void OnDisable()
    {
        AssignEventTriggers(false);
    }

    private void AssignEventTriggers(bool set)
    {
        if (set)
        {
            OverUITrigger.OnOverUI += AssignPlaceholderBox;
            WorldItemRef.OnWorldItemSelect += AssignWorldItem;
        }
        else
        {
            OverUITrigger.OnOverUI -= AssignPlaceholderBox;
            WorldItemRef.OnWorldItemSelect -= AssignWorldItem;
        }
    }


    private void AssignWorldItem(GameObject obj)
    {
        if (obj == null) { return; }
        currentWorldItem = obj;
    }

    private void AssignPlaceholderBox(GameObject curr, bool overUI)
    {
        Debug.Log("Placeholder: " + curr + " overUI: " + overUI);
        if (curr == null) { return; }
        if (currentWorldItem == null) { return; }

     
        if (overUI)
        {
            currentPlaceholderBox = curr;
            currentWorldItem.GetComponent<WorldItemRef>().localParent = currentPlaceholderBox.transform;
            //lastPlaceholderBox = currentWorldItem.GetComponent<WorldItemRef>().localParent.gameObject;
        }
        else
        {
            //currentWorldItem.GetComponent<WorldItemRef>().localParent = lastPlaceholderBox.transform;
            currentPlaceholderBox = null;
            currentWorldItem = null;
        }

    }

}

/*
 
         if (overUI)
        {
            lastPlaceholderBox = currentWorldItem.GetComponent<WorldItemRef>().localParent.gameObject;
        }
        else
        {
            currentWorldItem.GetComponent<WorldItemRef>().localParent = lastPlaceholderBox.transform;
            currentPlaceholderBox = null;
        }
 
 */