using System;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
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
    public event Action<int> OnRotate;

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
        inputSystem.PlacementInput.RotateLeft.started += RotatePropLeft;
        inputSystem.PlacementInput.RotateRight.started += RotatePropRight;

    }

    private void OnDisable()
    {
        inputSystem.PlacementInput.Disable();
        inputSystem.PlacementInput.MouseClick.started -= MouseClicked;
        inputSystem.PlacementInput.Escape.started -= EspaceClicked;
        inputSystem.PlacementInput.RotateLeft.started -= RotatePropLeft;
        inputSystem.PlacementInput.RotateRight.started -= RotatePropRight;
    }


    private void MouseClicked(InputAction.CallbackContext context)
    {
        MouseInput(0);
         //enter = true;
        //Debug.Log("Mouse Clicked");
    }

    private void EspaceClicked(InputAction.CallbackContext context)
    {
        MouseInput(1);

        //exit = true;
        //Debug.Log("Mouse Pressed");
    }

    private void RotatePropLeft(InputAction.CallbackContext context)
    {
        RotatePropInput(0);
    }

    private void RotatePropRight(InputAction.CallbackContext context)
    {
        RotatePropInput(1);

    }

    private void RotatePropInput(int rot)
    {

        OnRotate?.Invoke(rot);

    }

    private void MouseInput(int con)
    {
        if (con == 0)
        {
            OnClicked?.Invoke();

        }
        else
        {
            OnExit?.Invoke();

        }
    }


    public void ZoomCamera(float zoom)
    {
        mainCamera.transform.position =
        Vector3.MoveTowards(mainCamera.transform.position, cameraPivotCache.transform.position, zoom);
    }


    public void MovePlatform(int dir)
    {
        float increament = 1;
        switch (dir)
        {
            case 0: //left
                cameraPivot.position += new Vector3(-increament, 0, 0);
                break;
            case 1: //right
                cameraPivot.position += new Vector3(increament, 0, 0);
                break;
            case 2: //up
                cameraPivot.position += new Vector3(0, 0, increament);
                break;
            case 3: //down
                cameraPivot.position += new Vector3(0, 0, -increament);
                break;

        }

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

 
 */
