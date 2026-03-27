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

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    public void CreateNewEvent()
    {
        EventData eventData = new EventData();

        int playgroundIndex = GameManager.Instance.GameData.currentUser.curr_ply_index;
        GameManager.Instance.GameData.currentUser.playgrounds[playgroundIndex].event_data.Add(eventData);
    }

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


}
