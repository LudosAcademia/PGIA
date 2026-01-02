using UnityEngine;
using UnityEngine.EventSystems;

public class OverUI : MonoBehaviour, IPointerEnterHandler,
    IPointerExitHandler
{
    private InputManager inputManager;

    private void Start()
    {
        inputManager = GameObject.FindGameObjectWithTag("InputManager").GetComponent<InputManager>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        inputManager.UnsubscribeMouseInput();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inputManager.SubscribeMouseInput();
    }
}
