using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Transform cameraPivot;
    private Transform cameraPivotCache;
    private float rotateCameraPivot = 0;

    [SerializeField] private InputSystem inputSystem;
    [SerializeField] private Camera mainCamera;

    private Vector3 lastPosition;

    [SerializeField] private LayerMask placementLayerMask;

    public event Action OnClicked, OnExit;
    private bool enter, exit = false;

    private void Awake()
    {
        inputSystem = new InputSystem();
        cameraPivotCache = cameraPivot;
    }

    private void OnEnable()
    {
        inputSystem.PlacementInput.Enable();
        inputSystem.PlacementInput.MouseClick.started += MouseClicked;
        inputSystem.PlacementInput.Escape.started += EspaceClicked;

    }

    private void OnDisable()
    {
        inputSystem.PlacementInput.Disable();
        inputSystem.PlacementInput.MouseClick.started -= MouseClicked;
        inputSystem.PlacementInput.Escape.started -= EspaceClicked;
    }


    private void MouseClicked(InputAction.CallbackContext context)
    {
        enter = true;
        //Debug.Log("Mouse Clicked");
    }

    private void EspaceClicked(InputAction.CallbackContext context)
    {
        exit = true;
        //Debug.Log("Mouse Pressed");
    }

    private void Update()
    {
        if (enter)
        {
            OnClicked?.Invoke();
            enter = false;
        }

        if (exit)
        {
            OnExit?.Invoke();
            exit = false;
        }
    }

    public void ZoomCamera(float zoom)
    {
        mainCamera.transform.position = 
        Vector3.MoveTowards(mainCamera.transform.position, cameraPivotCache.transform.position, zoom);
    }

    public void RotateCamera(int dir)
    {
        rotateCameraPivot += dir;
        cameraPivot.Rotate(0, dir, 0);
    }

    public bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();


    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = mainCamera.nearClipPlane; //dont select objects that are not rendered by camera
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, placementLayerMask))
        {
            lastPosition = hit.point;
        }

        return lastPosition;
    }

}

/*
 
     private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClicked?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnExit?.Invoke();
        }

    }

 
 */
