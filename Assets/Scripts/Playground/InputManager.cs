using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private CinemachineCamera vCam;
    [SerializeField] private CinemachineOrbitalFollow vCamOrbitalFollow;
    [SerializeField] private CinemachineInputAxisController vCamAxisController;

    [SerializeField] private InputSystem inputSystem;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float zoomSpeed, minZoom, maxZoom;
    private Vector3 lastPosition;

    [SerializeField] private LayerMask placementLayerMask;
    [SerializeField] private Material graphShaderMat;
    [SerializeField] private Vector2 defaultGraphShaderTiling;
    private Vector2 currentGraphShaderTiling;
    public event Action OnClicked, OnExit, OnClickStarted, OnClickEnded, OnDebug, OnRightClick
        , OnWheelClickStarted, OnWheelClickEnded;


    private bool toggleVirtualCamera = false;
    private bool toggleVirtualCameraControls = false;

    private void Awake()
    {
        inputSystem = new InputSystem();
        graphShaderMat.SetVector("_Tiling", defaultGraphShaderTiling);
        currentGraphShaderTiling = defaultGraphShaderTiling;
    }

    private void OnEnable()
    {
        inputSystem.PlacementInput.Enable();
        SubscribeMouseInput();
        inputSystem.PlacementInput.Escape.started += EspaceClicked;
        inputSystem.PlacementInput.Debug.performed += DebugPressed;
        inputSystem.PlacementInput.MouseRightClick.performed += MouseRightClicked;

    }

    private void OnDisable()
    {
        inputSystem.PlacementInput.Disable();
        UnsubscribeMouseInput();
        inputSystem.PlacementInput.Escape.started -= EspaceClicked;
        inputSystem.PlacementInput.Debug.performed -= DebugPressed;
        inputSystem.PlacementInput.MouseRightClick.performed -= MouseRightClicked;
    }


    public void ToggleVirCameraControls(bool set)
    {
        toggleVirtualCamera = set;
    }

    private void ZoomCamera()
    {
        Vector2 scroll = inputSystem.PlacementInput.ZoomCamera.ReadValue<Vector2>();
        if (scroll.y != 0)
        {
            //Debug.Log(Mathf.Clamp(scroll.y, minZoom, maxZoom));
            if (scroll.y == 1)
            {
                float currentRad = vCamOrbitalFollow.Radius;
                float nextRad = currentRad + scroll.y;
                if (nextRad < maxZoom)
                {
                    vCamOrbitalFollow.Radius += scroll.y;
                }
            }

            if (scroll.y == -1)
            {
                float currentRad = vCamOrbitalFollow.Radius;
                float nextRad = currentRad + scroll.y;
                if (nextRad > minZoom)
                {
                    vCamOrbitalFollow.Radius += scroll.y;
                }
            }

            //vCamOrbitalFollow.Radius = Mathf.Clamp(scroll.y, minZoom, maxZoom);

        }

    }

    public Vector2 ZoomGraph()
    {
        Vector2 scroll = inputSystem.PlacementInput.ZoomGraph.ReadValue<Vector2>();
        return scroll;
    }

    private void MoveCamera()
    {

        if (toggleVirtualCamera)
        {
            vCamAxisController.enabled = true;
        }
        else
        {
            vCamAxisController.enabled = false;
        }
    }

    public void PointCamera(Vector3 pos)
    {
        Camera.main.transform.LookAt(pos);
    }

    public void SetCameraTarget(Transform target)
    {
        vCam.Target.TrackingTarget = target;
    }

    private void Update()
    {
        if (toggleVirtualCameraControls)
        {
            ZoomCamera();
            MoveCamera();
        }

    }

    private void MouseClickedStarted(InputAction.CallbackContext context)
    {
        if (!IsMouseOnGrid())
        {
            toggleVirtualCamera = true;
        }
        MouseClickStarted();

        MouseClicked();
        //enter = true;
        //Debug.Log("Mouse Clicked");
    }

    private void MouseClickedEnded(InputAction.CallbackContext context)
    {
        toggleVirtualCamera = false;
        MouseClickEnded();
        //enter = true;
        //Debug.Log("Mouse Clicked");
    }

    private void MouseWheelClickStarted(InputAction.CallbackContext context)
    {
        MouseWheelClickStarted();
    }

    private void MouseWheelClickEnded(InputAction.CallbackContext context)
    {
        MouseWheelClickEnded();
    }

    private void MouseRightClicked(InputAction.CallbackContext context)
    {
        MouseRightClick();
    }

    private void EspaceClicked(InputAction.CallbackContext context)
    {

        //exit = true;
        //Debug.Log("Mouse Pressed");
    }

    private void DebugPressed(InputAction.CallbackContext context)
    {
        DebugSignal();
    }

    private void MouseRightClick()
    {
        OnRightClick?.Invoke();
    }

    private void DebugSignal()
    {
        OnDebug?.Invoke();
    }

    private void MouseClicked()
    {
        OnClicked?.Invoke();
    }

    private void MouseClickStarted()
    {
        OnClickStarted?.Invoke();
    }

    private void MouseClickEnded()
    {
        OnClickEnded?.Invoke();
    }

    private void MouseWheelClickStarted()
    {
        OnWheelClickStarted?.Invoke();
    }

    private void MouseWheelClickEnded()
    {
        OnWheelClickEnded?.Invoke();
    }

    public bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();

    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = mainCamera.nearClipPlane; //dont select objects that are not rendered by camera
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        //Debug.Log("Hit point: " + Physics.Raycast(ray, out hit, 100));
        if (Physics.Raycast(ray, out hit, 100, placementLayerMask))
        {
            lastPosition = hit.point;
        }

        return lastPosition;
    }

    public Vector2 GetMousePosition()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        return (Vector2)mousePos;
    }

    public bool IsMouseOnGrid()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = mainCamera.nearClipPlane; //dont select objects that are not rendered by camera
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        //Debug.Log("Hit point: " + Physics.Raycast(ray, out hit, 100));
        if (Physics.Raycast(ray, out hit, 100, placementLayerMask))
        {
            return true;
        }

        return false;
    }

    public void SubscribeMouseInput()
    {
        inputSystem.PlacementInput.MouseClick.started += MouseClickedStarted;
        inputSystem.PlacementInput.MouseClick.canceled += MouseClickedEnded;
        inputSystem.PlacementInput.MouseWheelClick.started += MouseWheelClickStarted;
        inputSystem.PlacementInput.MouseWheelClick.canceled += MouseWheelClickEnded;
        toggleVirtualCameraControls = true;
    }

    public void UnsubscribeMouseInput()
    {
        inputSystem.PlacementInput.MouseClick.started -= MouseClickedStarted;
        inputSystem.PlacementInput.MouseClick.canceled -= MouseClickedEnded;
        inputSystem.PlacementInput.MouseWheelClick.started -= MouseWheelClickStarted;
        inputSystem.PlacementInput.MouseWheelClick.canceled -= MouseWheelClickEnded;
        toggleVirtualCameraControls = false;
    }

}

/*
 * 
 * 
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

 * 
 *             if (vCamOrbitalFollow.Radius > minZoom && vCamOrbitalFollow.Radius < maxZoom)
            {
            }

        if (scroll.y != 0)
        {
            Vector3 offset = vCamOrbitalFollow.TargetOffset;
            offset.z = Mathf.Clamp(offset.z - scroll.y * zoomSpeed, minZoom, maxZoom);
            vCamOrbitalFollow.TargetOffset = offset;
        }
 * 
 
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
